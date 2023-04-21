using EL;
using EL.Async;
using EL.Robot;
using EL.Robot.Core;
using EL.Robot.Core.Websocket;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using Protos;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Utils
{
    public struct Packet
    {
        public const int MinPacketSize = 2;
        public const int OpcodeIndex = 8;
        public const int KcpOpcodeIndex = 0;
        public const int OpcodeLength = 2;
        public const int ActorIdIndex = 0;
        public const int ActorIdLength = 8;
        public const int MessageIndex = 10;

        public ushort Opcode;
        public long ActorId;
        public MemoryStream MemoryStream;
    }
    /// <summary>
    /// Socket客户端帮助类
    /// </summary>
    public sealed class TChannel
    {
        public Guid Id { get; set; }
        private Socket socket;
        private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();

        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();


        private bool isConnected;

        private readonly PacketParser parser;

        private readonly byte[] sendCache = new byte[Packet.OpcodeLength + Packet.ActorIdLength];
        private readonly Dictionary<int, RpcInfo> requestCallbacks = new Dictionary<int, RpcInfo>();

        private void OnComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    OnConnectComplete(e);
                    break;
                case SocketAsyncOperation.Receive:
                    OnRecvComplete(e);
                    break;
                case SocketAsyncOperation.Send:
                    OnSendComplete(e);
                    break;
                case SocketAsyncOperation.Disconnect:
                    OnDisconnectComplete(e);
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }

        #region 网络线程
        public IPEndPoint RemoteAddress { get; set; }
        public TChannel(IPEndPoint ipEndPoint)
        {
            this.Id = Guid.NewGuid();
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = true;
            this.parser = new PacketParser(this.recvBuffer);
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;
            this.RemoteAddress = ipEndPoint;
            this.isConnected = false;
            this.ConnectAsync();
            Task.Run(() =>
            {
                this.StartRecv();
                Thread.Sleep(10);
            });
        }
        public static int RpcId = 0;
        public bool IsDisposed
        {
            get
            {
                return this.Id == Guid.Empty;
            }
        }
        public void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            this.Id = Guid.Empty;
            RoboatServerComponent.cts.Cancel();

            this.innArgs.Dispose();
            this.outArgs.Dispose();

            this.socket.Close();
            this.innArgs = null;
            this.outArgs = null;
            this.socket = null;
        }
        public void Send(object message)
        {
            var property = message.GetType().GetProperty("RpcId");
            int.TryParse(property.GetValue(message, null).ToString(), out int rpc);
            if (rpc < 1)
                property.SetValue(message, RpcId++, null);
            (ushort opcode, MemoryStream stream) = MessageSerializeHelper.MessageToStream(message);
            this.Send(stream);
        }
        public async ELTask<IResponse> Call(IRequest request)
        {
            int rpcId = ++RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            this.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;
            this.Send(request);
            var response = await rpcInfo.Tcs;
            return response;
        }
        public void Send(MemoryStream stream)
        {
            if (this.IsDisposed)
            {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }

            long messageSize = stream.Length - stream.Position;
            this.sendCache.WriteTo(0, messageSize);
            this.sendBuffer.Write(this.sendCache, 0, PacketParser.OuterPacketSizeLength);

            this.sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));

            var msgSize = BitConverter.ToInt32(this.sendBuffer.First.Take(PacketParser.OuterPacketSizeLength).ToArray(), 0);
            Log.Trace($"发送的消息长度({PacketParser.OuterPacketSizeLength})：" + msgSize);
            var t = this.sendBuffer.First.Skip(4).Take(2).ToArray();
            var code = BitConverter.ToUInt16(t, 0);
            Log.Trace($"发送的消息code({Packet.OpcodeLength})：" + code);
            var str = System.Text.Encoding.Default.GetString(this.sendBuffer.First.Skip(6).Take(sendBuffer.First.Length - 6).ToArray());
            Log.Trace("发送的消息内容：" + str);
            this.StartSend();
        }


        private void ConnectAsync()
        {
            this.outArgs.RemoteEndPoint = this.RemoteAddress;
            this.socket.ConnectAsync(RemoteAddress.Address, RemoteAddress.Port);
            OnConnectComplete(this.outArgs);
        }

        private void OnConnectComplete(object o)
        {
            if (this.socket == null)
            {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                this.OnError((int)e.SocketError);
                return;
            }

            e.RemoteEndPoint = null;
            this.isConnected = true;
            this.StartRecv();
            this.StartSend();
        }

        private void OnDisconnectComplete(object o)
        {
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;
            this.OnError((int)e.SocketError);
        }

        private void StartRecv()
        {
            while (true)
            {
                try
                {
                    if (this.socket == null)
                    {
                        return;
                    }

                    int size = this.recvBuffer.ChunkSize - this.recvBuffer.LastIndex;
                    this.innArgs.SetBuffer(this.recvBuffer.Last, this.recvBuffer.LastIndex, size);
                }
                catch (Exception e)
                {
                    return;
                }

                if (this.socket.ReceiveAsync(this.innArgs))
                {
                    return;
                }
                this.HandleRecv(this.innArgs);
            }
        }

        private void OnRecvComplete(object o)
        {
            this.HandleRecv(o);

            if (this.socket == null)
            {
                return;
            }
            this.StartRecv();
        }

        private void HandleRecv(object o)
        {
            if (this.socket == null)
            {
                return;
            }
            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                this.OnError((int)e.SocketError);
                return;
            }
            if (e.BytesTransferred == 0)
            {
                return;
            }

            this.recvBuffer.LastIndex += e.BytesTransferred;
            if (this.recvBuffer.LastIndex == this.recvBuffer.ChunkSize)
            {
                this.recvBuffer.AddLast();
                this.recvBuffer.LastIndex = 0;
            }

            // 收到消息回调
            while (true)
            {
                // 这里循环解析消息执行，有可能，执行消息的过程中断开了session
                if (this.socket == null)
                {
                    return;
                }
                try
                {
                    bool ret = this.parser.Parse();
                    if (!ret)
                    {
                        break;
                    }

                    this.OnRead(this.parser.MemoryStream);
                }
                catch (Exception ee)
                {
                    return;
                }
            }
        }

        private void StartSend()
        {
            if (!this.isConnected)
            {
                return;
            }

            while (true)
            {
                try
                {
                    if (this.socket == null)
                    {
                        return;
                    }

                    // 没有数据需要发送
                    if (this.sendBuffer.Length == 0)
                    {
                        return;
                    }

                    int sendSize = this.sendBuffer.ChunkSize - this.sendBuffer.FirstIndex;
                    if (sendSize > this.sendBuffer.Length)
                    {
                        sendSize = (int)this.sendBuffer.Length;
                    }
                    //ByteToFile(this.sendBuffer.First);
                    //var bytes = FileToByte();
                    this.outArgs.SetBuffer(this.sendBuffer.First, this.sendBuffer.FirstIndex, sendSize);

                    if (this.socket.SendAsync(this.outArgs))
                    {
                        return;
                    }

                    HandleSend(this.outArgs);
                }
                catch (Exception e)
                {
                    throw new Exception($"socket set buffer error: {this.sendBuffer.First.Length}, {this.sendBuffer.FirstIndex}", e);
                }
            }
        }
        public static bool ByteToFile(byte[] byteArray, string fileName = @"D:\Work Space\enterprise.library\EL.Bin\EL.Robot.WPF\Debug\Logs\loginTest.txt")
        {
            bool result = false;
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 将文件转换成byte[]数组
        /// </summary>
        /// <param name="fileUrl">文件路径文件名称</param>
        /// <returns>byte[]数组</returns>
        public static byte[] FileToByte(string fileName = @"D:\Work Space\enterprise.library\EL.Bin\EL.Robot.WPF\Debug\Logs\loginTest.txt")
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    byte[] byteArray = new byte[fs.Length];
                    fs.Read(byteArray, 0, byteArray.Length);
                    return byteArray;
                }
            }
            catch
            {
                return null;
            }
        }
        private void OnSendComplete(object o)
        {
            HandleSend(o);

            this.StartSend();
        }

        private void HandleSend(object o)
        {
            if (this.socket == null)
            {
                return;
            }

            SocketAsyncEventArgs e = (SocketAsyncEventArgs)o;

            if (e.SocketError != SocketError.Success)
            {
                this.OnError((int)e.SocketError);
                return;
            }

            if (e.BytesTransferred == 0)
            {
                return;
            }

            this.sendBuffer.FirstIndex += e.BytesTransferred;
            if (this.sendBuffer.FirstIndex == this.sendBuffer.ChunkSize)
            {
                this.sendBuffer.FirstIndex = 0;
                this.sendBuffer.RemoveFirst();
            }
        }

        private void OnRead(MemoryStream memoryStream)
        {
            ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.KcpOpcodeIndex);
            Type type = OpcodeTypeComponent.GetType(opcode);
            object message = MessageSerializeHelper.DeserializeFrom(opcode, type, memoryStream);
            if (message is IResponse response)
            {
                if (!this.requestCallbacks.TryGetValue(response.RpcId, out var action))
                {
                    return;
                }
                this.requestCallbacks.Remove(response.RpcId);
                action.Tcs.SetResult(response);
                return;
            }
            Handle(opcode, message, this).Coroutine();
        }

        public static async ELVoid Handle(ushort opcode, object message, TChannel channel)
        {
            List<IMHandler> actions;
            try
            {
                if (!MessageHandlerComponent.Handlers.TryGetValue(opcode, out actions))
                {
                    //MessageBox.Show($"消息没有处理: {opcode} {message}");
                    return;
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            foreach (IMHandler ev in actions)
            {
                try
                {
                    ev.Handle(channel, message);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }
        private void OnError(int error)
        {

        }

        #endregion

    }
    public interface IAgentMessage : IMessage
    {
    }

    public interface IAgentRequest : IRequest
    {
    }

    public interface IAgentResponse : IResponse
    {
    }
    public interface IMHandler
    {
        ELVoid Handle(TChannel channel, object message);
        Type GetMessageType();

        Type GetResponseType();
    }
    public abstract class AMHandler<Message> : IMHandler where Message : class
    {
        protected abstract ELTask Run(TChannel channel, Message message);

        public async ELVoid Handle(TChannel channel, object msg)
        {
            await ELTask.CompletedTask;
            try
            {
                Message message = msg as Message;
                this.Run(channel, message).Coroutine();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        public Type GetMessageType()
        {
            return typeof(Message);
        }

        public Type GetResponseType()
        {
            return null;
        }
    }


    [MessageHandler]
    public class G2C_AbandonedHandler : AMHandler<G2C_Abandoned>
    {
        protected override async ELTask Run(TChannel channel, G2C_Abandoned message)
        {
            await ELTask.CompletedTask;
            //MessageBox.Show(message.Reason);
        }
    }
    //[MessageHandler]
    //public class MsgAgentRequestHandler : AMHandler<G2C_MsgAgent>
    //{
    //    protected override void Run(TChannel channel, G2C_MsgAgent message)
    //    {

    //        channel.Send(new C2G_MsgAgent()
    //        {
    //            RpcId = message.RpcId,
    //            Content = "success",
    //            TargetUserId = message.TargetUserId,
    //            SelfUserId = message.SelfUserId
    //        });
    //        //WinSocketTest.WinSocketTest.RecMsg(message.Content);
    //    }
    //}

    public abstract class Object : ISupportInitialize, IDisposable
    {
        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
        }

        public virtual void Dispose()
        {
        }

        public override string ToString()
        {
            return JsonHelper.ToJson(this);
        }
    }
    public interface IMessage
    {
        public int RpcId { get; set; }
    }
    public interface IRequest : IMessage
    {
        int RpcId
        {
            get;
            set;
        }
    }

    public interface IResponse : IMessage
    {
        int Error
        {
            get;
            set;
        }

        string Message
        {
            get;
            set;
        }

        int RpcId
        {
            get;
            set;
        }
    }

    public readonly struct RpcInfo
    {
        public readonly IRequest Request;
        public readonly ELTask<IResponse> Tcs;

        public RpcInfo(IRequest request)
        {
            this.Request = request;
            this.Tcs = ELTask<IResponse>.Create(true);
        }
    }
    public class CircularBuffer : Stream
    {
        public int ChunkSize = 8192;

        private readonly Queue<byte[]> bufferQueue = new Queue<byte[]>();

        private readonly Queue<byte[]> bufferCache = new Queue<byte[]>();

        public int LastIndex { get; set; }

        public int FirstIndex { get; set; }

        private byte[] lastBuffer;

        public CircularBuffer()
        {
            this.AddLast();
        }

        public override long Length
        {
            get
            {
                int c = 0;
                if (this.bufferQueue.Count == 0)
                {
                    c = 0;
                }
                else
                {
                    c = (this.bufferQueue.Count - 1) * ChunkSize + this.LastIndex - this.FirstIndex;
                }
                if (c < 0)
                {
                    //Log.Error("CircularBuffer count < 0: {0}, {1}, {2}".Fmt(this.bufferQueue.Count, this.LastIndex, this.FirstIndex));
                }
                return c;
            }
        }

        public void AddLast()
        {
            byte[] buffer;
            if (this.bufferCache.Count > 0)
            {
                buffer = this.bufferCache.Dequeue();
            }
            else
            {
                buffer = new byte[ChunkSize];
            }
            this.bufferQueue.Enqueue(buffer);
            this.lastBuffer = buffer;
        }

        public void RemoveFirst()
        {
            this.bufferCache.Enqueue(bufferQueue.Dequeue());
        }

        public byte[] First
        {
            get
            {
                if (this.bufferQueue.Count == 0)
                {
                    this.AddLast();
                }
                return this.bufferQueue.Peek();
            }
        }

        public byte[] Last
        {
            get
            {
                if (this.bufferQueue.Count == 0)
                {
                    this.AddLast();
                }
                return this.lastBuffer;
            }
        }

        /// <summary>
        /// 从CircularBuffer读到stream中
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        //public async ETTask ReadAsync(Stream stream)
        //{
        //    long buffLength = this.Length;
        //	int sendSize = this.ChunkSize - this.FirstIndex;
        //    if (sendSize > buffLength)
        //    {
        //	    sendSize = (int)buffLength;
        //    }
        //	
        //    await stream.WriteAsync(this.First, this.FirstIndex, sendSize);
        //    
        //    this.FirstIndex += sendSize;
        //    if (this.FirstIndex == this.ChunkSize)
        //    {
        //	    this.FirstIndex = 0;
        //	    this.RemoveFirst();
        //    }
        //}

        // 从CircularBuffer读到stream
        public void Read(Stream stream, int count)
        {
            if (count > this.Length)
            {
                throw new Exception($"bufferList length < count, {Length} {count}");
            }

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
                if (ChunkSize - this.FirstIndex > n)
                {
                    stream.Write(this.First, this.FirstIndex, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    stream.Write(this.First, this.FirstIndex, ChunkSize - this.FirstIndex);
                    alreadyCopyCount += ChunkSize - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }
        }

        // 从stream写入CircularBuffer
        public void Write(Stream stream)
        {
            int count = (int)(stream.Length - stream.Position);

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                if (this.LastIndex == ChunkSize)
                {
                    this.AddLast();
                    this.LastIndex = 0;
                }

                int n = count - alreadyCopyCount;
                if (ChunkSize - this.LastIndex > n)
                {
                    stream.Read(this.lastBuffer, this.LastIndex, n);
                    this.LastIndex += count - alreadyCopyCount;
                    alreadyCopyCount += n;
                }
                else
                {
                    stream.Read(this.lastBuffer, this.LastIndex, ChunkSize - this.LastIndex);
                    alreadyCopyCount += ChunkSize - this.LastIndex;
                    this.LastIndex = ChunkSize;
                }
            }
        }


        /// <summary>
        ///  从stream写入CircularBuffer
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        //public async ETTask<int> WriteAsync(Stream stream)
        //{
        //    int size = this.ChunkSize - this.LastIndex;
        //    
        //    int n = await stream.ReadAsync(this.Last, this.LastIndex, size);
        //
        //    if (n == 0)
        //    {
        //	    return 0;
        //    }
        //
        //    this.LastIndex += n;
        //
        //    if (this.LastIndex == this.ChunkSize)
        //    {
        //	    this.AddLast();
        //	    this.LastIndex = 0;
        //    }
        //
        //    return n;
        //}

        // 把CircularBuffer中数据写入buffer
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < offset + count)
            {
                throw new Exception($"bufferList length < coutn, buffer length: {buffer.Length} {offset} {count}");
            }

            long length = this.Length;
            if (length < count)
            {
                count = (int)length;
            }

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
                if (ChunkSize - this.FirstIndex > n)
                {
                    Array.Copy(this.First, this.FirstIndex, buffer, alreadyCopyCount + offset, n);
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    Array.Copy(this.First, this.FirstIndex, buffer, alreadyCopyCount + offset, ChunkSize - this.FirstIndex);
                    alreadyCopyCount += ChunkSize - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }

            return count;
        }

        // 把buffer写入CircularBuffer中
        public override void Write(byte[] buffer, int offset, int count)
        {
            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                if (this.LastIndex == ChunkSize)
                {
                    this.AddLast();
                    this.LastIndex = 0;
                }

                int n = count - alreadyCopyCount;
                if (ChunkSize - this.LastIndex > n)
                {
                    Array.Copy(buffer, alreadyCopyCount + offset, this.lastBuffer, this.LastIndex, n);
                    this.LastIndex += count - alreadyCopyCount;
                    alreadyCopyCount += n;
                }
                else
                {
                    Array.Copy(buffer, alreadyCopyCount + offset, this.lastBuffer, this.LastIndex, ChunkSize - this.LastIndex);
                    alreadyCopyCount += ChunkSize - this.LastIndex;
                    this.LastIndex = ChunkSize;
                }
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Position { get; set; }
    }
    public enum ParserState
    {
        PacketSize,
        PacketBody
    }
    public class PacketParser
    {
        private readonly CircularBuffer buffer;
        private int packetSize;
        private ParserState state;
        private readonly byte[] cache = new byte[8];
        public const int InnerPacketSizeLength = 4;
        public const int OuterPacketSizeLength = 4;
        public MemoryStream MemoryStream;

        public PacketParser(CircularBuffer buffer)
        {
            this.buffer = buffer;
        }

        public bool Parse()
        {
            while (true)
            {
                switch (this.state)
                {
                    case ParserState.PacketSize:
                        {
                            if (this.buffer.Length < OuterPacketSizeLength)
                            {
                                return false;
                            }

                            this.buffer.Read(this.cache, 0, OuterPacketSizeLength);

                            this.packetSize = BitConverter.ToInt32(this.cache, 0);
                            if (this.packetSize < Packet.MinPacketSize)
                            {
                                throw new Exception($"recv packet size error, 可能是外网探测端口: {this.packetSize}");
                            }

                            this.state = ParserState.PacketBody;
                            break;
                        }


                    case ParserState.PacketBody:
                        {
                            if (this.buffer.Length < this.packetSize)
                            {
                                return false;
                            }

                            MemoryStream memoryStream = MessageSerializeHelper.GetStream(this.packetSize);
                            this.buffer.Read(memoryStream, this.packetSize);
                            //memoryStream.SetLength(this.packetSize - Packet.MessageIndex);
                            this.MemoryStream = memoryStream;
                            memoryStream.Seek(Packet.OpcodeLength, SeekOrigin.Begin);

                            this.state = ParserState.PacketSize;
                            return true;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
    public static class MessageSerializeHelper
    {
        public const ushort PbMaxOpcode = 40000;

        public const ushort JsonMinOpcode = 51000;

        public static object DeserializeFrom(ushort opcode, Type type, MemoryStream memoryStream)
        {


            if (opcode >= JsonMinOpcode)
            {
                var json = memoryStream.GetBuffer().Utf8ToStr((int)memoryStream.Position, (int)(memoryStream.Length - memoryStream.Position));
                return JsonHelper.FromJson(type, json);
            }
            return null;

        }
        public static object DeserializeFrom(ushort opcode, Type type, string json)
        {
            if (opcode >= JsonMinOpcode)
            {
                return JsonHelper.FromJson(type, json);
            }
            return null;

        }
        public static byte[] ToUtf8(this string str)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            return byteArray;
        }
        public static void SerializeTo(ushort opcode, object obj, MemoryStream memoryStream)
        {

            if (opcode >= JsonMinOpcode)
            {
                string s = JsonHelper.ToJson(obj);
                byte[] bytes = s.ToUtf8();
                memoryStream.Write(bytes, 0, bytes.Length);
                return;
            }

        }

        public static MemoryStream GetStream(int count = 0)
        {
            MemoryStream stream;
            if (count > 0)
            {
                stream = new MemoryStream(count);
            }
            else
            {
                stream = new MemoryStream();
            }

            return stream;
        }

        public static (ushort, MemoryStream) MessageToStream(object message, int count = 0)
        {
            MemoryStream stream = GetStream(Packet.OpcodeLength + count);

            ushort opcode = OpcodeTypeComponent.GetOpcode(message.GetType());

            stream.Seek(Packet.OpcodeLength, SeekOrigin.Begin);
            stream.SetLength(Packet.OpcodeLength);

            stream.GetBuffer().WriteTo(0, opcode);

            MessageSerializeHelper.SerializeTo(opcode, message, stream);

            stream.Seek(0, SeekOrigin.Begin);
            return (opcode, stream);
        }


        public static (ushort, MemoryStream) MessageToStream(long actorId, object message, int count = 0)
        {
            int actorSize = sizeof(long);
            MemoryStream stream = GetStream(actorSize + Packet.OpcodeLength + count);

            ushort opcode = OpcodeTypeComponent.GetOpcode(message.GetType());

            stream.Seek(actorSize + Packet.OpcodeLength, SeekOrigin.Begin);
            stream.SetLength(actorSize + Packet.OpcodeLength);

            // 写入actorId
            stream.GetBuffer().WriteTo(0, actorId);
            stream.GetBuffer().WriteTo(actorSize, opcode);

            MessageSerializeHelper.SerializeTo(opcode, message, stream);

            stream.Seek(0, SeekOrigin.Begin);
            return (opcode, stream);
        }
    }
    public static class JsonHelper
    {
        private static readonly JsonWriterSettings logDefineSettings = new JsonWriterSettings() { OutputMode = JsonOutputMode.RelaxedExtendedJson };

        public static string ToJson(object message)
        {
            return MongoDB.Bson.BsonExtensionMethods.ToJson(message, logDefineSettings);

        }

        public static object FromJson(Type type, string json)
        {
            return MongoDB.Bson.Serialization.BsonSerializer.Deserialize(json, type);


        }

        public static T FromJson<T>(string json)
        {
            return MongoDB.Bson.Serialization.BsonSerializer.Deserialize<T>(json);

        }
    }
    public static class ByteHelper
    {
        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        public static string ToHex(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, string format)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString(format));
            }
            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, int offset, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = offset; i < offset + count; ++i)
            {
                stringBuilder.Append(bytes[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string ToStr(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static string ToStr(this byte[] bytes, int index, int count)
        {
            return Encoding.Default.GetString(bytes, index, count);
        }

        public static string Utf8ToStr(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Utf8ToStr(this byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        public static void WriteTo(this byte[] bytes, int offset, uint num)
        {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
            bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
            bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
        }

        public static void WriteTo(this byte[] bytes, int offset, int num)
        {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
            bytes[offset + 2] = (byte)((num & 0xff0000) >> 16);
            bytes[offset + 3] = (byte)((num & 0xff000000) >> 24);
        }

        public static void WriteTo(this byte[] bytes, int offset, byte num)
        {
            bytes[offset] = num;
        }

        public static void WriteTo(this byte[] bytes, int offset, short num)
        {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
        }

        public static void WriteTo(this byte[] bytes, int offset, ushort num)
        {
            bytes[offset] = (byte)(num & 0xff);
            bytes[offset + 1] = (byte)((num & 0xff00) >> 8);
        }

        public static unsafe void WriteTo(this byte[] bytes, int offset, long num)
        {
            byte* bPoint = (byte*)&num;
            for (int i = 0; i < sizeof(long); ++i)
            {
                bytes[offset + i] = bPoint[i];
            }
        }
    }
    public class MessageAttribute : Attribute
    {
        public ushort Opcode
        {
            get;
        }

        public MessageAttribute(ushort opcode)
        {
            this.Opcode = opcode;
        }
    }
    public class OpcodeTypeComponent
    {
        private readonly static Dictionary<ushort, Type> opcodeTypes = new Dictionary<ushort, Type>();
        private readonly static Dictionary<Type, ushort> typeOpcodes = new Dictionary<Type, ushort>();
        static OpcodeTypeComponent()
        {
            var types = typeof(OpcodeTypeComponent).Assembly.GetTypes().Where(x => x.GetCustomAttribute<MessageAttribute>() != null);
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(MessageAttribute), false);

                MessageAttribute messageAttribute = attrs[0] as MessageAttribute;
                if (messageAttribute == null)
                {
                    continue;
                }
                opcodeTypes.Add(messageAttribute.Opcode, type);
                typeOpcodes.Add(type, messageAttribute.Opcode);
            }

        }
        public static ushort GetOpcode(Type type)
        {
            return typeOpcodes[type];
        }

        public static Type GetType(ushort opcode)
        {
            return opcodeTypes[opcode];
        }
    }
    public class MessageHandlerAttribute : Attribute
    {
    }
    public class MessageHandlerComponent
    {
        public static readonly Dictionary<ushort, List<IMHandler>> Handlers = new Dictionary<ushort, List<IMHandler>>();
        public static readonly Dictionary<ushort, List<W_IMHandler>> W_Handlers = new Dictionary<ushort, List<W_IMHandler>>();

        static MessageHandlerComponent()
        {
            var types = typeof(OpcodeTypeComponent).Assembly.GetTypes().Where(x => x.GetCustomAttribute<MessageHandlerAttribute>() != null);
            foreach (Type type in types)
            {
                IMHandler iMHandler = Activator.CreateInstance(type) as IMHandler;
                if (iMHandler == null)
                {
                    continue;
                }

                Type messageType = iMHandler.GetMessageType();
                ushort opcode = OpcodeTypeComponent.GetOpcode(messageType);
                if (!Handlers.ContainsKey(opcode))
                {
                    Handlers.Add(opcode, new List<IMHandler>());
                }
                Handlers[opcode].Add(iMHandler);
            }
            foreach (Type type in types)
            {
                W_IMHandler iMHandler = Activator.CreateInstance(type) as W_IMHandler;
                if (iMHandler == null)
                {
                    continue;
                }

                Type messageType = iMHandler.GetMessageType();
                ushort opcode = OpcodeTypeComponent.GetOpcode(messageType);
                if (!W_Handlers.ContainsKey(opcode))
                {
                    W_Handlers.Add(opcode, new List<W_IMHandler>());
                }
                W_Handlers[opcode].Add(iMHandler);
            }

        }
    }

}
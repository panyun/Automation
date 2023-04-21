using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ET
{
    /// <summary>
    /// 封装Socket,将回调push到主线程处理
    /// </summary>
    public sealed class TChannel : AChannel
    {
        private readonly TService Service;
        private Socket socket;
        private SocketAsyncEventArgs innArgs = new SocketAsyncEventArgs();
        private SocketAsyncEventArgs outArgs = new SocketAsyncEventArgs();

        private readonly CircularBuffer recvBuffer = new CircularBuffer();
        private readonly CircularBuffer sendBuffer = new CircularBuffer();

        private bool isSending;

        private bool isConnected;
#pragma warning disable IDE0052 // 删除未读的私有成员
        public bool IsWebSocket = false;
#pragma warning restore IDE0052 // 删除未读的私有成员

        private readonly PacketParser parser;

        private readonly byte[] sendCache = new byte[Packet.OpcodeLength + Packet.ActorIdLength];

        private void OnComplete(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    this.Service.ThreadSynchronizationContext.Post(() => OnConnectComplete(e));
                    break;
                case SocketAsyncOperation.Receive:
                    this.Service.ThreadSynchronizationContext.Post(() => OnRecvComplete(e));
                    break;
                case SocketAsyncOperation.Send:
                    this.Service.ThreadSynchronizationContext.Post(() => OnSendComplete(e));
                    break;
                case SocketAsyncOperation.Disconnect:
                    this.Service.ThreadSynchronizationContext.Post(() => OnDisconnectComplete(e));
                    break;
                default:
                    throw new Exception($"socket error: {e.LastOperation}");
            }
        }

        #region 网络线程

        public TChannel(long id, IPEndPoint ipEndPoint, TService service)
        {
            this.ChannelType = ChannelType.Connect;
            this.Id = id;
            this.Service = service;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.socket.NoDelay = true;
            this.parser = new PacketParser(this.recvBuffer, this.Service);
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;

            this.RemoteAddress = ipEndPoint;
            this.isConnected = false;
            this.isSending = false;

            this.Service.ThreadSynchronizationContext.PostNext(this.ConnectAsync);
        }

        public TChannel(long id, Socket socket, TService service)
        {
            this.ChannelType = ChannelType.Accept;
            this.Id = id;
            this.Service = service;
            this.socket = socket;
            this.socket.NoDelay = true;
            this.parser = new PacketParser(this.recvBuffer, this.Service);
            this.innArgs.Completed += this.OnComplete;
            this.outArgs.Completed += this.OnComplete;

            this.RemoteAddress = (IPEndPoint)socket.RemoteEndPoint;
            this.isConnected = true;
            this.isSending = false;

            // 下一帧再开始读写
            this.Service.ThreadSynchronizationContext.PostNext(() =>
            {
                this.StartRecv();
                this.StartSend();
            });
        }



        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            Log.Info($"channel dispose: {this.Id} {this.RemoteAddress}");

            long id = this.Id;
            this.Id = 0;
            this.Service.Remove(id);
            this.socket.Close();
            this.innArgs.Dispose();
            this.outArgs.Dispose();
            this.innArgs = null;
            this.outArgs = null;
            this.socket = null;
        }

        public void Send(long actorId, MemoryStream stream)
        {
            if (this.IsDisposed)
            {
                throw new Exception("TChannel已经被Dispose, 不能发送消息");
            }
            if (IsWebSocket)
            {
                var opcode = BitConverter.ToUInt16(stream.GetBuffer(), Packet.KcpOpcodeIndex);
                var type = OpcodeTypeComponent.Instance.GetType(opcode);
                var msg = stream.GetBuffer().ToStr(2, (int)(stream.Length - 2));
                var sendMsg = opcode + msg;
                var sendBytes = PackData(sendMsg);
                this.sendBuffer.Write(sendBytes, 0, sendBytes.Length);
                if (!this.isSending)
                {
                    //this.StartSend();
                    this.Service.NeedStartSend.Add(this.Id);
                }
                return;
            }
            switch (this.Service.ServiceType)
            {
                case ServiceType.Inner:
                    {
                        int messageSize = (int)(stream.Length - stream.Position);
                        if (messageSize > ushort.MaxValue * 16)
                        {
                            throw new Exception($"send packet too large: {stream.Length} {stream.Position}");
                        }

                        this.sendCache.WriteTo(0, messageSize);
                        this.sendBuffer.Write(this.sendCache, 0, PacketParser.InnerPacketSizeLength);

                        // actorId
                        stream.GetBuffer().WriteTo(0, actorId);
                        this.sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));
                        break;
                    }
                case ServiceType.Outer:
                    {
                        long messageSize = (int)(stream.Length - stream.Position);

                        this.sendCache.WriteTo(0, messageSize);
                        this.sendBuffer.Write(this.sendCache, 0, PacketParser.OuterPacketSizeLength);

                        this.sendBuffer.Write(stream.GetBuffer(), (int)stream.Position, (int)(stream.Length - stream.Position));

                        break;
                    }
            }


            if (!this.isSending)
            {
                //this.StartSend();
                this.Service.NeedStartSend.Add(this.Id);
            }
        }

        private void ConnectAsync()
        {
            this.outArgs.RemoteEndPoint = this.RemoteAddress;
            if (this.socket.ConnectAsync(this.outArgs))
            {
                return;
            }
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
        byte[] tempbytes = new byte[65535];
        //ArraySegment<byte> bufferTemps = new ArraySegment<byte>();
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
                    if (IsWebSocket)
                    {
                        this.innArgs.SetBuffer(tempbytes, 0, tempbytes.Length);
                        if (this.socket.ReceiveAsync(this.innArgs))
                        {
                            return;
                        }
                        this.HandleRecv(this.innArgs);
                        continue;
                    }
                    int size = this.recvBuffer.ChunkSize - this.recvBuffer.LastIndex;
                    this.innArgs.SetBuffer(this.recvBuffer.Last, this.recvBuffer.LastIndex, size);
                }
                catch (Exception e)
                {
                    Log.Error($"tchannel error: {this.Id}\n{e}");
                    this.OnError(ErrorCode.ERR_TChannelRecvError);
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
        public static Stopwatch sw = new Stopwatch();
        List<byte> SumPageContent = new List<byte>();
        public static int index = 0;
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
                this.OnError(ErrorCode.ERR_PeerDisconnect);
                return;
            }
            if (IsWebSocket)
            {
                // % x0：表示一个延续帧。当Opcode为0时，表示本次数据传输采用了数据分片，当前收到的数据帧为其中一个数据分片。
                //% x1：表示这是一个文本帧（frame）
                //% x2：表示这是一个二进制帧（frame）
                //% x3 - 7：保留的操作代码，用于后续定义的非控制帧。
                //% x8：表示连接断开。
                //% x9：表示这是一个ping操作。
                //% xA：表示这是一个pong操作。
                //% xB - F：保留的操作代码，用于后续定义的控制帧
                var opcode = tempbytes[0] & 0x0f;
                //pong 客户端pong不做日记记录
                if (opcode == 0xA)
                {
                    Log.Info($"收到pong消息");
                    return;
                }
                //
                if (opcode == 0x8)
                {
                    this.OnError(ErrorCode.ERR_PeerDisconnect);
                    return;
                }
                byte[] byteMsg = default;
                try
                {
                    byteMsg = AnalyticData(tempbytes, e.BytesTransferred);
                }
                catch (Exception ex)
                {
                    Log.Error(ex);
                }
                var memoryStream = MessageSerializeHelper.GetStream(byteMsg.Length);
                memoryStream.Write(byteMsg, 0, byteMsg.Length);
                this.OnRead(memoryStream);
                return;
            }
            this.recvBuffer.LastIndex += e.BytesTransferred;
            if (this.recvBuffer.LastIndex == this.recvBuffer.ChunkSize)
            {
                this.recvBuffer.AddLast();
                this.recvBuffer.LastIndex = 0;
            }
            var msg = Encoding.Default.GetString(this.recvBuffer.First);
            //  websocket建立连接的时候，除了TCP连接的三次握手，websocket协议中客户端与服务器想建立连接需要一次额外的握手动作
            if (msg.Contains("Sec-WebSocket-Key") && !IsWebSocket)
            {
                this.socket.Send(PackHandShakeData(GetSecKeyAccetp(this.recvBuffer.First, this.recvBuffer.First.Length)));
                IsWebSocket = true;
                MemoryStream memoryStream = MessageSerializeHelper.GetStream((int)this.recvBuffer.Length);
                recvBuffer.Read(memoryStream, (int)this.recvBuffer.Length);
                return;
            }
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
                    Log.Error($"ip: {this.RemoteAddress} {ee}");
                    this.OnError(ErrorCode.ERR_SocketError);
                    return;
                }
            }
        }
        #region 打包请求连接数据
        /// <summary>
        /// 打包握手信息
        /// </summary>
        /// <param name="secKeyAccept"></param>
        /// <returns></returns>
        private static byte[] PackHandShakeData(string secKeyAccept)
        {
            var responseBuilder = new StringBuilder();
            responseBuilder.Append("HTTP/1.1 101 Switching Protocols" + Environment.NewLine);
            responseBuilder.Append("Upgrade: websocket" + Environment.NewLine);
            responseBuilder.Append("Connection: Upgrade" + Environment.NewLine);
            responseBuilder.Append("Sec-WebSocket-Accept: " + secKeyAccept + Environment.NewLine + Environment.NewLine);
            //如果把上一行换成下面两行，才是thewebsocketprotocol-17协议，但居然握手不成功，目前仍没弄明白！
            //responseBuilder.Append("Sec-WebSocket-Accept: " + secKeyAccept + Environment.NewLine);
            //responseBuilder.Append("Sec-WebSocket-Protocol: chat" + Environment.NewLine);

            return Encoding.UTF8.GetBytes(responseBuilder.ToString());
        }

        /// <summary>
        /// 生成Sec-WebSocket-Accept
        /// </summary>
        /// <param name="handShakeText">客户端握手信息</param>
        /// <returns>Sec-WebSocket-Accept</returns>
        private static string GetSecKeyAccetp(byte[] handShakeBytes, int bytesLength)
        {
            string handShakeText = Encoding.UTF8.GetString(handShakeBytes, 0, bytesLength);
            string key = string.Empty;
            Regex r = new Regex(@"Sec\-WebSocket\-Key:(.*?)\r\n");
            Match m = r.Match(handShakeText);
            if (m.Groups.Count != 0)
            {
                key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)\r\n", "$1").Trim();
            }
            byte[] encryptionString = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            return Convert.ToBase64String(encryptionString);
        }

        #endregion
        #region 处理接收的数据
        private static byte[] masks = new byte[4];
        private static byte[] payload_data;
        /// <summary>
        /// 解析客户端数据包
        /// </summary>
        /// <param name="recBytes">服务器接收的数据包</param>
        /// <param name="recByteLength">有效数据长度</param>
        /// <returns></returns>
        private byte[] AnalyticData(byte[] tempBytes, int recByteLength)
        {
            byte[] recBytes = new byte[recByteLength];
            Array.Copy(tempBytes, 0, recBytes, 0, recByteLength);
            Array.Clear(masks, 0, masks.Length);
            if (payload_data != default)
                Array.Clear(payload_data, 0, payload_data.Length);

            if (recByteLength < 2)
            {
                return default;
            }
            bool fin = (recBytes[0] & 0x80) == 0x80; // 1bit，1表示最后一帧  
            if (!fin)
            {
                return default;// 超过一帧暂不处理 
            }

            bool mask_flag = (recBytes[1] & 0x80) == 0x80; // 是否包含掩码  
            if (!mask_flag)
            {
                return default;// 不包含掩码的暂不处理
            }
            int payload_len = recBytes[1] & 0x7F; // 数据长度  
            if (payload_len == 126)
            {
                Array.Copy(recBytes, 4, masks, 0, 4);
                payload_len = (int)(recBytes[2] << 8 | recBytes[3]);
                payload_data = new byte[payload_len];
                Array.Copy(recBytes, 8, payload_data, 0, payload_len);

            }
            else if (payload_len == 127)
            {
                Array.Copy(recBytes, 10, masks, 0, 4);
                byte[] uInt64Bytes = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    uInt64Bytes[i] = recBytes[9 - i];
                }
                UInt64 len = BitConverter.ToUInt64(uInt64Bytes, 0);

                payload_data = new byte[payload_len];
                for (UInt64 i = 0; i < len; i++)
                {
                    payload_data[i] = recBytes[i + 14];
                }
            }
            else
            {
                if (recBytes.Length < 7)
                {
                    Log.Debug($"rec:{System.Text.Encoding.UTF8.GetString(recBytes)}");
                    return default;
                }
                Array.Copy(recBytes, 2, masks, 0, 4);
                payload_data = new byte[payload_len];
                if (recBytes.Length < payload_len + 6)
                {
                    Log.Debug($"rec:{System.Text.Encoding.UTF8.GetString(recBytes)}");
                    return default;
                }
                Array.Copy(recBytes, 6, payload_data, 0, payload_len);
            }

            for (var i = 0; i < payload_len; i++)
            {
                payload_data[i] = (byte)(payload_data[i] ^ masks[i % 4]);
            }
            return payload_data;
        }

        #endregion
        public void Update()
        {
            this.StartSend();
        }
        private void StartSendWesocket(byte[] bytes)
        {

            if (this.socket == null)
            {
                return;
            }

            SocketAsyncEventArgs e = this.outArgs;

            if (e.SocketError != SocketError.Success)
            {
                this.OnError((int)e.SocketError);
                return;
            }


            this.outArgs.SetBuffer(bytes, 0, bytes.Length);
            try
            {
                if (this.socket.SendAsync(this.outArgs))
                {
                    return;
                }
            }
            catch (Exception)
            {

                throw;
            }
            if (e.BytesTransferred == 0)
            {
                this.OnError(ErrorCode.ERR_PeerDisconnect);
                return;
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
                        this.isSending = false;
                        return;
                    }

                    this.isSending = true;

                    int sendSize = this.sendBuffer.ChunkSize - this.sendBuffer.FirstIndex;
                    if (sendSize > this.sendBuffer.Length)
                    {
                        sendSize = (int)this.sendBuffer.Length;
                    }

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
        /// <summary>
        /// WebSocket Send 发送数据到客户端,打包服务器数据
        /// </summary>
        /// <param name="bytes">要发送的数据</param>
        /// <param name="sendMax">每次发送的最大数据包</param>

        public void PackDataSend(string msg, int sendMax = 65536)
        {
            Byte[] bytes = Encoding.UTF8.GetBytes(msg);
            bool canSend = true;
            //每次最大发送 64Kb的数据
            int SendMax = sendMax;

            int num = 0;
            //已经发送的字节数据
            int taked = 0;
            while (canSend)
            {
                //内容数据
                byte[] contentBytes = null;
                var sendArr = bytes.Skip(num * SendMax).Take(SendMax).ToArray();
                taked += sendArr.Length;
                if (sendArr.Length > 0)
                {
                    //是否可以继续发送
                    canSend = bytes.Length > taked;
                    if (sendArr.Length < 126)
                    {
                        #region 一次发送小于126的数据
                        contentBytes = new byte[sendArr.Length + 2];
                        contentBytes[0] = (byte)(num == 0 ? 0x81 : (!canSend ? 0x80 : 0));
                        contentBytes[1] = (byte)sendArr.Length;
                        Array.Copy(sendArr, 0, contentBytes, 2, sendArr.Length);
                        canSend = false;
                        #endregion

                    }
                    else if (sendArr.Length < 0xFFFF)
                    {
                        #region 发送小于65535的数据
                        contentBytes = new byte[sendArr.Length + 4];
                        //首次不分片发送,大于128字节的数据一次发完
                        if (!canSend && num == 0)
                        {
                            contentBytes[0] = 0x81;
                        }
                        else
                        {
                            //一个分片的消息由起始帧（FIN为0，opcode非0），若干（0个或多个）帧（FIN为0，opcode为0），结束帧（FIN为1，opcode为0）。
                            contentBytes[0] = (byte)(num == 0 ? 0x01 : (!canSend ? 0x80 : 0));
                        }
                        contentBytes[1] = 126;
                        byte[] ushortlen = BitConverter.GetBytes((short)sendArr.Length);
                        contentBytes[2] = ushortlen[1];
                        contentBytes[3] = ushortlen[0];
                        Array.Copy(sendArr, 0, contentBytes, 4, sendArr.Length);
                        #endregion
                    }
                    else if (sendArr.LongLength < long.MaxValue)
                    {
                        #region 一次发送所有数据
                        //long数据一次发完
                        contentBytes = new byte[sendArr.Length + 10];
                        //首次不分片发送,大于128字节的数据一次发完
                        if (!canSend && num == 0)
                        {
                            contentBytes[0] = 0x81;
                        }
                        else
                        {
                            //一个分片的消息由起始帧（FIN为0，opcode非0），若干（0个或多个）帧（FIN为0，opcode为0），结束帧（FIN为1，opcode为0）。
                            contentBytes[0] = (byte)(num == 0 ? 0x01 : (!canSend ? 0x80 : 0));
                        }
                        contentBytes[1] = 127;
                        byte[] ulonglen = BitConverter.GetBytes((long)sendArr.Length);
                        contentBytes[2] = ulonglen[7];
                        contentBytes[3] = ulonglen[6];
                        contentBytes[4] = ulonglen[5];
                        contentBytes[5] = ulonglen[4];
                        contentBytes[6] = ulonglen[3];
                        contentBytes[7] = ulonglen[2];
                        contentBytes[8] = ulonglen[1];
                        contentBytes[9] = ulonglen[0];

                        Array.Copy(sendArr, 0, contentBytes, 10, sendArr.Length);
                        #endregion
                    }
                }
                try
                {
                    if (contentBytes != null)
                    {
                        StartSendWesocket(contentBytes);
                    }
                }
                catch (Exception)
                {
                    //this.OnError(ErrorCode.ERR_WebsocketSendError);
                    break;
                }
                finally
                {
                    //Thread.Sleep(100);
                    num++;
                }

            }
        }

        /// <summary>
        /// 打包服务器数据
        /// </summary>
        /// <param name="message">数据</param>
        /// <returns>数据包</returns>
        /// <summary>
        /// 打包服务器数据
        /// </summary>
        /// <param name="message">数据</param>
        /// <returns>数据包</returns>
        private static byte[] PackData(string message)
        {
            byte[] contentBytes = null;
            byte[] temp = Encoding.UTF8.GetBytes(message);

            if (temp.Length < 126)
            {
                contentBytes = new byte[temp.Length + 2];
                contentBytes[0] = 0x81;
                contentBytes[1] = (byte)temp.Length;
                Array.Copy(temp, 0, contentBytes, 2, temp.Length);
            }
            else if (temp.Length < 0xFFFF)
            {
                contentBytes = new byte[temp.Length + 4];
                contentBytes[0] = 0x81;
                contentBytes[1] = (byte)126;
                contentBytes[2] = (byte)(temp.Length >> 8 & 0xFF);
                contentBytes[3] = (byte)(temp.Length & 0xFF);
                Array.Copy(temp, 0, contentBytes, 4, temp.Length);
            }
            else if (temp.Length > 65535)
            {
                contentBytes = new byte[temp.Length + 10];
                contentBytes[0] = 0x81;
                contentBytes[1] = (byte)127;
                contentBytes[2] = (byte)((temp.Length >> 56) & 0xFF);
                contentBytes[3] = (byte)((temp.Length >> 48) & 0xFF);
                contentBytes[4] = (byte)((temp.Length >> 40) & 0xFF);
                contentBytes[5] = (byte)((temp.Length >> 32) & 0xFF);
                contentBytes[6] = (byte)((temp.Length >> 24) & 0xFF);
                contentBytes[7] = (byte)((temp.Length >> 16) & 0xFF);
                contentBytes[8] = (byte)((temp.Length >> 8) & 0xFF);
                contentBytes[9] = (byte)(temp.Length & 0xFF);
                Array.Copy(temp, 0, contentBytes, 10, temp.Length);
            }
            return contentBytes;
        }
        // from WebSocketFrameWriter class
        //public void Write(WebSocketOpCode opCode, byte[] payload, bool isLastFrame)
        //{
        //    // best to write everything to a memory stream before we push it onto the wire
        //    // not really necessary but I like it this way
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        byte finBitSetAsByte = isLastFrame ? (byte)0x80 : (byte)0x00;
        //        byte byte1 = (byte)(finBitSetAsByte | (byte)opCode);
        //        memoryStream.WriteByte(byte1);

        //        // NB, set the mask flag if we are constructing a client frame
        //        byte maskBitSetAsByte = _isClient ? (byte)0x80 : (byte)0x00;

        //        // depending on the size of the length we want to write it as a byte, ushort or ulong
        //        if (payload.Length < 126)
        //        {
        //            byte byte2 = (byte)(maskBitSetAsByte | (byte)payload.Length);
        //            memoryStream.WriteByte(byte2);
        //        }
        //        else if (payload.Length <= ushort.MaxValue)
        //        {
        //            byte byte2 = (byte)(maskBitSetAsByte | 126);
        //            memoryStream.WriteByte(byte2);
        //            BinaryReaderWriter.WriteUShort((ushort)payload.Length, memoryStream, false);
        //        }
        //        else
        //        {
        //            byte byte2 = (byte)(maskBitSetAsByte | 127);
        //            memoryStream.WriteByte(byte2);
        //            BinaryReaderWriter.WriteULong((ulong)payload.Length, memoryStream, false);
        //        }

        //        // if we are creating a client frame then we MUST mack the payload as per the spec
        //        if (_isClient)
        //        {
        //            byte[] maskKey = new byte[WebSocketFrameCommon.MaskKeyLength];
        //            _random.NextBytes(maskKey);
        //            memoryStream.Write(maskKey, 0, maskKey.Length);

        //            // mask the payload
        //            WebSocketFrameCommon.ToggleMask(maskKey, payload);
        //        }

        //        memoryStream.Write(payload, 0, payload.Length);
        //        byte[] buffer = memoryStream.ToArray();
        //        _stream.Write(buffer, 0, buffer.Length);
        //    }
        //}

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
                this.OnError(ErrorCode.ERR_PeerDisconnect);
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
            try
            {
                long channelId = this.Id;
                this.Service.OnRead(channelId, memoryStream);
            }
            catch (Exception e)
            {
                Log.Error($"{this.RemoteAddress} {memoryStream.Length} {e}");
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                this.OnError(ErrorCode.ERR_PacketParserError);
            }
        }

        private void OnError(int error)
        {
            Log.Info($"TChannel OnError: {error} {this.RemoteAddress}");

            long channelId = this.Id;

            this.Service.Remove(channelId);

            this.Service.OnError(channelId, error);
        }

        #endregion

    }
}
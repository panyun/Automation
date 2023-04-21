﻿using EL.Async;
using EL.Net.Network.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EL.Basic.Network.Message
{
    public static class SceneHelper
    {
        public static int DomainZone(this Entity entity)
        {
            return ((Scene)entity.Domain)?.Zone ?? 0;
        }

        public static Scene DomainScene(this Entity entity)
        {
            return (Scene)entity.Domain;
        }
    }
    public sealed class Session : Entity
    {
        private readonly struct RpcInfo
        {
            public readonly IRequest Request;
            public readonly ELTask<IResponse> Tcs;

            public RpcInfo(IRequest request)
            {
                this.Request = request;
                this.Tcs = ELTask<IResponse>.Create(true);
            }
        }

        public AService AService;

        private static int RpcId
        {
            get;
            set;
        }

        private readonly Dictionary<int, RpcInfo> requestCallbacks = new Dictionary<int, RpcInfo>();

        public long LastRecvTime
        {
            get;
            set;
        }

        public long LastSendTime
        {
            get;
            set;
        }

        public int Error
        {
            get;
            set;
        }

        public void Awake(AService aService)
        {
            this.AService = aService;
            long timeNow = TimeHelper.ClientNow();
            this.LastRecvTime = timeNow;
            this.LastSendTime = timeNow;

            this.requestCallbacks.Clear();

            //Log.Info($"session create: zone: {this.DomainZone()} id: {this.Id} {timeNow} ");
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            int zone = this.DomainZone();
            long id = this.Id;

            base.Dispose();

            this.AService.RemoveChannel(this.Id);

            foreach (RpcInfo responseCallback in this.requestCallbacks.Values.ToArray())
            {
                responseCallback.Tcs.SetException(new RpcException(this.Error, $"session dispose: {id} {this.RemoteAddress}"));
            }

            Log.Info($"session dispose: {this.RemoteAddress} zone: {zone} id: {id} ErrorCode: {this.Error}, please see ErrorCode.cs! {TimeHelper.ClientNow()}");

            this.requestCallbacks.Clear();
        }

        public IPEndPoint RemoteAddress
        {
            get;
            set;
        }

        public void OnRead(ushort opcode, IResponse response)
        {
            OpcodeHelper.LogMsg(0, opcode, response);

            if (!this.requestCallbacks.TryGetValue(response.RpcId, out var action))
            {
                return;
            }

            this.requestCallbacks.Remove(response.RpcId);
            if (ErrorCode.IsRpcNeedThrowException(response.Error))
            {
                action.Tcs.SetException(new Exception($"Rpc error, request: {action.Request} response: {response}"));
                return;
            }
            action.Tcs.SetResult(response);
        }

        public async ELTask<IResponse> Call(IRequest request, ELCancellationToken cancellationToken)
        {
            int rpcId = ++RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            this.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;

            this.Send(request);

            void CancelAction()
            {
                if (!this.requestCallbacks.TryGetValue(rpcId, out RpcInfo action))
                {
                    return;
                }

                this.requestCallbacks.Remove(rpcId);
                Type responseType = OpcodeTypeComponent.Instance.GetResponseType(action.Request.GetType());
                IResponse response = (IResponse)Activator.CreateInstance(responseType);
                response.Error = ErrorCode.ERR_Cancel;
                action.Tcs.SetResult(response);
            }

            IResponse ret;
            try
            {
                cancellationToken?.Add(CancelAction);
                ret = await rpcInfo.Tcs;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
            return ret;
        }

        public async ELTask<IResponse> Call(IRequest request)
        {
            int rpcId = ++RpcId;
            RpcInfo rpcInfo = new RpcInfo(request);
            this.requestCallbacks[rpcId] = rpcInfo;
            request.RpcId = rpcId;
            this.Send(request);
            return await rpcInfo.Tcs;
        }

        public void Reply(IResponse message)
        {
            this.Send(message);
        }

        public void Send(IMessage message)
        {
            switch (this.AService.ServiceType)
            {
                case ServiceType.Inner:
                    {
                        (ushort opcode, MemoryStream stream) = MessageSerializeHelper.MessageToStream(0, message);
                        OpcodeHelper.LogMsg(this.DomainZone(), opcode, message);
                        this.Send(0, stream);
                        break;
                    }
                case ServiceType.Outer:
                    {
                        (ushort opcode, MemoryStream stream) = MessageSerializeHelper.MessageToStream(message);
                        OpcodeHelper.LogMsg(this.DomainZone(), opcode, message);
                        this.Send(0, stream);
                        break;
                    }
            }
        }

        public void Send(long actorId, IMessage message)
        {
            (ushort opcode, MemoryStream stream) = MessageSerializeHelper.MessageToStream(actorId, message);
            OpcodeHelper.LogMsg(this.DomainZone(), opcode, message);
            this.Send(actorId, stream);
        }

        public void Send(long actorId, MemoryStream memoryStream)
        {
            this.LastSendTime = TimeHelper.ClientNow();
            this.AService.SendStream(this.Id, actorId, memoryStream);
        }
    }
}

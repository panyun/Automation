using EL.Basic.Network;
//using ProtoBuf;

namespace EL.Net.Network.Message
{

    [Message(ushort.MaxValue)]
    //[ProtoContract]
    public partial class ActorResponse : ProtoObject, IActorResponse
    {
        //[ProtoMember(1)]
        public int RpcId { get; set; }
        //[ProtoMember(2)]
        public int Error { get; set; }
        //[ProtoMember(3)]
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }
    public class MessageAttribute : BaseAttribute
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
    public interface IActorLocationMessage : IActorRequest
    {
    }

    public interface IActorLocationRequest : IActorRequest
    {
    }

    public interface IActorLocationResponse : IActorResponse
    {
    }
    // 不需要返回消息
    public interface IActorMessage : IMessage
    {
    }

    public interface IActorRequest : IRequest
    {
    }

    public interface IActorResponse : IResponse
    {
    }

    public interface IActorNotification : IActorRequest
    {
        long NotificationId
        {
            get;
            set;
        }
    }
    public class ResponseTypeAttribute : BaseAttribute
    {
        public Type Type { get; }

        public ResponseTypeAttribute(Type type)
        {
            this.Type = type;
        }
    }
}

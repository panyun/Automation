using EL.Async;

namespace EL.Robot.Core
{
    public interface W_IMHandler
    {
        ELVoid Handle(WChannel channel, object message);
        Type GetMessageType();

        Type GetResponseType();
    }
    public abstract class W_AMHandler<Message> : W_IMHandler where Message : class
    {
        protected abstract ELTask Run(WChannel channel, Message message);

        public async ELVoid Handle(WChannel channel, object msg)
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
}

using EL;
using EL.Async;

namespace Automation.Parser
{
    public class Msg
    {
        public Msg()
        {

        }
        public Msg(string msg)
        {
            Id = (long)IdGenerater.Instance.GenerateInstanceId();
            Time = TimeHelper.ServerNow();
            Message = msg;
        }
        public long Id { get; set; }
        public string Message { get; set; }
        public string Nickname { get; set; }
        public long Time { get; set; }
        public string GroupName { get; set; }
        public Msg Clone()
        {
            return new Msg()
            {
                Id = Id,
                Message = Message,
                Nickname = Nickname,
                Time = Time,
                GroupName = GroupName
            };
        }
    }
}

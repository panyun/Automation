using EL.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Component
{
    public class TakeTimeComponent : IDisposable
    {
        /// <summary>
        /// 运行时间
        /// </summary>
        public long Ticks { get; set; } = DateTime.Now.Ticks;
        /// <summary>
        /// 为了防止忘记显式的调用Dispose方法
        /// </summary>
        public static TakeTimeComponent StartNew()
        {
            return new TakeTimeComponent();
        }

        public void Dispose()
        {
            //通知垃圾回收器不再调用终结器
            GC.SuppressFinalize(this);
        }
    }
    public static class TakeTimeComponentSystem
    {
        public static long GetNowUtc(this TakeTimeComponent self)
        {
            return TimeHelper.ServerNow();
        }
        public static DateTime GetDatetime(long time)
        {
            return TimeHelper.ToDateTime(time);
        }
        public static long ReStart(this TakeTimeComponent self)
        {
            var takeTime = self.GetTakeTime();
            self.Ticks = DateTime.Now.Ticks;
            return takeTime;
        }
        public static long GetTakeTime(this TakeTimeComponent self)
        {
            return (DateTime.Now.Ticks - self.Ticks) / 10000;
        }
        public static long Stop(this TakeTimeComponent self)
        {
            var takeTime = (DateTime.Now.Ticks - self.Ticks) / 10000;
            self.Dispose();
            return takeTime == default ? 1 : takeTime;
        }
    }
}

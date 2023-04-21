using EL.Async;
using EL.Robot.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.Core.SqliteEntity
{
    /// <summary>
    /// 计划数据
    /// </summary>
    public class PlanData
    {
        public long Id { get; set; }
        public long FlowId { get; set; }
        public string FlowName { get; set; }
        public string Expression { get; set; }
        public short Enable { get; set; }
        public long UpdateTime { get; set; }
        public DateTime GetUpdateTime
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(UpdateTime).LocalDateTime;
            }
        }
        public void Update()
        {
            this.UpdateTime = GetTime();
        }
        public static long GetTime()
        {
           return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        }
    }
}

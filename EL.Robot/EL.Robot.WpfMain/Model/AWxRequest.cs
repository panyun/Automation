using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Robot.WpfMain.Model
{
    public abstract class AWxRequest<T>
    {
        public T data { get; set; }
        /// <summary>
        /// 600000 正常，非600000不正常
        /// </summary>
        public int code { get; set; }
        public string msg { get; set; }
        public bool IsSuccess
        {
            get
            {
                return code == 600000;
            }
        }
    }
}

using Robot.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Robot.ViewModel
{
    public class BigRobotViewModel : PropertyChangeBase
    {
        CancellationTokenSource cancellationToken = new CancellationTokenSource();
        public BigRobotViewModel()
        {
            //Run();
            IninImg();
            ChangeImg(true);
        }
        private string _image;
        public string Image
        {
            set
            {
                _image = value;
                NC();
            }
            get { return _image; }
        }


        private int _percent;
        public int Percent
        {
            set
            {
                _percent = value;
                NC();
            }
            get { return _percent; }
        }


        private int _msgType;
        /// <summary>
        /// 消息类型 0 提示，1 异常，2 成功
        /// </summary>
        public int MsgType
        {
            set
            {
                _msgType = value;
                NC();
            }
            get { return _msgType; }
        }
        private string _msgTitle;
        public string MsgTitle
        {
            set
            {
                _msgTitle = value;
                NC();
            }
            get { return _msgTitle; }
        }


        private string _msgContent;
        public string MsgContent
        {
            set
            {
                _msgContent = value;
                NC();
            }
            get { return _msgContent; }
        }

        private string _msgButtonTxt;
        public string MsgButtonTxt
        {
            set
            {
                _msgButtonTxt = value;
                NC();
            }
            get { return _msgButtonTxt; }
        }

        private string _fontGround;
        public string FontGround
        {
            set
            {
                _fontGround = value;
                NC();
            }
            get { return _fontGround; }
        }

        private bool _showMsg;
        public bool ShowMsg
        {
            set
            {
                _showMsg = value;
                NC();
            }
            get { return _showMsg; }
        }
        private int State { get; set; } = 0;
        public void ChangeImg(bool state = true)
        {
            State = state ? 0 : 1;
        }
        void IninImg()
        {
            Task.Run(() =>
            {
                var i = 0;
                while (true)
                {
                    string uri = string.Empty;
                    if (State == 0)
                    {
                        uri = $"pack://application:,,,/EL.Robot.WpfMain;component/Resources/Images/Robot/{i:00000}.png";
                        i++;
                        if (i > 25)
                            i = 0;
                    }
                    else if (State == 1)
                    {
                        uri = $"pack://application:,,,/EL.Robot.WpfMain;component/Resources/Images/RobotEx/机器人报错浮窗_{i:00000}.png";
                        i++;
                        if (i > 49)
                            i = 0;
                    }
                    Image = uri;
                    Thread.Sleep(Sleep);
                }
            });
        }
        public int Sleep { get; set; } = (1000/25);

    }
}

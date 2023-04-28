using EL.Robot.Component;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.Robot.WindowApiTest
{

    public partial class DesignViewForm : UserControl
    {
        public Action<Flow> InitAction { get; set; }
        public Flow Flow { get; set; }
        public DesignViewForm()
        {
            InitializeComponent();
            InitAction = (x) =>
            {
                x
            };
        }
    }
}

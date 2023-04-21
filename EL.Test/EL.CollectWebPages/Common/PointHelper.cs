using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.CollectWebPages.Common
{
    public static class PointHelper
    {
        public static double VWidth;
        public static double VHeight;
        static PointHelper()
        {
            var Desktop = Screen.PrimaryScreen.Bounds;
            VWidth =  (double)65536 / (double)(Desktop.Width  + 1);
            VHeight = (double)65536 / (double)(Desktop.Height + 1);
        }

        public static Point GetMovePoint(this Point point)
        {   
            return new Point((int)(point.X * VWidth), (int)(point.Y * VHeight));
        }

    }
}

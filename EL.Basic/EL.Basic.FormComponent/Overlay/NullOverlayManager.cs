using EL.Overlay;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EL.Overlay
{
    public class NullOverlayManager : IOverlayManager
    {
        public int Size { get; set; }
        public int Margin { get; set; }

        public void Dispose()
        {
            // Noop
        }

        public void Show(Rectangle rectangle, Color color, int durationInMs)
        {

            // Noop
        }

        public void ShowBlocking(Rectangle rectangle, Color color, int durationInMs)
        {
            Thread.Sleep(durationInMs);
        }
    }
}

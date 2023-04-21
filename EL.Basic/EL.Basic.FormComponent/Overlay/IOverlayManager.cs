﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EL.Overlay
{
    public interface IOverlayManager : IDisposable
    {
        /// <summary>
        /// Border size of the overlay
        /// </summary>
        int Size { get; set; }

        /// <summary>
        /// Margin of the overlay (use negative to move it inside)
        /// </summary>
        int Margin { get; set; }

        /// <summary>
        /// Shows the overlay for a given duration asynchronously.
        /// </summary>
        void Show(Rectangle rectangle, Color color, int durationInMs);

        /// <summary>
        /// Shows the overlay for a given duration and blocks further execution until it is hidden again.
        /// </summary>
        void ShowBlocking(Rectangle rectangle, Color color, int durationInMs);
    }
}

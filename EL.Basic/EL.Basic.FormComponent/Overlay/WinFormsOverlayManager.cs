﻿//#if NETFRAMEWORK || NETCOREAPP
//using EL.Async;
//using EL.WindowsAPI;
//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Threading;

//namespace EL.Overlay
//{
//    public class WinFormsOverlayManager : IOverlayManager
//    {
//        public int Size { get; set; }
//        public int Margin { get; set; }
//        //public ELTask<IUIAutomationElement> ELTaskOver { get; set; }
//        public WinFormsOverlayManager()
//        {
//            Size = 3;
//            Margin = 0;
//        }

//        public void Show(Rectangle rectangle, Color color, int durationInMs)
//        {
//            if (!rectangle.IsEmpty)
//            {
//#if NET35
//                new Thread(() =>
//                {
//                    CreateAndShowForms(rectangle, color, durationInMs);
//                }).Start();
//#elif NET40
//                System.Threading.Tasks.Task.Factory.StartNew(() =>
//                {
//                    CreateAndShowForms(rectangle, color, durationInMs);
//                });
//#else
//                System.Threading.Tasks.Task.Run(() => CreateAndShowForms(rectangle, color, durationInMs));
//#endif
//            }
//        }

//        public void ShowBlocking(Rectangle rectangle, Color color, int durationInMs)
//        {
//            CreateAndShowForms(rectangle, color, durationInMs);
//        }

//        private void CreateAndShowForms(Rectangle rectangle, Color color, int durationInMs)
//        {
//            var leftBorder = new Rectangle(rectangle.X - Margin, rectangle.Y - Margin, Size, rectangle.Height + 2 * Margin);
//            var topBorder = new Rectangle(rectangle.X - Margin, rectangle.Y - Margin, rectangle.Width + 2 * Margin, Size);
//            var rightBorder = new Rectangle(rectangle.X + rectangle.Width - Size + Margin, rectangle.Y - Margin, Size, rectangle.Height + 2 * Margin);
//            var bottomBorder = new Rectangle(rectangle.X - Margin, rectangle.Y + rectangle.Height - Size + Margin, rectangle.Width + 2 * Margin, Size);
//            var allBorders = new[] { leftBorder, topBorder, rightBorder, bottomBorder };

//            var gdiColor = Color.FromArgb(color.A, color.R, color.G, color.B);
//            var forms = new List<OverlayRectangleForm>();
//            foreach (var border in allBorders)
//            {
//                var form = new OverlayRectangleForm { BackColor = gdiColor };
//                forms.Add(form);
//                // Position the window


//                User32.SetWindowPos(form.Handle, new IntPtr(-1), border.X, border.Y,
//                    border.Width, border.Height, SetWindowPosFlags.SWP_NOACTIVATE);
//                // Show the window
//                User32.ShowWindow(form.Handle, ShowWindowTypes.SW_SHOWNA);
//            }
//            Thread.Sleep(durationInMs);
//            foreach (var form in forms)
//            {
//                // Cleanup
//                form.Hide();
//                form.Close();
//                form.Dispose();
//            }
//        }
//        OverlayRectangleForm overLayForm = default(OverlayRectangleForm);
//        //public List<OverlayRectangleForm> Forms = new List<OverlayRectangleForm>();
//        public async ELTask Show(Color color)
//        {
//            //var element = await ELTaskOver;
//            ThreadSynchronizationContext.Instance.Post(() =>
//            {
//                if (overLayForm != null)
//                    overLayForm.Hide();
//                var rec = element.CurrentBoundingRectangle;
//                var rectangle = new Rectangle(rec.left, rec.top, rec.right - rec.left, rec.bottom - rec.top);
//                var gdiColor = Color.FromArgb(color.A, color.R, color.G, color.B);
//                if (overLayForm == null)
//                {
//                    overLayForm = new OverlayRectangleForm();
//                }
//                overLayForm.SetWindow(element, gdiColor);
//                overLayForm.Show();
//            });
//        }

//        public void Dispose()
//        {
//            // Nothing to dispose
//        }
//    }
//}
//#endif

using EL.CollectWebPages.BLL;
using EL.CollectWebPages.Common;
using EL.CollectWebPages.Model;
using NPOI.SS.Formula.Functions;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EL.CollectWebPages
{
    public partial class Form2 : Form
    {
        CurrentData currentData = null;
        public string RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
        public Form2()
        {
            InitializeComponent();

            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;

        }
        Point mouseDownPoint = new Point(); //记录拖拽过程鼠标位置
        bool isMove = false;    //判断鼠标在picturebox上移动时，是否处于拖拽过程(鼠标左键是否按下)
        int zoomStep = 20;      //缩放步长 
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
                isMove = true;
                pictureBox1.Focus();
            }
        }
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMove = false;
            }
        }
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentData != null && isMove)
            {
                pictureBox1.Focus();
                var moveX = Cursor.Position.X - mouseDownPoint.X;
                var moveY = Cursor.Position.Y - mouseDownPoint.Y;
                var x = pictureBox1.Location.X + moveX;
                var y = pictureBox1.Location.Y + moveY;
                pictureBox1.Location = new Point(x, y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }
        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            var x = e.Location.X;
            var y = e.Location.X;
            var ow = pictureBox1.Width;
            var oh = pictureBox1.Height;
            if (e.Delta > 0)
            {
                pictureBox1.Width += zoomStep;
                pictureBox1.Height += zoomStep;
                PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance | BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);
                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }
            else
            {
                pictureBox1.Width -= zoomStep;
                pictureBox1.Height -= zoomStep;
                PropertyInfo pInfo = pictureBox1.GetType().GetProperty("ImageRectangle", BindingFlags.Instance | BindingFlags.NonPublic);
                Rectangle rect = (Rectangle)pInfo.GetValue(pictureBox1, null);
                pictureBox1.Width = rect.Width;
                pictureBox1.Height = rect.Height;
            }


            var VX = (int)((double)x * (ow - pictureBox1.Width) / ow);
            var VY = (int)((double)y * (oh - pictureBox1.Height) / oh);

            pictureBox1.Location = new Point(pictureBox1.Location.X + VX, pictureBox1.Location.Y + VY);
        }
        public Dictionary<string, (string json, string image)> Dic = new Dictionary<string, (string json, string image)>();
        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string sPath = folderBrowserDialog1.SelectedPath;
                if (Directory.Exists(sPath))
                {
                    Dic.Clear();
                    RootPath = sPath;
                    currentData = null;
                    listBox1.Items.Clear();
                    var list = new List<string>();
                    var file = Directory.GetFiles(RootPath, "*.json", searchOption: SearchOption.AllDirectories);
                    if (file?.Any() == true)
                    {
                        foreach (var item in file)
                        {
                            var name = Path.GetFileNameWithoutExtension(item);
                            Dic[name] = (item, Path.Combine(Path.GetDirectoryName(item), $"{name}.jpg"));
                            list.Add(name);
                        }
                    }
                    foreach (var item in list)
                    {
                        listBox1.Items.Add(item);
                    }
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            currentData = null;
            var data = listBox1.SelectedItem;
            if (data != null)
            {
                if (Dic.TryGetValue(data.ToString(), out var result))
                {
                    var jsonPath = result.json;
                    var ImagePath = result.image;
                    var EWindow = File.ReadAllText(jsonPath).ToObj<EWindow>();

                    currentData = new CurrentData(ImagePath, jsonPath, EWindow);
                    WindowRefresh();
                }
            }
        }

        /// <summary>
        /// 下一个节点
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            currentData?.Next();
            WindowRefresh();
        }
        /// <summary>
        /// 上一个节点
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            currentData?.Previous();
            WindowRefresh();
        }
        /// <summary>
        /// 修改内容
        /// </summary>
        private void button4_Click(object sender, MouseEventArgs e)
        {
            if (currentData != null)
            {
                var currentWindow = currentData.GetIndexWindow();
                currentWindow.Name = textBox2.Text;
                currentWindow.Remarks = textBox1.Text;
                currentWindow.Enable = checkBox1.Checked;
                lock (LockObj)
                {
                    var saveJson = currentData.Window.ToJson();
                    File.WriteAllText(currentData.jsonPath, saveJson);
                }
                WindowRefresh();
            }
        }
        private static object LockObj = new object();
        private void DrawRec(string filepath, Rectangle rectangle)
        {
            lock (LockObj)
            {
                var image = Image.FromFile(filepath);
                Graphics interGraphics = Graphics.FromImage(image);
                Pen pen = new Pen(Color.Red, 2);
                interGraphics.DrawRectangle(pen, rectangle);
                pictureBox1.Image = image;
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        private void WindowRefresh()
        {
            if (currentData != null)
            {
                RefreshImg(currentData);
                RefreshInfo(currentData);
            }
        }
        private void RefreshImg(CurrentData currentData)
        {
            var currentWindow = currentData.GetIndexWindow();
            DrawRec(currentData.ImagePath, currentWindow.RelativeRectangle);
        }
        private void RefreshInfo(CurrentData currentData)
        {
            var currentWindow = currentData.GetIndexWindow();
            label15.Text = $"{currentData.Index} / {currentData.WindowsCount}";

            label1.Text = currentWindow.ID;
            textBox2.Text = currentWindow.Name;
            label9.Text = currentWindow.ClassName;
            label6.Text = $"X:{currentWindow.Rectangle.X} Y:{currentWindow.Rectangle.Y} W:{currentWindow.Rectangle.Width} H:{currentWindow.Rectangle.Height}";
            label8.Text = currentWindow.ProgramType.ToString();
            label10.Text = currentWindow.ControlType.ToString();
            label13.Text = currentWindow.LocalizedControlType.ToString();
            textBox1.Text = currentWindow.Remarks;
            label16.Text = currentWindow.Value;
            label20.Text = currentWindow.Role;
            label22.Text = currentWindow.Text;
            checkBox1.Checked = currentWindow.Enable;
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }
}

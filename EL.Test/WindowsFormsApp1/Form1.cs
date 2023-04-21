using CefSharp.WinForms;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ChromiumWebBrowser chromiumWebBrowser = new ChromiumWebBrowser("www.bing.com");
            chromiumWebBrowser.Height = 200;
            chromiumWebBrowser.Width = 200;
            this.Controls.Add(chromiumWebBrowser);
            //WebBrowser webBrowser = new WebBrowser();
            //webBrowser.Width = 200;
            //webBrowser.Height = 200;
            //webBrowser.Navigate("www.bing.com");
            //WebView2 webView2 = new WebView2();
            //this.Controls.Add(webBrowser);
        }
    }
}

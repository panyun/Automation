using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {

        static void Main(string[] args)
        {
            try
            {
                var dd = new CDD();

                int ret = dd.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DD94687.32.dll"));
                if (ret != 1)
                {
                    Console.WriteLine("Load Error");
                }


                ret = dd.btn(0); //DD Initialize
                if (ret != 1)
                {
                    Console.WriteLine("Initialize Error");
                }
                else
                {
                    Thread.Sleep(5000);
                    dd.str("pjcW.1234");
                    Console.WriteLine("输入密码完毕");


                    //Task.Run(() =>
                    //{

                    //    while (true)
                    //    {
                    //        dd.key(400, 1);
                    //        System.Threading.Thread.Sleep(50);           //may, delay 50ms
                    //        dd.key(400, 2);

                    //        dd.key(810, 1);
                    //        System.Threading.Thread.Sleep(50);           //may, delay 50ms
                    //        dd.key(810, 2);

                    //        dd.key(710, 1);
                    //        System.Threading.Thread.Sleep(50);           //may, delay 50ms
                    //        dd.key(710, 2);
                    //        Thread.Sleep(1000);
                    //        Console.WriteLine($"CapsLock:{Console.CapsLock} NumberLock:{Console.NumberLock}");
                    //    }
                    //});

                    //Thread.Sleep(5000);

                    //dd.str("pjcw1234");
                    //Console.WriteLine("输入密码完毕");
                    //Thread.Sleep(5000);
                    ////按键
                    //dd.btn(1);
                    //System.Threading.Thread.Sleep(50);           //may, delay 50ms
                    //dd.btn(2);
                    //Console.WriteLine("点击完毕");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
            Console.WriteLine("测试完毕!");
            Console.ReadLine();

        }
    }
}

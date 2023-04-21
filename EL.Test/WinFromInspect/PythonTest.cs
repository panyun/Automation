using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace WinFromInspect
{
    public static class PythonTest
    {
        public static void RunPythonTest()
        {
            try
            {
                var engine = Python.CreateEngine();
             
                var paths = engine.GetSearchPaths();
                paths.Add(@"D:\Work Space\python\Uidetect_Ocr\Uidetect_backend\lite\");
                paths.Add(@"D:\Work Space\python\Uidetect_Ocr\Uidetect_backend\lite\Lib\site-packages");
                engine.SetSearchPaths(paths);
                //dynamic py = engine.ExecuteFile(@"D:\Work Space\c-automation\EL.Test\WinFromInspect\demo.py");
                dynamic py = engine.ExecuteFile(@"D:\Work Space\python\Uidetect_Ocr\Uidetect_backend\ocr.py");
                var resp = py.Test();
            }
            catch (Exception ex)
            {

                throw;
            }
        
            //// 加载外部 python 脚本文件.
            ScriptRuntime pyRumTime = Python.CreateRuntime();


            //dynamic obj = pyRumTime.UseFile(@"D:\Work Space\python\Uidetect_Ocr\Uidetect_backend\ocr.py");
            //dynamic obj = pyRumTime.UseFile(@"D:\Work Space\python\Uidetect_Ocr\Uidetect_backend\ocr.py");

            //try
            //{
            //    var test = obj.Test("C:/Users/panyun.li/Desktop/1681783604395.png");
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}


            //// ==================================================
            //// 简单调用脚本文件中的方法.
            //Console.WriteLine(obj.welcome("Test C# Call Python."));
            //Console.WriteLine(obj.welcome("测试中文看看是否正常！"));

            //// ==================================================
            //// 测试自定义对象.
            //DataObjectTest testObj = new DataObjectTest("张三", 20, "");


            //Console.WriteLine("调用脚本前对象数据：{0}", testObj);
            //obj.testAddAge(testObj);
            //Console.WriteLine("调用 testAddAge 脚本后，对象数据={0}", testObj);
            //obj.testAddAge2(testObj);
            //Console.WriteLine("调用 testAddAge2 脚本后，对象数据={0}", testObj);


            //// ==================================================
            //// 测试 List.
            ////List list = obj.list1;
            ////testList.Add("List数据1");
            ////testList.Add("List数据2");
            ////testList.Add("List数据3");
            ////// 测试参数为 List.
            ////string result = obj.testList(testList);
            ////Console.WriteLine("调用 testList ， 返回结果：{0}", result);

            //// ==================================================
            //// 测试 Set.
            //IronPython.Runtime.SetCollection testSet = new IronPython.Runtime.SetCollection();
            //testSet.add("Set数据1");
            //testSet.add("Set数据2");
            //testSet.add("Set数据3");
            //// 测试参数为 Set.
            //var result = obj.testSet(testSet);
            //Console.WriteLine("调用 testSet ， 返回结果：{0}", result);

            //// ==================================================
            //// 测试 Dictionary.
            //IronPython.Runtime.PythonDictionary testDictionary = new IronPython.Runtime.PythonDictionary();
            //testDictionary["Key1"] = "Value1";
            //testDictionary["Key2"] = "Value2";
            //testDictionary["Key3"] = "Value3";
            //// 测试参数为 Dictionary.
            //result = obj.testDictionary(testDictionary);
            //Console.WriteLine("调用 testDictionary ， 返回结果：{0}", result);
            //Console.ReadLine();
        }
    }

    public class DataObjectTest
    {
        public DataObjectTest(string userName, int age, string desc)
        {
            UserName = userName;
            Age = age;
            Desc = desc;
        }

        /// <summary>
        /// 属性：姓名
        /// </summary>
        public string UserName { set; get; }
        /// <summary>
        /// 属性：年龄
        /// </summary>
        public int Age { set; get; }
        /// <summary>
        /// 属性：描述
        /// </summary>
        public string Desc { set; get; }


        public void AddAge(int age)
        {
            this.Age += age;
            this.Desc = String.Format("{0}长大了{1}岁（from C#）", this.UserName, age);
        }
        public override string ToString()
        {
            return String.Format("姓名：{0}; 年龄：{1}; 描述:{2}", this.UserName, this.Age, this.Desc);
        }
    }
}

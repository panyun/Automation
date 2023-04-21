using Automation.Inspect;
using EL;
using NUnit.Framework;
namespace Inspect.Test
{
    public class JavaInspectComponentTest
    {
        [SetUp]
        public void SetupAsync()
        {
            //创建
            Boot.App = new AppMananger();
            //加载程序集
            Boot.App.EventSystem.Add(typeof(WinFormInspectComponent).Assembly);
            Boot.AddComponent<InspectComponent>();
        }
        //[Test]
        //public void JavaElement()
        //{
        //    var inspect = Boot.GetComponent<InspectComponent>();
        //    var component = inspect.GetComponent<JavaFormInspectComponent>();
        //    component.FromPoint();
        //}
    }

}
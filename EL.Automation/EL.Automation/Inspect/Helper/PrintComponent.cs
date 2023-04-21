using Automation.Parser;
using EL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interop.UIAutomationClient;

namespace Automation.Inspect
{
    public static class PrintComponent
    {
        public static void PrintAvigation(ElementUIA e)
        {
            var json = JsonHelper.ToJson(new
            {
                e.NativeElement.CurrentAcceleratorKey,
                e.NativeElement.CurrentAccessKey,
                e.NativeElement.CurrentAriaRole,
                e.NativeElement.CurrentAutomationId,
                e.NativeElement.CurrentControlType,
                e.NativeElement.CurrentFrameworkId,
                e.NativeElement.CurrentItemStatus,
                e.NativeElement.CurrentOrientation,
                e.NativeElement.CurrentClassName,
                e.NativeElement.CurrentName,
            });
            Debug.WriteLine($"Avigation:{json}");
            Log.Trace($"Avigation:{json}");
        }
        //public static void PrintAvigation(ElementJava e)
        //{
        //    var pro = e.Properties.FirstOrDefault(x => x.Name == "Focused element");
        //    if (pro != null)
        //        e.Properties.Remove(pro);
        //    var json = JsonHelper.ToJson(e.Properties);
        //    Debug.WriteLine($"Avigation:{json}");
        //    Log.Trace($"Avigation:{json}");
        //}
        public static void PrintAvigation(ElementNode e, List<IUIAutomationElement> entitys)
        {
            if (entitys == null || entitys.Count == 0)
            {
                Debug.WriteLine("Avigation:当前节点导航为NUll");
                Log.Trace("Avigation:当前节点导航为NUll");
                Log.Trace($"Avigation:当前节点导航为NUll");
                Debug.WriteLine($"Avigation:当前节点导航为NUll");
            }
            e.CurrentElementWin.NativeElement = entitys[0];
            var json = JsonHelper.ToJson(new
            {
                CompareId = e.CompareValues[nameof(ElementExpand.CompareId).ToLower()],
                e.CurrentElementWin.NativeElement.CurrentAcceleratorKey,
                e.CurrentElementWin.NativeElement.CurrentAccessKey,
                e.CurrentElementWin.NativeElement.CurrentAriaRole,
                e.CurrentElementWin.NativeElement.CurrentAutomationId,
                e.CurrentElementWin.NativeElement.CurrentControlType,
                e.CurrentElementWin.NativeElement.CurrentFrameworkId,
                e.CurrentElementWin.NativeElement.CurrentItemStatus,
                e.CurrentElementWin.NativeElement.CurrentOrientation,
                e.CurrentElementWin.NativeElement.CurrentClassName,
                e.CurrentElementWin.NativeElement.CurrentName,
            });
            Debug.WriteLine($"Avigation:{json}");
            //Log.Trace($"Avigation:{json}");
            //Log.Trace($"Avigation:{new { e.Index, e.Length, e.LevelIndex, currentLenth = entitys.Count } } ");
            Debug.WriteLine($"Avigation:{new { e.Index, e.Length, e.LevelIndex, currentLenth = entitys.Count }} ");
        }
        public static void PrintCatch(ElementNode e)
        {
            var json = JsonHelper.ToJson(new
            {
                CompareId = e.CompareValues[nameof(ElementExpand.CompareId).ToLower()],
                e.CurrentElementWin.NativeElement.CurrentAcceleratorKey,
                e.CurrentElementWin.NativeElement.CurrentAccessKey,
                e.CurrentElementWin.NativeElement.CurrentAriaRole,
                e.CurrentElementWin.NativeElement.CurrentAutomationId,
                e.CurrentElementWin.NativeElement.CurrentControlType,
                e.CurrentElementWin.NativeElement.CurrentFrameworkId,
                e.CurrentElementWin.NativeElement.CurrentItemStatus,
                e.CurrentElementWin.NativeElement.CurrentOrientation,
                e.CurrentElementWin.NativeElement.CurrentClassName,
                e.CurrentElementWin.NativeElement.CurrentName,

            }) ;
            Debug.WriteLine($"Catch:{json}");
            //Log.Trace($"Catch:{json}");
            //Log.Trace($"Catch:{new { e.Index, e.Length, e.LevelIndex }} ");
            Debug.WriteLine($"Catch:{new { e.Index, e.Length, e.LevelIndex }} ");
        }

    }

}

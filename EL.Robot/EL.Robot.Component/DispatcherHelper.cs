using Automation.Inspect;
using Automation.Parser;
using EL.Robot.Component;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace EL.Robot.Core
{
    public class DispatcherHelper
    {
        public static Dispatcher dispatcher;
        public static Form BaseForm;

        public static void ExecInspect(Action<InspectComponent> action)
        {
            if (dispatcher != null)
                dispatcher?.Invoke(() =>
                {
                    (var inspect, var parser) = CreateInspect();
                    action.Invoke(inspect);
                }
               );
            if (BaseForm != null)
                BaseForm.Invoke(() =>
           {
               (var inspect, var parser) = CreateInspect();
               action.Invoke(inspect);
           });
        }
        public static void ExecInspect(Action<InspectComponent, ParserComponent> action)
        {
            if (dispatcher != null)
                dispatcher?.Invoke(() =>
                  {
                      (var inspect, var parser) = CreateInspect();
                      action.Invoke(inspect, parser);
                  });
            if (BaseForm != null)
                BaseForm?.Invoke(() =>
                {
                    (var inspect, var parser) = CreateInspect();
                    action.Invoke(inspect, parser);
                });
        }
        public static async Task<IResponse> ExecInspectAsync(Func<InspectComponent, ParserComponent, Task<IResponse>> action)
        {
            if (dispatcher != null)
                return await dispatcher.Invoke(async () =>
                  {
                      try
                      {
                          (var inspect, var parser) = CreateInspect();
                          return await action.Invoke(inspect, parser);
                      }
                      catch (Exception ex)
                      {
                          throw ex;
                      }
                  }
                  );
            if (BaseForm != null)
            {
                bool isExec = false;
                object val = default;
                var obj = BaseForm.BeginInvoke(async () =>
                {
                    try
                    {
                        (var inspect, var parser) = CreateInspect();
                        val = await action.Invoke(inspect, parser);
                        isExec = true;
                        return val;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                });
                await Task.Run(() =>
                 {
                     while (true)
                     {
                         Thread.Sleep(1);
                         if (isExec)
                             break;
                     }
                 });
                return val as IResponse;
            }
            return default;
        }
        public static void ExecDispatcher(Action action)
        {
            if (dispatcher != null)
                dispatcher.Invoke(() =>
            {
                action.Invoke();
            }
            );
            if (BaseForm != null)
                BaseForm.Invoke(() =>
                {
                    action.Invoke();
                }
        );
        }


        public static (InspectComponent, ParserComponent) CreateInspect()
        {
            InspectComponent inspect = default;
            ParserComponent parser = default;
            try
            {
                inspect = Boot.GetComponent<InspectComponent>();
                if (inspect == null)
                    inspect = Boot.AddComponent<InspectComponent>();
                parser = Boot.GetComponent<ParserComponent>();
                if (parser == null)
                    parser = Boot.AddComponent<ParserComponent>();
                return (inspect, parser);
            }
            catch (Exception ex)
            {
                inspect = null;
                parser = null;
            }
            parser = Boot.AddComponent<ParserComponent>();
            inspect = Boot.AddComponent<InspectComponent>();
            return (inspect, parser);
        }

        public static WpfInspect.MainWindow mainWindowInstance;
    }
}

using EL;
using EL.Async;
using EL.Input;
using EL.Overlay;
using Interop.UIAutomationClient;
using System.Diagnostics;
using System.Drawing;

namespace Automation.Inspect
{
	public static class CatchElementSystem
	{
		public static void CatchElement(this CatchUIRequest self)
		{
			var inspect = Boot.GetComponent<InspectComponent>();
			var formOver = inspect.GetComponent<FormOverLayComponent>();
			var winInspect = inspect.GetComponent<WinFormInspectComponent>();
			var javaInspect = inspect.GetComponent<JavaFormInspectComponent>();
			var vcOcrInspect = inspect.GetComponent<VcOcrInspectComponent>();
			var playwrightInspect = inspect.GetComponent<PlaywrightInspectComponent>();
			if (self.Mode != Mode.None)
				formOver.Mode = self.Mode;
			formOver.Form.Show();
			formOver.FloatShow(self.Msg, 1500);
			RequestOptionComponent.Index = 0;
			inspect.NewRepeatedTimerId = Boot.GetComponent<TimerComponent>().NewRepeatedTimer(100, async () =>
			{

				formOver.Form.UpdateText(inspect.CurrentElement);
				if (formOver.IsCatchComplete)
				{
					formOver.CompleteEvent?.Invoke();
					return;
				}
				ElementIns? elementTemp = default;
				if (self.CatchType == CatchType.Element)
				{
					await ElementCatch();
					if (elementTemp != null && elementTemp.ProcessId == Process.GetCurrentProcess().Id)
						return;
				}
				if (self.CatchType == CatchType.Window)
					WindowCatch();
				if (self.CatchType == CatchType.WxChat && !WxChatCatch())
					return;
				if (elementTemp == default)
					return;
				if (ElementIns.Equal(inspect.CurrentElement, elementTemp))
					return;
				inspect.CurrentElement = elementTemp;
				formOver.ELTaskOverLay = ELTask<dynamic>.Create();
				formOver.Show(Color.Red).Coroutine();
				formOver.ELTaskOverLay.SetResult(inspect.CurrentElement);
				#region wxChatCatch
				bool WxChatCatch()
				{
					try
					{
						IUIAutomationElement e = winInspect.ElementFromPoint();
						if (e != default)
							e = e.GetNativeWindowHandle();
						elementTemp = e?.ConvertElementInspect() ?? default;
						var className = elementTemp.ElementUIA.CurrentClassName.ToLower();
						if (!className.Contains("chat") && className != "weworkwindow")
							return false;
						return true;
					}
					catch (Exception)
					{

					}
					return false;
				}
				#endregion
				#region windowCatch
				void WindowCatch()
				{
					try
					{
						IUIAutomationElement e = winInspect.ElementFromPoint();
						if (e != default)
							e = e.GetNativeWindowHandle();
						elementTemp = e?.ConvertElementInspect() ?? default;
					}
					catch (Exception)
					{

					}

				}
				#endregion
				#region playwrightAsync
				async Task playwrightAsync()
				{
					try
					{
						if (playwrightInspect == default || playwrightInspect.PlaywrightContexts.Count == 0)
							return;
						elementTemp = await playwrightInspect.ElementFromPoint();
						if (elementTemp == null)
							return;
						if (inspect.CurrentElement?.ElementPlaywright != default && elementTemp.ElementPlaywright.PXPath == inspect.CurrentElement.ElementPlaywright.PXPath)
							return;
					}
					catch (Exception)
					{

					}

				}
				#endregion
				#region ElementCatch
				async ELTask ElementCatch()
				{
					try
					{
						if (formOver.Mode == Mode.Auto)
						{
							var e1 = await javaInspect.FromPoint();
							elementTemp = e1?.ConvertElementInspect() ?? default;
							if (elementTemp == null)
							{
								IUIAutomationElement e = winInspect.ElementFromPoint();
								elementTemp = e?.ConvertElementInspect() ?? default;
								if (elementTemp != null) return;
							}
							if (elementTemp == default || !winInspect.CanCatch(elementTemp.ElementUIA))
							{
								elementTemp = null;
								await playwrightAsync();
							}
							if (elementTemp == null)
							{
								var e = winInspect.ElementFromPoint_MSAA();
								if (e == default)
									return;
								elementTemp = e?.ConvertElementInspect() ?? default;
							}
							if (elementTemp == null)
							{
								if (vcOcrInspect == default) return;
								elementTemp = await vcOcrInspect.ElementFromPoint();
							}
						}
						if (formOver.Mode == Mode.UIA2)
						{
							IUIAutomationElement e = winInspect.ElementFromPoint();
							elementTemp = e?.ConvertElementInspect() ?? default;
						}
						if (formOver.Mode == Mode.MSAA)
						{
							var e = winInspect.ElementFromPoint_MSAA();
							if (e == default)
								elementTemp = default;
							elementTemp = e?.ConvertElementInspect() ?? default;
							return;

						}
						if (formOver.Mode == Mode.Playwright)
						{
							await playwrightAsync();
							return;
						}
						if (formOver.Mode == Mode.VCOcr)
						{
							if (vcOcrInspect == default) return;
							elementTemp = await vcOcrInspect.ElementFromPoint();
							return;
						}
						if (formOver.Mode == Mode.JAB)
						{
							var e = await javaInspect.FromPoint();
							elementTemp = e?.ConvertElementInspect() ?? default;
							return;
						}
					}
					catch (Exception ex)
					{
						Log.Error(ex);
					}
				}
				#endregion

			});
		}
	}
}

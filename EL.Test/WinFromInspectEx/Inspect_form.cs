using Automation.Inspect;
using Automation.Parser;
using EL;
using EL.Async;
using EL.Basic.Component.Clipboard;
using EL.Overlay;

namespace WinFromInspect
{
    public partial class Inspect_form : Form
    {
        public Inspect_form()
        {
            InitializeComponent();
            // 异步方法全部会回掉到主线程
            SynchronizationContext.SetSynchronizationContext(ThreadSynchronizationContext.Instance);
            var inspect = Boot.AddComponent<InspectComponent>();
            var parser = Boot.GetComponent<ParserComponent>();
            inspect.KeyboardHookInit();
            inspect.Action = (x, y) =>
            {
                pictureBox1.BackgroundImage = WinPathComponentSystem.Base64ToImage(y.Img, new Size(pictureBox1.Width, pictureBox1.Height));
                pictureBox1.SizeMode = PictureBoxSizeMode.Normal;
                txt_Title.Text = y.MainWindowTitle;
                txt_NativeWindowTitle.Text = y.NativeWindowTitle;
                txt_Name.Text = y.Name;
                txt_Path.Text = y.Path;
                txt_ProcessName.Text = y.ProcessName;
                txt_Value.Text = y.Value;
                txt_wh.Text = $"{y.Width}/{y.Height}";
                txt_XY.Text = $"{y.X}/{y.Y}";
                txt_ProgramType.Text = y.ProgramType;
            };
        }
        private void btn_ElementCatch_Click(object sender, EventArgs e)
        {
            var inspect = Boot.GetComponent<InspectComponent>();
            inspect.CatchElement();
        }
        public static MouseAction elementMouseAction = default;
        List<ELAction> eLActions = new List<ELAction>();
        private void btn_Parser_Click(object sender, EventArgs e)
        {
            try
            {
                var parser = Boot.GetComponent<ParserComponent>();
                var inspect = Boot.GetComponent<InspectComponent>();
                var json = inspect.GetComponent<ClipboardComponent>().GetFromClipboard();
                var path = JsonHelper.FromJson<ElementPath>(json);
                eLActions.Clear();
                eLActions.Add(elementMouseAction);
                if (!string.IsNullOrEmpty(txt_Content.Text))
                    eLActions.Add(new InputAction() { InputTxt = txt_Content.Text });
                var element = parser.Start(path, eLActions);
                var formOver = inspect.GetComponent<FormOverLayComponent>();
                formOver.From.Show();
                formOver.ELTaskOverLay = ELTask<dynamic>.Create();
                formOver.Show(Color.Red).Coroutine();
                formOver.ELTaskOverLay.SetResult(element);
            }
            catch (Exception ex)
            {

            }
        }
        private void cb_Click_SelectedIndexChanged(object sender, EventArgs e)
        {
            elementMouseAction = new MouseAction()
            {
                ActionType = ActionType.Mouse,
                ClickType = ClickType.LeftClick,
                LocationType = LocationType.Center,
                OffsetX = 0,
                OffsetY = 0,
            };
            if (((ComboBox)sender).SelectedIndex == 0)
                elementMouseAction.ClickType = ClickType.LeftClick;
            if (((ComboBox)sender).SelectedIndex == 1)
                elementMouseAction.ClickType = ClickType.LeftDoubleClick;
            if (((ComboBox)sender).SelectedIndex == 2)
                elementMouseAction.ClickType = ClickType.RightClick;
        }
    }
}
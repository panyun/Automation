namespace EL.CollectWebPages.Model
{
    public class CurrentData
    {
        public CurrentData(string ImagePath, string jsonPath, EWindow Window)
        {
            this.ImagePath = ImagePath;
            this.jsonPath = jsonPath;
            this.Window = Window;
            Windows = this.Window.GetEWindows();
            WindowsCount = Windows?.Count() > 0 ? Windows.Count() : 0;
        }
        public string ImagePath { get; private set; }
        public string jsonPath { get; private set; }
        public EWindow Window { get; private set; }
        public List<EWindow> Windows { get; private set; }
        public int WindowsCount { get; private set; }
        public int Index { get; set; } = 1;
        public void Next()
        {
            if (Index + 1 <= WindowsCount)
            {
                Index = Index + 1;
            }
        }
        public void Previous()
        {
            if (Index - 1 >= 1)
            {
                Index = Index - 1;
            }
        }
        public EWindow GetIndexWindow()
        {
            return Windows[Index - 1];
        }
    }
}

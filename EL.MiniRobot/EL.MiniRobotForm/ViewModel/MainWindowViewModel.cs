using AduSkin.Controls.Metro;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiniRobotForm.Views;
using System.Windows.Controls;

namespace ViewModel
{
    public abstract class ViewModelBase : ObservableRecipient
    {

    }
    public class MainWindowViewModel : ViewModelBase
    {
        private static DesignMainWindow DesignMainWindow = new DesignMainWindow();
        private static MarketMainWindow MarketMainWindow = new MarketMainWindow();
        public RelayCommand<string> CheckedCommand => new RelayCommand<string>((x) =>
        {
            if (x == "robot")
                MainContent = DesignMainWindow;
            if (x == "market")
                MainContent = MarketMainWindow;
        });
        public MainWindowViewModel()
        {
        }
        private UserControl _MainContent = DesignMainWindow;
        public UserControl MainContent
        {
            get => _MainContent;
            set => SetProperty(ref _MainContent, value);
        }
    }
}

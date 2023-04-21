using CefSharp.DevTools.CSS;
using EL;
using EL.Robot.Core;
using EL.Robot.Core.SqliteEntity;
using EL.Robot.WpfMain.Command;
using EL.Robot.WpfMain.Config;
using EL.Sqlite;
using Robot.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Robot.ViewModel
{
    public class LogManageViewModel : PropertyChangeBase
    {
        private MvvmCommand _SelectFolderCommand;
        private SqliteComponent SqliteComponent;
        public LogManageViewModel()
        {
            SqliteComponent = Boot.GetComponent<RobotComponent>().GetComponent<SqliteComponent>();
            var list = new List<int>();
            for (int i = 1; i < 30; i++)
            {
                list.Add(i);
            }
            BackupDayList = new ObservableCollection<int>(list);
        }
        public bool AutomaticBackup
        {
            get
            {
                return ConfigItemsHelper.Log_IsAutomaticBackup;
            }
            set
            {
                ConfigItemsHelper.Log_IsAutomaticBackup = value;
                NC();
            }
        }
        public ObservableCollection<int> BackupDayList { get; private set; }
        public int BackupDays
        {
            get
            {
                return ConfigItemsHelper.Log_BackupDays;
            }
            set
            {
                ConfigItemsHelper.Log_BackupDays = value;
                NC();
            }
        }

        public string BackupDirectory
        {
            get
            {
                return ConfigItemsHelper.Log_BackupDirectory;
            }
            set
            {
                ConfigItemsHelper.Log_BackupDirectory = value;
                NC();
            }
        }
        public bool ScreenRecording
        {
            get
            {
                return ConfigItemsHelper.Log_IsScreenRecording;
            }
            set
            {
                ConfigItemsHelper.Log_IsScreenRecording = value;
                NC();
            }
        }
        public ICommand SelectFolderCommand => _SelectFolderCommand ?? new MvvmCommand(SelectFolder);
        public void SelectFolder()
        {
            System.Windows.Forms.FolderBrowserDialog openFileDialog = new System.Windows.Forms.FolderBrowserDialog();
            openFileDialog.SelectedPath = BackupDirectory;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BackupDirectory = openFileDialog.SelectedPath;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EL.Robot.WpfMain.Command
{
    public class MvvmCommand : ICommand
    {
        public Func<object, bool> CanExecuteCommand = null;
        public Action ExecuteCommand = null;
        public event EventHandler CanExecuteChanged;

        public MvvmCommand(Action executeCommand)
        {
            this.ExecuteCommand = executeCommand;
        }

        public MvvmCommand(Action executeCommand, Func<object, bool> canExecuteCommand)
        {
            this.ExecuteCommand = executeCommand;
            this.CanExecuteCommand = canExecuteCommand;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteCommand != null)
                return CanExecuteCommand(parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            ExecuteCommand?.Invoke();
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, EventArgs.Empty);
        }
    }
    public class MvvmCommand<T> : ICommand
    {
        public Func<T, bool> CanExecuteCommand = null;
        public Action<T> ExecuteCommand = null;
        public event EventHandler CanExecuteChanged;

        public MvvmCommand(Action<T> executeCommand)
        {
            this.ExecuteCommand = executeCommand;
        }

        public MvvmCommand(Action<T> executeCommand, Func<T, bool> canExecuteCommand)
        {
            this.ExecuteCommand = executeCommand;
            this.CanExecuteCommand = canExecuteCommand;
        }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteCommand != null)
                return CanExecuteCommand((T)parameter);
            return true;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            ExecuteCommand?.Invoke((T)parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

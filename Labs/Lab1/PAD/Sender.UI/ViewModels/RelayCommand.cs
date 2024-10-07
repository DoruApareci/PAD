using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sender.UI.ViewModels
{
    internal class RelayCommand : ICommand
    {
        Action<object> _Execute;
        Predicate<object> _CanExecute;

        public RelayCommand(Action<object> executeCommand, Predicate<object?> canExecute)
        {
            this._Execute = executeCommand;
            this._CanExecute = canExecute;
        }

        public RelayCommand(Action<object> executeCommand)
        {
            this._Execute = executeCommand;
        }

        public bool CanExecute(object parameter)
        {
            if (_CanExecute == null)
                return true;
            else
                return _CanExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _Execute(parameter);
        }
    }
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Marya.ViewModels;

namespace Marya.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Action _Command;
        private readonly Func<bool> _CanExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action command, Func<bool> canExecute = null)
        {
            _CanExecute = canExecute;
            _Command = command ?? throw new ArgumentNullException(nameof(command));
        }

        public void Execute(object parameter)
        {
            _Command();
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute == null || _CanExecute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

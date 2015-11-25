using System;
using System.Windows.Input;

namespace Framework.Commands
{
    public abstract class CommandBase : ICommand
    {
        #region Methods
        public virtual void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
        #endregion Methods

        #region ICommand Members
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);
        #endregion ICommand Members
    }
}

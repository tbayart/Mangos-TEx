using System;

namespace Framework.Commands
{
    public class DelegateCommand<T> : CommandBase
    {
        #region Fields
        private Action<T> _executeMethod;
        private Func<T, bool> _canExecuteMethod;
        #endregion Fields

        #region Ctor
        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, (o) => true)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if (executeMethod == null)
                throw new ArgumentNullException("executeMethod", "executeMethod cannot be null");

            if (canExecuteMethod == null)
                throw new ArgumentNullException("canExecuteMethod", "canExecuteMethod cannot be null");

            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }
        #endregion Ctor

        #region CommandBase
        public override void Execute(object parameter)
        {
            _executeMethod((T)parameter);
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecuteMethod((T)parameter);
        }
        #endregion CommandBase
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        #region Ctor
        public DelegateCommand(Action executeMethod)
            : this(executeMethod, () => true)
        {
        }

        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
            : base((o) => executeMethod(), (o) => canExecuteMethod())
        {
            if (executeMethod == null)
                throw new ArgumentNullException("executeMethod", "executeMethod cannot be null");

            if (canExecuteMethod == null)
                throw new ArgumentNullException("canExecuteMethod", "canExecuteMethod cannot be null");
        }
        #endregion Ctor
    }
}

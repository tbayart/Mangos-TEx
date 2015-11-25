using System.Windows.Input;
using Framework.Commands;

namespace Framework.Helpers
{
    public static class ICommandHelpers
    {
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            CommandBase commandBase = command as CommandBase;
            if (commandBase != null)
                commandBase.RaiseCanExecuteChanged();
        }
    }
}

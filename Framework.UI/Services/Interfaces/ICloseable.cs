using System.Windows.Input;

namespace Framework.Services.Interfaces
{
    public interface ICloseable
    {
        ICommand CloseCommand { get; set; }
    }
}

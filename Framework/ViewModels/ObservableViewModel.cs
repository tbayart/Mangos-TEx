namespace Framework.MVVM
{
    public class ObservableViewModel : ObservableObject, IViewModel
    {
        public ObservableViewModel()
        {
            InitializeCommands();
        }

        protected virtual void InitializeCommands()
        {
        }
    }
}

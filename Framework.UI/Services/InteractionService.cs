using System;
using System.Windows;
using System.Windows.Data;
using Framework.Commands;
using Framework.Services.Interfaces;

namespace Framework.Services
{
    public class InteractionService : IService
    {
        static InteractionService()
        {
            Uri resourceLocater = new Uri("/Framework.UI;component/Debug/ExceptionViewTemplate.xaml", UriKind.Relative);
            var dic = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            Application.Current.Resources.MergedDictionaries.Add(dic);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInfo(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowContent(object content)
        {
            var dialog = new Window
            {
                ResizeMode = ResizeMode.NoResize,
                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            if (Application.Current.MainWindow != null
            && Application.Current.MainWindow.IsLoaded == true)
            {
                dialog.Owner = Application.Current.MainWindow;
                dialog.ShowInTaskbar = false;
            }
            else // handle the cases when app crashes before main window is shown
            {
                dialog.Owner = null;
                dialog.ShowInTaskbar = true;
                Application.Current.MainWindow = dialog;
            }

            BindingOperations.SetBinding(dialog, Window.ContentProperty, new Binding());
            BindingOperations.SetBinding(dialog, Window.TitleProperty, new Binding("DisplayName"));
            BindingOperations.SetBinding(dialog, Window.WidthProperty, new Binding("Width"));
            BindingOperations.SetBinding(dialog, Window.HeightProperty, new Binding("Height"));

            if (content is IResizeable)
            {
                dialog.ResizeMode = ResizeMode.CanResizeWithGrip;
                dialog.MaxWidth = SystemParameters.WorkArea.Width;
                dialog.MaxHeight = SystemParameters.WorkArea.Height;
            }
            if (content is ICloseable)
            {
                ((ICloseable)content).CloseCommand = new DelegateCommand(() => dialog.Close());
            }

            dialog.DataContext = content;
            dialog.ShowDialog();
        }

        public void ShowException(Exception exception, string userMessage)
        {
            var content = new Framework.Debug.ExceptionViewModel(exception, userMessage);
            ShowContent(content);
        }
    }
}

using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;
using Framework.MVVM;
using Framework.Services;
using MangosTEx.ViewModels;

namespace MangosTEx
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // override the default value of the Language property
            // on all FrameworkElement inherited classes to the CurrentCulture
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement), new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.Name)));

            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            Application.Current.MainWindow = new MangosTEx.Views.MainWindow();
            Application.Current.MainWindow.Show();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
            e.Handled = true;
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject, true);
        }

        private static void HandleException(Exception ex)
        {
            HandleException(ex, false);
        }

        private static void HandleException(Exception ex, bool exitFailure)
        {
            TreatException(ex, "UnhandledException");

            if (exitFailure)
            {
                ServiceProvider.GetInstance<InteractionService>().ShowError("Erreur fatale, l'application va se fermer.");
                Environment.Exit(1);
            }
        }

        private static bool _abortNext = false;
        private static void TreatException(Exception exception, string userMessage)
        {
            // avoid displaying and sending cascading errors
            if (_abortNext == true)
                return;

            _abortNext = true;
            Action displayExceptionToUser = () =>
            {
                Console.WriteLine(exception.Message);
#if DEBUG
                ServiceProvider.GetInstance<InteractionService>().ShowException(exception, userMessage);
#else
                ServiceProvider.GetInstance<InteractionService>().ShowError(userMessage);
#endif
            };

            displayExceptionToUser();
            _abortNext = false;
        }
    }
}

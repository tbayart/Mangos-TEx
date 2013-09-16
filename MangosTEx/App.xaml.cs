using System;
using System.Net;
using System.Windows;

namespace MangosTEx
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var proxy = new WebProxy("proxy.elior.net", 8080);
            proxy.UseDefaultCredentials = true;
            WebRequest.DefaultWebProxy = proxy;
            base.OnStartup(e);
        }
    }
}

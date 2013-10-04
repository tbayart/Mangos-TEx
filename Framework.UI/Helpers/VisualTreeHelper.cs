using System.Windows;
using vth = System.Windows.Media.VisualTreeHelper;

namespace Framework.Helpers
{
    public static class VisualTreeHelper
    {
        public static T GetParent<T>(DependencyObject source)
            where T : DependencyObject
        {
            while (source is T == false && source != null)
                source = vth.GetParent(source);

            return source as T;
        }
    }
}

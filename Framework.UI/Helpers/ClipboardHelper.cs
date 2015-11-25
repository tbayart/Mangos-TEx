using System.Threading;
using System.Windows;

namespace Framework.Helpers
{
    public static class ClipboardHelper
    {
        public static IDataObject SafeGetClipboardDataObject()
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    return Clipboard.GetDataObject();
                }
                catch
                {
                }

                Thread.Sleep(100);
            }

            return null;
        }

        public static void SafeSetClipboardDataObject(object data)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    Clipboard.SetDataObject(data);
                    return;
                }
                catch
                {
                }

                Thread.Sleep(100);
            }
        }
    }
}

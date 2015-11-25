using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Framework.Helpers
{
    public static class FrameworkElementHelpers
    {
        public static Bitmap DesktopToBitmap()
        {
            // Windows desktop's total size
            Rectangle desktop = Screen.AllScreens
                .Select(o => o.Bounds)
                .Aggregate((a, b) => Rectangle.Union(a, b));

            // Create a bitmap to store the capture
            Bitmap image = new Bitmap(desktop.Width, desktop.Height);

            // User Graphics class to retrieve desktop and store it into the Bitmap
            using (Graphics g = Graphics.FromImage(image))
            {
                g.CopyFromScreen(desktop.Location, Point.Empty, desktop.Size);
            }

            return image;
        }
    }
}

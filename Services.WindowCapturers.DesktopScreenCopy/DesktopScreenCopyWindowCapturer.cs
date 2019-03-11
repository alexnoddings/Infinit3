using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AlexNoddings.Infinit3.Core.WindowCapturers;

namespace AlexNoddings.Infinit3.Services.WindowCapturers.DesktopScreenCopy
{
    public class DesktopScreenCopyWindowCapturer : IWindowCapturer
    {
        private const string ProcessName = "Tower-Win64-Shipping";

        public bool IsReady()
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out uint pid);
            // Check if current foreground window belongs to the target process
            return string.Equals(ProcessName, Process.GetProcessById((int) pid).ProcessName);
        }

        public Bitmap CaptureWindow()
        {
            if (!IsReady()) return null;

            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            var desktop = new Bitmap(screenBounds.Width, screenBounds.Height);

            using (Graphics g = Graphics.FromImage(desktop))
            {
                g.CopyFromScreen(screenBounds.X, screenBounds.Y, 0, 0, desktop.Size, CopyPixelOperation.SourceCopy);
            }

            return desktop;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
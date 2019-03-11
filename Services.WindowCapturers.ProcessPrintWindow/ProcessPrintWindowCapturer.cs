using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AlexNoddings.Infinit3.Core.WindowCapturers;

namespace AlexNoddings.Infinit3.Services.WindowCapturers.ProcessPrintWindow
{
    public class ProcessPrintWindowCapturer : IWindowCapturer
    {
        private const string ProcessName = "Tower-Win64-Shipping";

        public bool IsReady()
        {
            // Can capture as long as it can see the process
            return Process.GetProcessesByName(ProcessName).Length > 0;
        }

        public Bitmap CaptureWindow()
        {
            if (!IsReady()) return null;

            Process process = Process.GetProcessesByName(ProcessName)[0];
            IntPtr hwnd = process.MainWindowHandle;
            GetWindowRect(hwnd, out Rect windowRect);
            var window = new Bitmap(windowRect.Width, windowRect.Height, PixelFormat.Format32bppArgb);

            using (Graphics g = Graphics.FromImage(window))
            {
                IntPtr windowHdc = g.GetHdc();
                PrintWindow(hwnd, windowHdc, 0);
                g.ReleaseHdc(windowHdc);
            }

            return window;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [StructLayout(LayoutKind.Sequential)]
        private struct Rect
        {
            public Rect(Rect rectangle) : this(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom)
            {
            }

            public Rect(int left, int top, int right, int bottom)
            {
                X = left;
                Y = top;
                Right = right;
                Bottom = bottom;
            }

            public int X { get; set; }

            public int Y { get; set; }

            public int Left
            {
                get => X;
                set => X = value;
            }

            public int Top
            {
                get => Y;
                set => Y = value;
            }

            public int Right { get; set; }

            public int Bottom { get; set; }

            public int Height
            {
                get => Bottom - Y;
                set => Bottom = value + Y;
            }

            public int Width
            {
                get => Right - X;
                set => Right = value + X;
            }

            public Point Location
            {
                get => new Point(Left, Top);
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }

            public Size Size
            {
                get => new Size(Width, Height);
                set
                {
                    Right = value.Width + X;
                    Bottom = value.Height + Y;
                }
            }

            public static implicit operator Rectangle(Rect rectangle)
            {
                return new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height);
            }

            public static implicit operator Rect(Rectangle rectangle)
            {
                return new Rect(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            }

            public static bool operator ==(Rect rectangle1, Rect rectangle2)
            {
                return rectangle1.Equals(rectangle2);
            }

            public static bool operator !=(Rect rectangle1, Rect rectangle2)
            {
                return !rectangle1.Equals(rectangle2);
            }

            public override string ToString()
            {
                return "{Left: " + X + "; " + "Top: " + Y + "; Right: " + Right + "; Bottom: " + Bottom + "}";
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals(Rect rectangle)
            {
                return rectangle.Left == X && rectangle.Top == Y && rectangle.Right == Right &&
                       rectangle.Bottom == Bottom;
            }

            public override bool Equals(object Object)
            {
                switch (Object)
                {
                    case Rect rectangle:
                        return Equals(rectangle);
                    case Rectangle rectangle:
                        return Equals(new Rect(rectangle));
                    default:
                        return false;
                }
            }
        }
    }
}
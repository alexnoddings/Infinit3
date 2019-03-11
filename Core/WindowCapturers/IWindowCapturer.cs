using System.Drawing;

namespace AlexNoddings.Infinit3.Core.WindowCapturers
{
    public interface IWindowCapturer
    {
        bool IsReady();
        Bitmap CaptureWindow();
    }
}
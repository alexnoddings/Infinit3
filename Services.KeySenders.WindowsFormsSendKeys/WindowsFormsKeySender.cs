using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AlexNoddings.Infinit3.Core.KeySenders;

namespace AlexNoddings.Infinit3.Services.KeySenders.WindowsFormsSendKeys
{
    public class WindowsFormsKeySender : IKeySender
    {
        private const string ProcessName = "Tower-Win64-Shipping";

        public bool IsReady()
        {
            IntPtr hwnd = GetForegroundWindow();
            GetWindowThreadProcessId(hwnd, out uint pid);
            // Check if current foreground window belongs to the target process
            return string.Equals(ProcessName, Process.GetProcessById((int) pid).ProcessName);
        }

        public void SendChar(char character)
        {
            if (IsReady()) SendKeys.SendWait(character.ToString());
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
    }
}
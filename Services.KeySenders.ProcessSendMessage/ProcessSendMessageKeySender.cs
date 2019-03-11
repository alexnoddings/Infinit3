using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using AlexNoddings.Infinit3.Core.KeySenders;

namespace AlexNoddings.Infinit3.Services.KeySenders.ProcessSendMessage
{
    public class ProcessSendMessageKeySender : IKeySender
    {
        private const string ProcessName = "Tower-Win64-Shipping";

        private const uint MsgKeyDown = 0x0100;
        private const uint MsgKeyUp = 0x0101;
        private const uint MsgCharSent = 0x0102;
        private static readonly Random Rand;

        static ProcessSendMessageKeySender()
        {
            Rand = new Random();
        }

        public bool IsReady()
        {
            // Can send as long as it can see the process
            return Process.GetProcessesByName(ProcessName).Length > 0;
        }

        public void SendChar(char character)
        {
            if (!IsReady()) return;

            Process process = Process.GetProcessesByName(ProcessName)[0];
            SendMessage(process, MsgKeyDown, character);
            SendMessage(process, MsgCharSent, character);
            // Wait before sending key up
            Thread.Sleep(Rand.Next(17, 43));
            SendMessage(process, MsgKeyUp, character);
        }

        private static void SendMessage(Process process, uint message, char character)
        {
            SendMessage(process.MainWindowHandle, message, (IntPtr) char.ToUpper(character), IntPtr.Zero);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}
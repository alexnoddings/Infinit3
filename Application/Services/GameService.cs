using System;
using System.Drawing;
using System.Threading;
using AlexNoddings.Infinit3.Core.KeyRecognisers;
using AlexNoddings.Infinit3.Core.KeySenders;
using AlexNoddings.Infinit3.Core.WindowCapturers;
using AlexNoddings.Infinit3.Services.KeyRecognisers.IronOcr;
using AlexNoddings.Infinit3.Services.KeySenders.ProcessSendMessage;
using AlexNoddings.Infinit3.Services.KeySenders.WindowsFormsSendKeys;
using AlexNoddings.Infinit3.Services.WindowCapturers.DesktopScreenCopy;
using AlexNoddings.Infinit3.Services.WindowCapturers.ProcessPrintWindow;
using GalaSoft.MvvmLight;

namespace AlexNoddings.Infinit3.Application.Services
{
    public class GameService : ObservableObject
    {
        private readonly Random _random;
        
        private Thread _checkAfkThread;
        private Thread _interactionThread;

        private volatile bool _isRunning;
        private bool _isKeySenderReady;
        private bool _isWindowCapturerReady;
        private bool _canSeeKey;

        private IKeyRecogniser _keyRecogniser;
        private IKeySender _keySender;
        private IWindowCapturer _windowCapturer;

        public GameService()
        {
            _random = new Random();
        }

        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                RaisePropertyChanged();
            }
        }

        public bool IsKeySenderReady
        {
            get => _isKeySenderReady;
            set
            {
                if (_isKeySenderReady == value) return;
                _isKeySenderReady = value;
                RaisePropertyChanged();
            }
        }

        public bool IsWindowCapturerReady
        {
            get => _isWindowCapturerReady;
            set
            {
                if (_isWindowCapturerReady == value) return;
                _isWindowCapturerReady = value;
                RaisePropertyChanged();
            }
        }

        public bool CanSeeKey
        {
            get => _canSeeKey;
            set
            {
                if (_canSeeKey == value) return;
                _canSeeKey = value;
                RaisePropertyChanged();
            }
        }

        internal void Start(KeySenderSetting keySenderSetting, WindowCapturerSetting windowCapturerSetting)
        {
            _keyRecogniser = new IronOcrKeyRecogniser();

            switch (keySenderSetting)
            {
                case KeySenderSetting.ProcessSendMessage:
                    _keySender = new ProcessSendMessageKeySender();
                    break;
                case KeySenderSetting.WindowsFormsSendKeys:
                    _keySender = new WindowsFormsKeySender();
                    break;
                default:
                    throw new NotSupportedException($"Unknown KeySenderSetting {keySenderSetting}");
            }

            switch (windowCapturerSetting)
            {
                case WindowCapturerSetting.DesktopScreenCopy:
                    _windowCapturer = new DesktopScreenCopyWindowCapturer();
                    break;
                case WindowCapturerSetting.ProcessPrintWindow:
                    _windowCapturer = new ProcessPrintWindowCapturer();
                    break;
                default:
                    throw new NotSupportedException($"Unknown WindowCapturerSetting {windowCapturerSetting}");
            }

            IsRunning = true;

            if (_interactionThread != null && _interactionThread.IsAlive)
                _interactionThread.Abort();

            if (_checkAfkThread != null && _checkAfkThread.IsAlive)
                _checkAfkThread.Abort();

            _interactionThread = new Thread(Interact) {IsBackground = true};
            _interactionThread.Start();
            _checkAfkThread = new Thread(CheckAfk) {IsBackground = true};
            _checkAfkThread.Start();
        }

        internal void Stop()
        {
            IsRunning = false;
            IsKeySenderReady = false;
            IsWindowCapturerReady = false;
            CanSeeKey = false;
        }

        private void Interact()
        {
            while (_isRunning)
            {
                if (_keySender.IsReady())
                {
                    IsKeySenderReady = true;
                    _keySender.SendChar(' ');
                }
                else
                {
                    IsKeySenderReady = false;
                }

                Thread.Sleep(_random.Next(450, 750));
            }
        }

        private void CheckAfk()
        {
            while (_isRunning)
            {
                if (_windowCapturer.IsReady())
                {
                    IsWindowCapturerReady = true;
                    if (_keySender.IsReady())
                    {
                        IsKeySenderReady = true;
                        using (Bitmap window = _windowCapturer.CaptureWindow())
                        {
                            char c = _keyRecogniser.GetChar(window);
                            if (c != 0)
                            {
                                _keySender.SendChar(c);
                                CanSeeKey = true;
                            }
                            else
                            {
                                CanSeeKey = false;
                            }
                        }
                    }
                    else
                    {
                        IsKeySenderReady = false;
                    }
                }
                else
                {
                    IsWindowCapturerReady = false;
                }

                Thread.Sleep(_random.Next(1500, 3500));
            }
        }
    }
}
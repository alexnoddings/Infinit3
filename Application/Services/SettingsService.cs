using System;
using AlexNoddings.Infinit3.Application.Properties;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AlexNoddings.Infinit3.Application.Services
{
    internal enum WindowCapturerSetting
    {
        ProcessPrintWindow,
        DesktopScreenCopy
    }

    internal enum KeySenderSetting
    {
        WindowsFormsSendKeys,
        ProcessSendMessage
    }

    public class SettingsService : ObservableObject
    {
        private RelayCommand _discardCommand;
        private RelayCommand _saveCommand;

        private bool _shouldUseDesktopScreenCopy;
        private bool _shouldUseProcessPrintWindow;
        private bool _shouldUseProcessSendMessage;
        private bool _shouldUseWindowsFormsSendKeys;

        public SettingsService()
        {
            LoadPersistent();
            DiscardChanges();

            SaveCommand = new RelayCommand(SaveChanges);
            DiscardCommand = new RelayCommand(DiscardChanges);
        }

        internal WindowCapturerSetting WindowCapturer { get; private set; }
        internal KeySenderSetting KeySender { get; private set; }

        public bool ShouldUseProcessPrintWindow
        {
            get => _shouldUseProcessPrintWindow;
            set
            {
                if (_shouldUseProcessPrintWindow == value) return;
                _shouldUseProcessPrintWindow = value;
                RaisePropertyChanged();
            }
        }

        public bool ShouldUseDesktopScreenCopy
        {
            get => _shouldUseDesktopScreenCopy;
            set
            {
                if (_shouldUseDesktopScreenCopy == value) return;
                _shouldUseDesktopScreenCopy = value;
                RaisePropertyChanged();
            }
        }

        public bool ShouldUseWindowsFormsSendKeys
        {
            get => _shouldUseWindowsFormsSendKeys;
            set
            {
                if (_shouldUseWindowsFormsSendKeys == value) return;
                _shouldUseWindowsFormsSendKeys = value;
                RaisePropertyChanged();
            }
        }

        public bool ShouldUseProcessSendMessage
        {
            get => _shouldUseProcessSendMessage;
            set
            {
                if (_shouldUseProcessSendMessage == value) return;
                _shouldUseProcessSendMessage = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand SaveCommand
        {
            get => _saveCommand;
            set
            {
                if (_saveCommand == value) return;
                _saveCommand = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand DiscardCommand
        {
            get => _discardCommand;
            set
            {
                if (_discardCommand == value) return;
                _discardCommand = value;
                RaisePropertyChanged();
            }
        }

        private void DiscardChanges()
        {
            ShouldUseProcessPrintWindow = WindowCapturer == WindowCapturerSetting.ProcessPrintWindow;
            ShouldUseDesktopScreenCopy = WindowCapturer == WindowCapturerSetting.DesktopScreenCopy;
            ShouldUseWindowsFormsSendKeys = KeySender == KeySenderSetting.WindowsFormsSendKeys;
            ShouldUseProcessSendMessage = KeySender == KeySenderSetting.ProcessSendMessage;
        }

        private void SaveChanges()
        {
            if (ShouldUseProcessPrintWindow)
                WindowCapturer = WindowCapturerSetting.ProcessPrintWindow;
            if (ShouldUseDesktopScreenCopy)
                WindowCapturer = WindowCapturerSetting.DesktopScreenCopy;
            if (ShouldUseWindowsFormsSendKeys)
                KeySender = KeySenderSetting.WindowsFormsSendKeys;
            if (ShouldUseProcessSendMessage)
                KeySender = KeySenderSetting.ProcessSendMessage;
            SavePersistent();
        }

        private void LoadPersistent()
        {
            string windowCapturerString = Settings.Default.WindowCapturer;
            string keySenderString = Settings.Default.KeySender;

            bool windowCapturerParsed = Enum.TryParse(windowCapturerString, out WindowCapturerSetting windowCapturer);
            WindowCapturer = windowCapturerParsed ? windowCapturer : WindowCapturerSetting.ProcessPrintWindow;

            bool keySenderParsed = Enum.TryParse(keySenderString, out KeySenderSetting keySender);
            KeySender = keySenderParsed ? keySender : KeySenderSetting.ProcessSendMessage;

            // Save settings if loading one failed
            if (!windowCapturerParsed || !keySenderParsed) SavePersistent();
        }

        private void SavePersistent()
        {
            Settings.Default.WindowCapturer = WindowCapturer.ToString();

            Settings.Default.KeySender = KeySender.ToString();

            LoadPersistent();
        }
    }
}
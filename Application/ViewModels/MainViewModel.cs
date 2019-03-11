using AlexNoddings.Infinit3.Application.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace AlexNoddings.Infinit3.Application.ViewModels
{
    /// <summary>
    ///     This class contains properties that the Main View can data bind to.
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private GameService _gameService;

        private SettingsService _settingsService;

        private RelayCommand _startCommand;

        private RelayCommand _stopCommand;

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(SettingsService settingsService, GameService gameService)
        {
            SettingsService = settingsService;
            GameService = gameService;
            StartCommand = new RelayCommand(() =>
                GameService.Start(SettingsService.KeySender, SettingsService.WindowCapturer));
            StopCommand = new RelayCommand(GameService.Stop);
        }

        public SettingsService SettingsService
        {
            get => _settingsService;
            set
            {
                if (_settingsService == value) return;
                _settingsService = value;
                RaisePropertyChanged();
            }
        }

        public GameService GameService
        {
            get => _gameService;
            set
            {
                if (_gameService == value) return;
                _gameService = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand StartCommand
        {
            get => _startCommand;
            set
            {
                if (_startCommand == value) return;
                _startCommand = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand StopCommand
        {
            get => _stopCommand;
            set
            {
                if (_stopCommand == value) return;
                _stopCommand = value;
                RaisePropertyChanged();
            }
        }
    }
}
using Microsoft.UI.Xaml;
using NAudio.Wave;
using NAudioEqualizer.Commands;
using NAudioEqualizer.Models;
using NAudioEqualizer.Services.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace NAudioEqualizer.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        private readonly AudioModel _audioModel;
        private readonly IAudioService _audioService;
        private readonly IEqualizerService _equalizerService;
        private string? _selectedFilePath;
        private bool _isDragging;
        private bool _isMonitoring;
        private double _progressValue;
        private double _maxProgressValue;
        private string _currentTimeText = "00:00:00";
        private string _durationText = "00:00:00";
        private float _volume = 1.0f;
        private ObservableCollection<double> _equalizerValues;
        private Window? _window;
        private PlaybackState _currentPlaybackState = PlaybackState.Stopped;

        public MainWindowViewModel(IAudioService audioService, IEqualizerService equalizerService)
        {
            _audioService = audioService;
            _equalizerService = equalizerService;
            _audioModel = new AudioModel(_audioService);
            _equalizerValues = new ObservableCollection<double>(new double[_equalizerService.BandCount]);
            
            // Initialize equalizer values
            for (int i = 0; i < _equalizerValues.Count; i++)
            {
                _equalizerValues[i] = 0;
                _audioModel.SetEqualizerGain(i, 0f);
            }

            // Add equalizer values change handler
            _equalizerValues.CollectionChanged += (s, e) =>
            {
                if (e.NewItems != null && e.NewStartingIndex >= 0)
                {
                    _audioModel.SetEqualizerGain(e.NewStartingIndex, (float)_equalizerValues[e.NewStartingIndex]);
                }
            };

            PlayCommand = new RelayCommand(_ => Play(), _ => CanPlay);
            PauseCommand = new RelayCommand(_ => Pause(), _ => CanPause);
            StopCommand = new RelayCommand(_ => Stop(), _ => CanStop);
            SelectFileCommand = new RelayCommand(async _ => await SelectFile());
            ResetEqualizerCommand = new RelayCommand(_ => ResetEqualizer(), _ => true);

            StartPlaybackMonitor();
        }

        public void Initialize(Window window)
        {
            _window = window;
        }

        public ICommand PlayCommand { get; }
        public ICommand PauseCommand { get; }
        public ICommand StopCommand { get; }
        public ICommand SelectFileCommand { get; }
        public ICommand ResetEqualizerCommand { get; }

        public string? SelectedFilePath
        {
            get => _selectedFilePath;
            set
            {
                if (SetProperty(ref _selectedFilePath, value))
                {
                    OnPropertyChanged(nameof(HasLoadedFile));
                    UpdateCommandStates();
                }
            }
        }

        public double MaxProgressValue
        {
            get => _maxProgressValue;
            set => SetProperty(ref _maxProgressValue, value);
        }

        public double ProgressValue
        {
            get => _progressValue;
            set
            {
                if (SetProperty(ref _progressValue, Math.Min(value, MaxProgressValue)) && !_isDragging)
                {
                    _audioModel.SetPosition(TimeSpan.FromSeconds(ProgressValue));
                }
            }
        }

        public string CurrentTimeText
        {
            get => _currentTimeText;
            set => SetProperty(ref _currentTimeText, value);
        }

        public string DurationText
        {
            get => _durationText;
            set => SetProperty(ref _durationText, value);
        }

        public float Volume
        {
            get => _volume;
            set
            {
                if (SetProperty(ref _volume, value))
                {
                    _audioModel.Volume = value;
                }
            }
        }

        public ObservableCollection<double> EqualizerValues
        {
            get => _equalizerValues;
            set => SetProperty(ref _equalizerValues, value);
        }

        public bool HasLoadedFile => !string.IsNullOrEmpty(SelectedFilePath);
        public bool CanPlay => HasLoadedFile && _currentPlaybackState != PlaybackState.Playing;
        public bool CanPause => _currentPlaybackState == PlaybackState.Playing;
        public bool CanStop => _currentPlaybackState != PlaybackState.Stopped;

        private void UpdateCommandStates()
        {
            OnPropertyChanged(nameof(CanPlay));
            OnPropertyChanged(nameof(CanPause));
            OnPropertyChanged(nameof(CanStop));

            (PlayCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (PauseCommand as RelayCommand)?.RaiseCanExecuteChanged();
            (StopCommand as RelayCommand)?.RaiseCanExecuteChanged();
        }

        private void UpdatePlaybackState(PlaybackState newState)
        {
            if (_currentPlaybackState != newState)
            {
                _currentPlaybackState = newState;
                UpdateCommandStates();
            }
        }

        public void HandleDrop(StorageFile file)
        {
            LoadFile(file.Path);
        }

        public void SetProgressDragging(bool dragging)
        {
            _isDragging = dragging;
            if (!dragging)
            {
                var wasPlaying = _currentPlaybackState == PlaybackState.Playing;
                _audioModel.SetPosition(TimeSpan.FromSeconds(ProgressValue));
                if (wasPlaying)
                {
                    _audioModel.Play();
                }
            }
        }

        private async void StartPlaybackMonitor()
        {
            if (_isMonitoring) return;
            _isMonitoring = true;

            while (!_isDisposed)
            {
                var state = _audioModel.PlaybackState;
                UpdatePlaybackState(state);

                if (state == PlaybackState.Playing)
                {
                    UpdatePlaybackInfo();
                }
                await Task.Delay(50);
            }
        }

        private void UpdatePlaybackInfo()
        {
            if (!_isDragging)
            {
                ProgressValue = Math.Min(_audioModel.CurrentTime.TotalSeconds, MaxProgressValue);
            }
            CurrentTimeText = _audioModel.CurrentTime.ToString(@"hh\:mm\:ss");
            DurationText = _audioModel.Duration.ToString(@"hh\:mm\:ss");
        }

        private async Task SelectFile()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wav");

            if (_window != null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(_window);
                WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
            }

            StorageFile? file = await picker.PickSingleFileAsync();
            
            if (file != null)
            {
                LoadFile(file.Path);
            }
        }

        private void LoadFile(string filePath)
        {
            Stop();
            SelectedFilePath = filePath;
            _audioModel.LoadFile(filePath);
            MaxProgressValue = _audioModel.Duration.TotalSeconds;
            UpdatePlaybackInfo();
            UpdateCommandStates();
        }

        private void Play()
        {
            if (_currentPlaybackState == PlaybackState.Stopped && ProgressValue >= MaxProgressValue)
            {
                ProgressValue = 0;
                _audioModel.SetPosition(TimeSpan.Zero);
            }
            _audioModel.Play();
            UpdatePlaybackState(PlaybackState.Playing);
        }

        private void Pause()
        {
            _audioModel.Pause();
            UpdatePlaybackState(PlaybackState.Paused);
        }

        private void Stop()
        {
            _audioModel.Stop();
            ProgressValue = 0;
            CurrentTimeText = "00:00:00";
            UpdatePlaybackState(PlaybackState.Stopped);
        }

        private void ResetEqualizer()
        {
            _audioModel.ResetEqualizer();
            var newValues = new double[_equalizerValues.Count];
            for (int i = 0; i < _equalizerValues.Count; i++)
            {
                newValues[i] = 0;
            }
            EqualizerValues = new ObservableCollection<double>(newValues);
        }

        private bool _isDisposed;
        public void Dispose()
        {
            if (_isDisposed) return;

            _audioModel.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
} 
using NAudio.Wave;
using NAudioEqualizer.Services.Interfaces;
using System;

namespace NAudioEqualizer.Services
{
    public class AudioService : IAudioService
    {
        private readonly IEqualizerService _equalizerService;
        private WaveOutEvent? _outputDevice;
        private AudioFileReader? _audioFile;
        private AudioEqualizer? _equalizer; // Explicitly store equalizer
        private bool _isDisposed;
        private bool _isPaused;

        public bool IsPlaying => _outputDevice?.PlaybackState == PlaybackState.Playing;
        public bool IsPaused => _isPaused;
        public TimeSpan CurrentTime => _audioFile?.CurrentTime ?? TimeSpan.Zero;
        public TimeSpan Duration => _audioFile?.TotalTime ?? TimeSpan.Zero;
        public float Volume
        {
            get => _audioFile?.Volume ?? 1.0f;
            set
            {
                if (_audioFile != null)
                {
                    _audioFile.Volume = Math.Clamp(value, 0.0f, 1.0f);
                }
            }
        }

        public double[] EqualizerValues => _equalizerService.Gains;

        public AudioService(IEqualizerService equalizerService)
        {
            _equalizerService = equalizerService ?? throw new ArgumentNullException(nameof(equalizerService));
        }

        public void LoadFile(string filePath)
        {
            DisposeDevices();

            _audioFile = new AudioFileReader(filePath);
            _equalizer = new AudioEqualizer(_audioFile, _equalizerService); // Store equalizer reference
            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_equalizer);
            _isPaused = false;
        }

        public void Play()
        {
            if (_outputDevice == null) return;

            if (_outputDevice.PlaybackState == PlaybackState.Stopped && CurrentTime >= Duration)
            {
                _audioFile!.Position = 0;
            }
            _outputDevice.Play();
            _isPaused = false;
        }

        public void Pause()
        {
            _outputDevice?.Pause();
            _isPaused = true;
        }

        public void Stop()
        {
            _outputDevice?.Stop();
            if (_audioFile != null)
            {
                _audioFile.Position = 0;
            }
            _isPaused = false;
        }

        public void SetPosition(TimeSpan position)
        {
            if (_audioFile != null)
            {
                _audioFile.CurrentTime = position;
            }
        }

        public void SetEqualizerGain(int band, float gain)
        {
            _equalizerService.SetGain(band, gain);
            _equalizer?.SetGain(band, gain); // Update equalizer directly
        }

        public void ResetEqualizer()
        {
            _equalizerService.Reset();
            if (_equalizer != null)
            {
                for (int i = 0; i < _equalizerService.BandCount; i++)
                {
                    _equalizer.SetGain(i, 0.0f);
                }
            }
        }

        private void DisposeDevices()
        {
            if (_outputDevice != null)
            {
                _outputDevice.Stop();
                _outputDevice.Dispose();
                _outputDevice = null;
            }

            if (_audioFile != null)
            {
                _audioFile.Dispose();
                _audioFile = null;
            }

            _equalizer = null;
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            DisposeDevices();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
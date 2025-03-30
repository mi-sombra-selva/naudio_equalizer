using NAudio.Wave;
using NAudioEqualizer.Services.Interfaces;
using System;

namespace NAudioEqualizer.Models
{
    public class AudioModel : IDisposable
    {
        private readonly IAudioService _audioService;
        private bool _isDisposed;

        public TimeSpan Duration => _audioService.Duration;
        public TimeSpan CurrentTime => _audioService.CurrentTime;
        public float Volume
        {
            get => _audioService.Volume;
            set => _audioService.Volume = value;
        }

        public PlaybackState PlaybackState => _audioService.IsPlaying ? PlaybackState.Playing :
                                            _audioService.IsPaused ? PlaybackState.Paused :
                                            PlaybackState.Stopped;

        public AudioModel(IAudioService audioService)
        {
            _audioService = audioService;
        }

        public void LoadFile(string filePath)
        {
            _audioService.LoadFile(filePath);
        }

        public void Play()
        {
            _audioService.Play();
        }

        public void Pause()
        {
            _audioService.Pause();
        }

        public void Stop()
        {
            _audioService.Stop();
        }

        public void SetPosition(TimeSpan position)
        {
            _audioService.SetPosition(position);
        }

        public void SetEqualizerGain(int band, float gain)
        {
            _audioService.SetEqualizerGain(band, gain);
        }

        public void ResetEqualizer()
        {
            _audioService.ResetEqualizer();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _audioService.Dispose();
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
} 
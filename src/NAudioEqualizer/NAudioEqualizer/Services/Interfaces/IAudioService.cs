using NAudio.Wave;
using System;

namespace NAudioEqualizer.Services.Interfaces
{
    public interface IAudioService : IDisposable
    {
        bool IsPlaying { get; }
        bool IsPaused { get; }
        TimeSpan CurrentTime { get; }
        TimeSpan Duration { get; }
        float Volume { get; set; }
        double[] EqualizerValues { get; }

        void LoadFile(string filePath);
        void Play();
        void Pause();
        void Stop();
        void SetPosition(TimeSpan position);
        void SetEqualizerGain(int band, float gain);
        void ResetEqualizer();
    }
} 
using System;

namespace NAudioEqualizer.Services.Interfaces
{
    public interface IEqualizerService
    {
        int BandCount { get; }
        double[] Gains { get; }
        float[] Frequencies { get; }

        void SetGain(int band, float gain);
        void Reset();
    }
} 
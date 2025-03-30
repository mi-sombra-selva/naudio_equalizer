using NAudio.Wave;
using NAudio.Dsp;
using NAudioEqualizer.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace NAudioEqualizer
{
    public class AudioEqualizer : ISampleProvider
    {
        private readonly ISampleProvider _sourceProvider;
        private readonly IEqualizerService _equalizerService;
        private readonly List<BiQuadFilter> _filters;

        public WaveFormat WaveFormat => _sourceProvider.WaveFormat;

        public AudioEqualizer(ISampleProvider sourceProvider, IEqualizerService equalizerService)
        {
            _sourceProvider = sourceProvider ?? throw new ArgumentNullException(nameof(sourceProvider));
            _equalizerService = equalizerService ?? throw new ArgumentNullException(nameof(equalizerService));
            _filters = new List<BiQuadFilter>();

            // Initialize filters based on EqualizerService frequencies
            for (int i = 0; i < _equalizerService.BandCount; i++)
            {
                var filter = BiQuadFilter.PeakingEQ(
                    sourceProvider.WaveFormat.SampleRate,
                    _equalizerService.Frequencies[i],
                    1.0f, // Increased Q for better frequency separation
                    (float)_equalizerService.Gains[i]); // Use initial gain from service
                _filters.Add(filter);
            }
        }

        public void SetGain(int band, float gain)
        {
            if (band >= 0 && band < _equalizerService.BandCount)
            {
                _equalizerService.SetGain(band, gain);
                _filters[band].SetPeakingEq(
                    _sourceProvider.WaveFormat.SampleRate,
                    _equalizerService.Frequencies[band],
                    1.0f, // Consistent Q value
                    gain);
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = _sourceProvider.Read(buffer, offset, count);
            if (samplesRead == 0) return 0;

            int channels = WaveFormat.Channels; // Get number of channels (1 for mono, 2 for stereo)

            // Process samples based on channel count
            for (int i = 0; i < samplesRead; i += channels)
            {
                for (int channel = 0; channel < channels; channel++)
                {
                    int index = offset + i + channel;
                    float sample = buffer[index];
                    foreach (var filter in _filters)
                    {
                        sample = filter.Transform(sample);
                    }
                    buffer[index] = Math.Max(-1.0f, Math.Min(1.0f, sample));
                }
            }

            return samplesRead;
        }
    }
}
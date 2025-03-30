using NAudioEqualizer.Services.Interfaces;

namespace NAudioEqualizer.Services
{
    public class EqualizerService : IEqualizerService
    {
        private readonly float[] _frequencies = { 60, 170, 310, 600, 1000, 3000, 6000, 12000, 14000, 16000 };
        private readonly double[] _gains;

        public int BandCount => _frequencies.Length;
        public double[] Gains => _gains;
        public float[] Frequencies => _frequencies;

        public EqualizerService()
        {
            _gains = new double[_frequencies.Length];
            Reset();
        }

        public void SetGain(int band, float gain)
        {
            if (band >= 0 && band < _frequencies.Length)
            {
                _gains[band] = gain;
            }
        }

        public void Reset()
        {
            for (int i = 0; i < _frequencies.Length; i++)
            {
                _gains[i] = 0;
            }
        }
    }
}
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using Windows.UI;

namespace NAudioEqualizer
{
    public sealed partial class EqualizerVisualizer : UserControl
    {
        private readonly List<Rectangle> bars;
        private readonly float[] frequencies = { 60, 170, 310, 600, 1000, 3000, 6000, 12000, 14000, 16000 };
        private readonly LinearGradientBrush gradientBrush;

        public EqualizerVisualizer()
        {
            this.InitializeComponent();
            bars = new List<Rectangle>();
            
            // Create gradient brush
            gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Windows.Foundation.Point(0, 0);
            gradientBrush.EndPoint = new Windows.Foundation.Point(0, 1);
            
            var gradientStop1 = new GradientStop
            {
                Color = Color.FromArgb(255, 0, 120, 215),
                Offset = 0
            };
            
            var gradientStop2 = new GradientStop
            {
                Color = Color.FromArgb(255, 0, 99, 177),
                Offset = 1
            };
            
            gradientBrush.GradientStops.Add(gradientStop1);
            gradientBrush.GradientStops.Add(gradientStop2);

            InitializeBars();
        }

        private void InitializeBars()
        {
            // Clear only bars, keep grid
            foreach (var bar in bars)
            {
                VisualizerCanvas.Children.Remove(bar);
            }
            bars.Clear();

            // Positions for bars (aligned with vertical lines)
            double[] positions = { 30, 65, 100, 135, 170, 205, 240, 275, 310, 345 };
            double barWidth = 25;

            for (int i = 0; i < frequencies.Length; i++)
            {
                var bar = new Rectangle
                {
                    Fill = gradientBrush,
                    Width = barWidth,
                    Height = 80,
                    RadiusX = 2,
                    RadiusY = 2
                };

                Canvas.SetLeft(bar, positions[i] - barWidth/2);
                Canvas.SetTop(bar, 80 - bar.Height);

                bars.Add(bar);
                VisualizerCanvas.Children.Add(bar);
            }
        }

        public void UpdateVisualization(float[] gains)
        {
            if (gains == null || gains.Length != bars.Count) return;

            for (int i = 0; i < bars.Count; i++)
            {
                float gain = gains[i];
                float normalizedHeight = (gain + 10) / 20 * 80; // Normalize from -10..10 to 0..80
                bars[i].Height = Math.Max(1, normalizedHeight);
                Canvas.SetTop(bars[i], 80 - bars[i].Height);
            }
        }
    }
} 
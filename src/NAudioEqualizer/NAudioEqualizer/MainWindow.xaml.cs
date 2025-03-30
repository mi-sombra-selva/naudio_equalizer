using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using NAudioEqualizer.Services;
using NAudioEqualizer.Services.Interfaces;
using NAudioEqualizer.ViewModels;
using Windows.Storage;
using System;

namespace NAudioEqualizer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }

        public MainWindow()
        {
            // Create services
            IEqualizerService equalizerService = new EqualizerService();
            IAudioService audioService = new AudioService(equalizerService);

            // Create and initialize ViewModel
            ViewModel = new MainWindowViewModel(audioService, equalizerService);
            this.InitializeComponent();
            ViewModel.Initialize(this);
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            ViewModel.Dispose();
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
        }

        private async void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var file = items[0] as StorageFile;
                    if (file != null)
                    {
                        ViewModel.HandleDrop(file);
                    }
                }
            }
        }

        private void ProgressSlider_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.SetProgressDragging(true);
        }

        private void ProgressSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.SetProgressDragging(false);
        }

        private void ProgressSlider_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.SetProgressDragging(false);
        }

        private void ProgressSlider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.SetProgressDragging(false);
        }
    }
}

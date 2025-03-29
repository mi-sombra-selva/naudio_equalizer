using Microsoft.UI.Xaml;
using NAudio.Wave;
using Windows.Storage.Pickers;
using Windows.Storage;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace NAudioEqualizer
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private string selectedFilePath;
        private bool isPaused;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".wav");

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                selectedFilePath = file.Path;
                FilePathText.Text = selectedFilePath;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath)) return;

            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                audioFile = new AudioFileReader(selectedFilePath);
                outputDevice.Init(audioFile);
            }

            if (isPaused)
            {
                outputDevice.Play();
                isPaused = false;
            }
            else
            {
                outputDevice.Play();
            }
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (outputDevice?.PlaybackState == PlaybackState.Playing)
            {
                outputDevice?.Pause();
                isPaused = true;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();
            outputDevice = null;
            audioFile = null;
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
                        selectedFilePath = file.Path;
                        FilePathText.Text = selectedFilePath;
                    }
                }
            }
        }
    }
}

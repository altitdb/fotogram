using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FotogramClient.Camera
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PhotoCapturePage
    {
        private CameraCapture cameraCapture;

        public PhotoCapturePage()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Init and show preview
            cameraCapture = new CameraCapture();
            PreviewElement.Source = await cameraCapture.Initialize();
            await cameraCapture.StartPreview();
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Release resources
            if (cameraCapture != null)
            {
                await cameraCapture.StopPreview();
                PreviewElement.Source = null;
                cameraCapture.Dispose();
                cameraCapture = null;
            }
        }

        private async void BtnCapturePhoto_Click(object sender, RoutedEventArgs e)
        {
            // Take snapshot and add to ListView
            // Disable button to prevent exception due to parallel capture usage
            BtnCapturePhoto.IsEnabled = false;
            var photoStorageFile = await cameraCapture.CapturePhoto();

            var bitmap = new BitmapImage();
            await bitmap.SetSourceAsync(await photoStorageFile.OpenReadAsync());
            //PhotoListView.Items.Add(bitmap);
            //BtnCapturePhoto.IsEnabled = true;

            var wb = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            await wb.SetSourceAsync(await photoStorageFile.OpenReadAsync());

            PhotoListView.Items.Add(wb);
            BtnCapturePhoto.IsEnabled = true;
        }

        private void PhotoListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var lst = sender as ListView;

            if (lst != null)
            {
                var img = lst.SelectedItem as WriteableBitmap;

                Frame.Navigate(typeof (NovaPostagem), img);
            }
        }
    }
}

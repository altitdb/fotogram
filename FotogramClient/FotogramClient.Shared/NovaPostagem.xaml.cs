using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using FotogramClient.Camera;
using FotogramClient.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FotogramClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NovaPostagem
    {
        private readonly Postagem _postagens = new Postagem();

        private WriteableBitmap _image;

        public NovaPostagem()
        {
            this.InitializeComponent();

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _image = e.Parameter as WriteableBitmap;

            if (_image != null)
            {
                ImgFoto.Source = _image;
            }

            base.OnNavigatedTo(e);
        }

        private void BtnCapturar_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PhotoCapturePage));
        }

        private async void BtnPostar_Click(object sender, RoutedEventArgs e)
        {
            var msgErro = string.Empty;

            try
            {
                var novo = new NovaPostagemViewModel
                {
                    Texto = TxtTexto.Text,
                    Longitude = "",
                    Latitude = "",
                    Local = "",
                    Imagem = ConvertImgSourceToArray(ImgFoto)
                };

                await _postagens.NovoPost(novo);
            }
            catch (Exception ex)
            {
                msgErro = string.IsNullOrWhiteSpace(ex.Message) ? ex.InnerException.Message : ex.Message;
            }

            if (!string.IsNullOrWhiteSpace(msgErro))
            {
                var dialog = new MessageDialog(msgErro, "Ooooopss...");
                await dialog.ShowAsync();
            }
        }

        private string ConvertImgSourceToArray(Image img)
        {
            var imgBase64 = string.Empty;
            
            //var bmp = img.Source as WriteableBitmap;

            

            if (_image != null)
            {
                using (Stream stream = _image.PixelBuffer.AsStream())
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    imgBase64 = Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            
            return imgBase64;
        }
    }
}

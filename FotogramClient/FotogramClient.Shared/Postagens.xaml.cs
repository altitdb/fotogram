using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FotogramClient.Models;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace FotogramClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Postagens
    {
        private readonly Postagem _postagens = new Postagem();

        public Postagens()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            var postagens = await _postagens.ListaPostagens();
            Lista.ItemsSource = postagens;
        }

        private void BtnNovoPost_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof (NovaPostagem));
        }

        private void BtnPesquisar_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLogoff_OnClick(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSobre_OnClick(object sender, RoutedEventArgs e)
        {

        }
    }
}

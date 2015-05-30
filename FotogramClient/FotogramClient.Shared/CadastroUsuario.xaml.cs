using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class CadastroUsuario : Page
    {
        public CadastroUsuario()
        {
            this.InitializeComponent();
        }

        private async void BtnCadastro_Click(object sender, RoutedEventArgs e)
        {
            var novoCadastro = new NovoUsuario
            {
                NomeUsuario = TxTNomeUsuario.Text,
                NomeCompleto = TxTNomeCompleto.Text,
                DataNascimento = DtpDataNascimento.Date.DateTime,
                Email = TxtEmail.Text,
                Senha = TxtSenha.Password,
                ConfirmacaoSenha = TxtConfirmacaoSenha.Password
            };

            var msgErro = string.Empty;

            try
            {
                var user = new Usuario();

                user = await user.CadastroUsuario(novoCadastro);

                if (user == null)
                {
                    throw new Exception("erro desconhecido!");
                }

                Frame.Navigate(typeof (Postagens));
            }
            catch (Exception ex)
            {
                msgErro = string.IsNullOrWhiteSpace(ex.Message)
                    ? ex.InnerException.Message
                    : ex.Message;
            }

            if (!string.IsNullOrWhiteSpace(msgErro))
            {
                var dialog = new MessageDialog(msgErro, "Ooooopsss...");

                await dialog.ShowAsync();
            }
        }
    }
}

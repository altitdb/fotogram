using FotogramClient.Models;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace FotogramClient
{
    public partial class MainPage
    {
        private readonly Funcoes _func = new Funcoes();
        private readonly Usuario _usuario = new Usuario();

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await Login(true);
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = TxtUsuario.Text;
            var password = TxtPassword.Password;

            await Login(false);
        }

        private void BtnCadastro_Click(object sender, RoutedEventArgs e)
        {

        }

        internal async Task Login(bool autoLogin)
        {
            var msgErro = "";

            try
            {
                var userName = TxtUsuario.Text;
                var password = TxtPassword.Password;

                if (autoLogin)
                {
                    userName = _func.GetValuesOnLocalStorage("Username") != null ? _func.GetValuesOnLocalStorage("Username").ToString() : null;
                    password = _func.GetValuesOnLocalStorage("Password") != null ? _func.GetValuesOnLocalStorage("Password").ToString() : null;
                }

                if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(password))
                {
                    try
                    {
                        var token = await _usuario.GetToken(userName, password);

                        if (string.IsNullOrWhiteSpace(token))
                        {
                            throw new Exception("Token vazio! Por favor tente novamente!");
                        }
                        
                        await _usuario.GetUsuarioFromServer();

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (!(string.IsNullOrWhiteSpace(TxtPassword.Password) &&
                             string.IsNullOrWhiteSpace(TxtUsuario.Text)))
                            {
                                _func.SaveUserNameAndPassword(TxtUsuario.Text, TxtPassword.Password);
                            }

                            TxtUsuario.Text = string.Empty;
                            TxtPassword.Password = string.Empty;

                            Frame.Navigate(typeof(Postagens));
                        });
                    }
                    catch (Exception ex)
                    {
                        msgErro = string.IsNullOrWhiteSpace(ex.Message) ? ex.InnerException.Message : ex.Message;
                    }
                }
                else
                {
                    msgErro = "Os campos nome de usuário e senha são obrigatórios!";
                }

                if (!string.IsNullOrWhiteSpace(msgErro) && !autoLogin)
                {
                    var dialog = new MessageDialog(msgErro, "Oooopsss...");

                    await dialog.ShowAsync();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

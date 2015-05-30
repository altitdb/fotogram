using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FotogramClient.Models
{
    public class Usuario
    {
        public string NomeCompleto { get; set; }
        public string NomeUsuario { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Biografia { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public int Seguindo { get; set; }
        public int Seguidores { get; set; }

        private readonly Funcoes _func = new Funcoes();

        public async Task<string> GetToken(string nomeUsuario, string senha)
        {
            var client = new HttpClient { BaseAddress = new Uri(App.UrlBase) };

            client
                .DefaultRequestHeaders
                .Add("Accept", "application/json");

            var content =
                string.Format("grant_type=password&username={0}&password={1}",
                    nomeUsuario, senha);

            var response =
                await client
                    .PostAsync("api/acesso/token",
                    new StringContent(content));

            response.EnsureSuccessStatusCode();

            var tokenObject = await response.Content.ReadAsStringAsync();

            var token = JsonConvert.DeserializeObject<Token>(tokenObject);

            return token.access_token;
        }

        public async Task GetUsuarioFromServer()
        {
            var token = _func.GetValuesOnLocalStorage("token");

            var client = new HttpClient { BaseAddress = new Uri(App.UrlBase) };

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.GetAsync("api/usuario/meusdados");

            response.EnsureSuccessStatusCode();

            var usuario = await response.Content.ReadAsStringAsync();

            _func.SaveOrUpdateOnLocalStorage("Usuario", usuario);
        }

        public Usuario GetUsuarioFromLocal()
        {
            var usuarioLocalJson = _func.GetValuesOnLocalStorage("Usuario").ToString();
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioLocalJson);
            return usuario;
        }
    }

    public class Token
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string userName { get; set; }
    }
}

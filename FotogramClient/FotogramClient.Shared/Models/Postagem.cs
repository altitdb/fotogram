using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FotogramClient.Models
{
    public class Postagem
    {
        private readonly Funcoes _func = new Funcoes();

        public async Task<IEnumerable<VisualizacaoPostagemViewModel>> ListaPostagens()
        {
            var token = _func.GetValuesOnLocalStorage("token");

            var client = new HttpClient { BaseAddress = new Uri(App.UrlBase) };

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var response = await client.GetAsync("api/postagem/listar");

            response.EnsureSuccessStatusCode();

            var postagens =
                JsonConvert.DeserializeObject<IEnumerable<VisualizacaoPostagemViewModel>>(await response.Content.ReadAsStringAsync());

            return postagens;
        }

        public async Task NovoPost(NovaPostagemViewModel post)
        {
            var token = _func.GetValuesOnLocalStorage("token");

            var client = new HttpClient { BaseAddress = new Uri(App.UrlBase) };

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

            var jsonObj = JsonConvert.SerializeObject(post);

            var dataToPost = new StringContent(jsonObj, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/postagem/nova", dataToPost);

            response.EnsureSuccessStatusCode();

            var postagens =
                JsonConvert.DeserializeObject<IEnumerable<NovaPostagemViewModel>>(await response.Content.ReadAsStringAsync());

            //return postagens;
        }
    }

    public class NovaPostagemViewModel
    {
        /// <summary>
        /// Identificador do usuário
        /// </summary>
        public int UsuarioModelId { get; set; }

        /// <summary>
        /// Imagem da postagem
        /// </summary>
        public string Imagem { get; set; }

        /// <summary>
        /// Texto da postagem
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Descrição do local, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        public string Local { get; set; }

        /// <summary>
        /// Latitude, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Longitude, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        public string Longitude { get; set; }
    }

    public class VisualizacaoPostagemViewModel
    {
        /// <summary>
        /// Identificador da Postagem
        /// </summary>
        public int PostagemId { get; set; }

        /// <summary>
        /// Identificador do usuário
        /// </summary>
        public string NomeUsuario { get; set; }

        /// <summary>
        /// Imagem da postagem
        /// </summary>
        public string Imagem { get; set; }

        /// <summary>
        /// Texto da postagem
        /// </summary>
        public string Texto { get; set; }

        /// <summary>
        /// Descrição do local, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        public string Local { get; set; }

        /// <summary>
        /// Latitude, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// Longitude, de acordo com o mapa (bing, nokia, google)
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// Quantidade de curtidas da postagem
        /// </summary>
        public int Curtidas { get; set; }

        /// <summary>
        /// QuantidadeComentarios
        /// </summary>
        public int QuantidadeComentarios { get; set; }
    }
}

using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Fotogram.Models;
using Fotogram.Services;

namespace Fotogram.Controllers.Api
{
    /// <summary>
    /// Classe responsável pelas postagens
    /// </summary>
    [Authorize]
    public class PostagemController : ApiController
    {
        readonly FotogramContextDb _db = new FotogramContextDb();

        /// <summary>
        /// Carrega as postagens do usuário (necessário autenticação)
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetPostagens()
        {
            var model = await _db.PostagemModel
                .Where(w =>
                    w.Usuario.NomeUsuario == User.Identity.Name)
                .Select(s => new VisualizacaoPostagemViewModel
                {
                    PostagemId = s.Id,
                    NomeUsuario = s.Usuario.NomeUsuario,
                    Imagem = s.ImagemUrl,
                    Local = s.Local,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Texto = s.Texto
                })
                .ToListAsync();

            return Ok(model);
        }

        public async Task<IHttpActionResult> PostPostagem(NovaPostagemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioLogado = await
                _db.UsuarioModel
                    .FirstOrDefaultAsync(f => 
                    f.NomeUsuario == User.Identity.Name);

            var path = HttpContext.Current.Server.MapPath("~/Images/Uploads/");

            var postagem = new PostagemModel
            {
                UsuarioModelId = usuarioLogado.Id,
                DataAtualizacao = DateTime.UtcNow.AddHours(-3),
                DataPostagem = DateTime.UtcNow.AddHours(-3),
                ImagemUrl = ImageService.SalvarImagemNoServidor(usuarioLogado.NomeUsuario, path, model.Imagem),
                Texto = model.Texto,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Local = model.Local
            };

            _db.PostagemModel.Add(postagem);

            await _db.SaveChangesAsync();

            model.Imagem = postagem.ImagemUrl;

            return Ok(model);
        }
    }
}

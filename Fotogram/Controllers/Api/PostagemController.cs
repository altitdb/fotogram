using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
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
        /// <returns>Lista de VisualizacaoPostagemViewModel</returns>
        [ResponseType(typeof(VisualizacaoPostagemViewModel))]
        [Route("api/postagem/listar/")]
        public async Task<IHttpActionResult> GetPostagens()
        {
            var seguindo = await _db.SeguindoModel
                .Where(w => w.UsuarioSeguidor.NomeUsuario == User.Identity.Name)
                .Select(s => s.UsuarioSeguidoId)
                .ToListAsync();

            var urlBase = string.Format("{0}://{1}", 
                HttpContext.Current.Request.Url.Scheme,
                HttpContext.Current.Request.Url.Authority);

            var model = await _db.PostagemModel
                .Where(w =>
                    w.Usuario.NomeUsuario == User.Identity.Name ||
                    seguindo.Contains(w.UsuarioModelId))
                .Select(s => new VisualizacaoPostagemViewModel
                {
                    PostagemId = s.Id,
                    NomeUsuario = s.Usuario.NomeUsuario,
                    Imagem = urlBase + s.ImagemUrl,
                    Local = s.Local,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    Texto = s.Texto,
                    Curtidas = s.Curtidas.Count,
                    QuantidadeComentarios = s.Comentarios.Count
                })
                .ToListAsync();

            return Ok(model);
        }

        /// <summary>
        /// Carrega as postagens de um determinado usuário (necessário autenticação)
        /// </summary>
        /// <param name="nomeusuario">string</param>
        /// <returns>Lista de VisualizacaoPostagemViewModel</returns>
        [ResponseType(typeof(VisualizacaoPostagemViewModel))]
        [Route("api/postagem/listar/")]
        public async Task<IHttpActionResult> GetPostagens(string nomeusuario)
        {
            if (await _db.UsuarioModel.CountAsync(c => c.NomeUsuario == nomeusuario) > 0)
            {
                var seguindo = await _db.SeguindoModel
                .Where(w => w.UsuarioSeguidor.NomeUsuario == User.Identity.Name)
                .Select(s => s.UsuarioSeguidoId)
                .ToListAsync();

                var urlBase = string.Format("{0}://{1}",
                    HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.Authority);

                var model = await _db.PostagemModel
                    .Where(w =>
                        w.Usuario.NomeUsuario == User.Identity.Name)
                    .Select(s => new VisualizacaoPostagemViewModel
                    {
                        PostagemId = s.Id,
                        NomeUsuario = s.Usuario.NomeUsuario,
                        Imagem = urlBase + s.ImagemUrl,
                        Local = s.Local,
                        Latitude = s.Latitude,
                        Longitude = s.Longitude,
                        Texto = s.Texto,
                        Curtidas = s.Curtidas.Count
                    })
                    .ToListAsync();

                return Ok(model);
            }
            else
            {
                return BadRequest("Usuário não encontrado!");
            }
        }

        /// <summary>
        /// Carrega apenas uma postagem para visualização (necessário autenticação)
        /// </summary>
        /// <param name="id">Identificação da postagem</param>
        /// <returns>VisualizacaoPostagemViewModel</returns>
        [ResponseType(typeof (VisualizacaoPostagemViewModel))]
        [Route("api/postagem/visualizar/", Name = "Visualizar")]
        public async Task<IHttpActionResult> GetPostagem(int id)
        {
            if (id > 0)
            {
                var model = await _db.PostagemModel.FindAsync(id);

                if (model == null)
                {
                    return BadRequest("Postagem não encotrada!");
                }

                var urlBase = string.Format("{0}://{1}",
                    HttpContext.Current.Request.Url.Scheme,
                    HttpContext.Current.Request.Url.Authority);

                return Ok(new VisualizacaoPostagemViewModel
                {
                    PostagemId = model.Id,
                    NomeUsuario = model.Usuario.NomeUsuario,
                    Longitude = model.Longitude,
                    Local = model.Local,
                    Latitude = model.Latitude,
                    Texto = model.Texto,
                    Curtidas = model.Curtidas.Count,
                    Imagem = urlBase + model.ImagemUrl
                });
            }
            else
            {
                return BadRequest("Postagem não encontrada!");
            }
        }

        /// <summary>
        /// Faz o upload de uma imagem (necessário autenticação)
        /// </summary>
        /// <param name="model">NovaPostagemViewModel</param>
        /// <returns>NovaPostagemViewModel</returns>
        [ResponseType(typeof(NovaPostagemViewModel))]
        [Route("api/postagem/nova/")]
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
            
            return CreatedAtRoute("Visualizar", new { nomeUsuario = postagem.Id }, model);
        }

        /// <summary>
        /// Curte uma postagem. Se a postagem já foi curtida, desfaz o curtir (necessário autenticação)
        /// </summary>
        /// <param name="id">int</param>
        /// <returns>Ok</returns>
        [Route("api/postagem/curtir/")]
        public async Task<IHttpActionResult> PostCurtir(int id)
        {
            var postagem = await _db.PostagemModel.FindAsync(id);

            if (postagem == null)
            {
                return BadRequest("Postagem não encontrada");
            }

            var curtidaModel = await _db.CurtidaModel.FirstOrDefaultAsync(f => f.PostagemModelId == id);

            if (curtidaModel != null)
            {
                _db.CurtidaModel.Remove(curtidaModel);
            }
            else
            {
                var usuario = await _db.UsuarioModel.FirstOrDefaultAsync(f => f.NomeUsuario == User.Identity.Name);

                curtidaModel = new CurtidaModel
                {
                    PostagemModelId = id,
                    UsuarioModelId = usuario.Id,
                    DataAtualizacao = DateTime.UtcNow.AddHours(-3)
                };

                _db.CurtidaModel.Add(curtidaModel);
            }

            await _db.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Grava um comentário do usuário (necessário autenticação)
        /// </summary>
        /// <param name="model">ComentarioViewModel</param>
        /// <returns>ComentarioViewModel</returns>
        [ResponseType(typeof(ComentarioViewModel))]
        [Route("api/postagem/novocomentario/")]
        public async Task<IHttpActionResult> PostComentario(ComentarioViewModel model)
        {
            if (await _db.PostagemModel.CountAsync(c => c.Id == model.PostagemModelId) == 0)
            {
                return BadRequest("Postagem não encontrada!");
            }

            var usuario = await _db.UsuarioModel.FirstOrDefaultAsync(f => f.NomeUsuario == User.Identity.Name);

            var comentario = new ComentarioModel
            {
                DataAtualizacao = DateTime.UtcNow.AddHours(-3),
                PostagemModelId = model.PostagemModelId,
                UsuarioModelId = usuario.Id,
                Texto = model.Texto
            };

            _db.ComentarioModel.Add(comentario);

            await _db.SaveChangesAsync();
            
            return Ok();
        }

        /// <summary>
        /// Carrega todos os comentários de acordo com a postagem (necessário autenticação)
        /// </summary>
        /// <param name="postagemId"></param>
        /// <returns></returns>
        [ResponseType(typeof(ComentarioVisualizacaoViewModel))]
        [Route("api/postagem/visualizarcomentarios/")]
        public async Task<IHttpActionResult> GetComentarios(int postagemId)
        {
            if (await _db.PostagemModel.CountAsync(c => c.Id == postagemId) == 0)
            {
                return BadRequest("Postagem não encontrada!");
            }

            var comentarios = await _db.ComentarioModel
                .Where(w => w.PostagemModelId == postagemId)
                .Select(s => new ComentarioVisualizacaoViewModel
                {
                    PostagemModelId = s.PostagemModelId,
                    NomeUsuario = s.Usuario.NomeUsuario,
                    Texto = s.Texto,
                    Data = s.DataAtualizacao
                })
                .ToListAsync();
            
            return Ok(comentarios);
        }
    }
}

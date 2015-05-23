using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Fotogram.Models;
using Fotogram.Providers;
using Fotogram.Security;
using Fotogram.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

namespace Fotogram.Controllers.Api
{
    /// <summary>
    /// Acesso do Usuário
    /// </summary>
    [Authorize]
    public class UsuarioController : ApiController
    {
        private readonly FotogramContextDb _db = new FotogramContextDb();

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        /// <summary>
        /// Carrega os dados do usuário de acordo com o nome de usuário
        /// </summary>
        /// <param name="nomeUsuario"></param>
        /// <returns>Ok (200)</returns>
        [AllowAnonymous]
        [ResponseType(typeof(VisualizacaoUsuarioViewModel))]
        [Route("api/usuario/pesquisa/", Name = "Pesquisa")]
        public async Task<IHttpActionResult> GetUsuarioModel(string nomeUsuario)
        {
            if (string.IsNullOrWhiteSpace(nomeUsuario))
            {
                return BadRequest("Nome de usuário é obrigatório para a pesquisa!");
            }

            var usuarioModel = await _db.UsuarioModel.FirstOrDefaultAsync(f => f.NomeUsuario == nomeUsuario);

            if (usuarioModel == null)
            {
                return NotFound();
            }

            var seguindo = await _db.SeguindoModel.CountAsync(c => c.UsuarioSeguidorId == usuarioModel.Id);
            var seguidores = await _db.SeguindoModel.CountAsync(c => c.UsuarioSeguidoId == usuarioModel.Id);

            return Ok(PopulaModelVisualizacao(usuarioModel, seguindo, seguidores));
        }

        /// <summary>
        /// Carrega os dados do usuário logado
        /// </summary>
        /// <returns>VisualizacaoUsuarioViewModel</returns>
        [ResponseType(typeof(VisualizacaoUsuarioViewModel))]
        [Route("api/usuario/meusdados/")]
        public async Task<IHttpActionResult> GetMeuUsuarioModel()
        {
            var usuarioModel = await _db.UsuarioModel.FirstOrDefaultAsync(f => f.NomeUsuario == User.Identity.Name);

            if (usuarioModel == null)
            {
                return NotFound();
            }

            var seguindo = await _db.SeguindoModel.CountAsync(c => c.UsuarioSeguidorId == usuarioModel.Id);
            var seguidores = await _db.SeguindoModel.CountAsync(c => c.UsuarioSeguidoId == usuarioModel.Id);

            return Ok(PopulaModelVisualizacao(usuarioModel, seguindo, seguidores));
        }

        /// <summary>
        /// Alterar os dados do usuário
        /// </summary>
        /// <param name="usuarioModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        [Route("api/usuario/alterar/")]
        public async Task<IHttpActionResult> PostAlteracaoUsuarioModel(AlteracaoUsuarioViewModel usuarioModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (User.Identity.Name != usuarioModel.NomeUsuario)
            {
                return BadRequest("Você não pode alterar os dados de outro usuário");
            }

            var usuario = await _db.UsuarioModel.FirstOrDefaultAsync(f => f.NomeUsuario == usuarioModel.NomeUsuario);

            AtualizarModel(ref usuario, usuarioModel);

            _db.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!UsuarioModelExists(usuarioModel.NomeUsuario))
                {
                    return NotFound();
                }
                else
                {
                    return InternalServerError(ex);
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Cria um novo registro de usuário
        /// </summary>
        /// <param name="usuarioModel">RegistroUsuarioViewModel</param>
        /// <returns>Ok (201, VisualizacaoUsuarioViewModel)</returns>
        [AllowAnonymous]
        [ResponseType(typeof(VisualizacaoUsuarioViewModel))]
        [Route("api/usuario/novo/")]
        public async Task<IHttpActionResult> PostInclusaoUsuarioModel(RegistroUsuarioViewModel usuarioModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if (UsuarioModelExists(usuarioModel.NomeUsuario))
            {
                ModelState.AddModelError("NomeUsuario", "Este nome de usuário já existe! Por favor digite outro nome de usuário.");
                return BadRequest(ModelState);
            }

            if (!usuarioModel.Senha.Equals(usuarioModel.ConfirmacaoSenha))
            {
                ModelState.AddModelError("Senha", "A senha e a confirmação de senha não são iguais. Por favor corrija e tente novamente.");
                return BadRequest(ModelState);
            }

            var usuario = new UsuarioModel();

            AtualizarModel(ref usuario, usuarioRegistro: usuarioModel);

            _db.UsuarioModel.Add(usuario);
            try
            {
                await _db.SaveChangesAsync();

                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                var visualizacaoModel = PopulaModelVisualizacao(usuario, 0, 0);

                var nameClaim = new Claim(ClaimTypes.Name, usuario.NomeUsuario);
                var nameIdentifier = new Claim(ClaimTypes.NameIdentifier, new Guid().ToString());
                var identityProvider = new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", new Guid().ToString());

                var claims = new List<Claim> { nameClaim, nameIdentifier, identityProvider };

                // não há necessidade de incluir permissões, pois as
                // permissões são as mesmas para todos.
                var oAuthIdentity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                var cookiesIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

                var properties = ApplicationOAuthProvider.CreateProperties(usuario.NomeUsuario);

                var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                Authentication.SignIn(cookiesIdentity);

                return CreatedAtRoute("Pesquisa", new { nomeUsuario = usuario.NomeUsuario }, visualizacaoModel);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Exclui a conta de usuário
        /// </summary>
        /// <param name="nomeUsuario">string</param>
        /// <returns>VisualizacaoUsuarioViewModel</returns>
        [ResponseType(typeof(VisualizacaoUsuarioViewModel))]
        [Route("api/usuario/excluir/")]
        public async Task<IHttpActionResult> DeleteUsuarioModel(string nomeUsuario)
        {
            if (User.Identity.Name != nomeUsuario)
            {
                return BadRequest("Você não pode excluir outros usuários!");
            }

            if (string.IsNullOrWhiteSpace(nomeUsuario))
            {
                return BadRequest("O nome de usuário é obrigatório!");
            }

            var usuarioModel = await _db.UsuarioModel.FirstOrDefaultAsync(f => f.NomeUsuario == nomeUsuario);

            if (usuarioModel == null)
            {
                return NotFound();
            }
            
            var excluirSeguidosQuery = string.Format(@"DELETE FROM SeguindoModel WHERE UsuarioSeguidorId = {0}", usuarioModel.Id);
            var excluirSeguidoresQuery = string.Format(@"DELETE FROM SeguindoModel WHERE UsuarioSeguidoId = {0}", usuarioModel.Id);
            var excluirCurtidasQuery = string.Format(@"DELETE FROM CurtidaModel WHERE UsuarioModelId = {0}", usuarioModel.Id);
            var excluirComentariosQuery = string.Format(@"DELETE FROM ComentarioModel WHERE UsuarioModelId = {0}", usuarioModel.Id);
            var excluirPostagensQuery = string.Format(@"DELETE FROM PostagemModel WHERE UsuarioModelId = {0}", usuarioModel.Id);

            // EXCLUI TODOS OS REGISTROS RELACIONADOS À ESTE USUÁRIO
            await _db.Database.ExecuteSqlCommandAsync(excluirSeguidosQuery);
            await _db.Database.ExecuteSqlCommandAsync(excluirSeguidoresQuery);
            await _db.Database.ExecuteSqlCommandAsync(excluirCurtidasQuery);
            await _db.Database.ExecuteSqlCommandAsync(excluirComentariosQuery);
            await _db.Database.ExecuteSqlCommandAsync(excluirPostagensQuery);
            
            _db.UsuarioModel.Remove(usuarioModel);
            await _db.SaveChangesAsync();

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            return Ok(PopulaModelVisualizacao(usuarioModel, 0, 0));
        }

        /// <summary>
        /// Segue ou deixa de seguir determinado usuário
        /// </summary>
        /// <param name="nomeUsuario">string</param>
        /// <returns>Ok</returns>
        [Route("api/usuario/seguir/")]
        public async Task<IHttpActionResult> PostSeguirUsuario(string nomeUsuario)
        {
            var usuarioSeguido =
                await _db.UsuarioModel.FirstOrDefaultAsync(f => f.NomeUsuario == nomeUsuario);

            var usuarioSeguidor =
                await _db.UsuarioModel.FirstOrDefaultAsync(f => f.NomeUsuario == User.Identity.Name);

            var seguindo =
                await
                    _db.SeguindoModel.FirstOrDefaultAsync(
                        f => f.UsuarioSeguidoId == usuarioSeguido.Id && f.UsuarioSeguidorId == usuarioSeguidor.Id);

            if (seguindo == null)
            {
                seguindo = new SeguindoModel
                {
                    UsuarioSeguidoId = usuarioSeguido.Id,
                    UsuarioSeguidorId = usuarioSeguidor.Id,
                    DataAtualizacao = DateTime.UtcNow.AddHours(-3)
                };

                _db.SeguindoModel.Add(seguindo);
            }
            else
            {
                _db.SeguindoModel.Remove(seguindo);
            }

            try
            {
                await _db.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioModelExists(string nomeUsario)
        {
            return _db.UsuarioModel.Count(e => e.NomeUsuario == nomeUsario) > 0;
        }

        private void AtualizarModel(ref UsuarioModel usuario, AlteracaoUsuarioViewModel usuarioAlteracao = null, RegistroUsuarioViewModel usuarioRegistro = null)
        {
            if (usuarioAlteracao != null)
            {
                if (string.IsNullOrWhiteSpace(usuario.NomeUsuario))
                {
                    usuario.NomeUsuario = usuarioAlteracao.NomeUsuario;
                }

                usuario.Biografia = usuarioAlteracao.Biografia;
                usuario.DataNascimento = usuarioAlteracao.DataNascimento;
                usuario.Email = usuarioAlteracao.Email;
                usuario.NomeCompleto = usuarioAlteracao.NomeCompleto;
                usuario.DataAtualizacao = DateTime.UtcNow.AddHours(-3);

                if (string.IsNullOrWhiteSpace(usuarioAlteracao.Avatar)) return;

                var path = HttpContext.Current.Server.MapPath("~/Images/Uploads/");
                usuario.Avatar = ImageService.SalvarImagemNoServidor(usuarioAlteracao.NomeUsuario, path, usuarioAlteracao.Avatar);
            }
            else if (usuarioRegistro != null)
            {
                var kriptho = new Kriptho();

                if (string.IsNullOrWhiteSpace(usuario.NomeUsuario))
                {
                    usuario.NomeUsuario = usuarioRegistro.NomeUsuario;
                }

                usuario.DataNascimento = usuarioRegistro.DataNascimento;
                usuario.Email = usuarioRegistro.Email;
                usuario.NomeCompleto = usuarioRegistro.NomeCompleto;
                usuario.DataAtualizacao = DateTime.UtcNow.AddHours(-3);
                usuario.Senha = kriptho.GetPassword(usuarioRegistro.Senha);
            }
        }

        private VisualizacaoUsuarioViewModel PopulaModelVisualizacao(UsuarioModel usuarioModel, int seguindo, int seguidores)
        {
            return new VisualizacaoUsuarioViewModel
            {
                Avatar = usuarioModel.Avatar ?? "",
                Biografia = usuarioModel.Biografia ?? "",
                DataNascimento = usuarioModel.DataNascimento,
                Email = usuarioModel.Email,
                NomeCompleto = usuarioModel.NomeCompleto,
                NomeUsuario = usuarioModel.NomeUsuario,
                Seguindo = seguindo,
                Seguidores = seguidores
            };
        }
    }
}
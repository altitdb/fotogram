using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Fotogram.Models;
using Fotogram.Security;

namespace Fotogram.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var nomeUsuario = context.UserName;
            var senha = context.Password;

            if (!(string.IsNullOrWhiteSpace(nomeUsuario) && string.IsNullOrWhiteSpace(senha)))
            {
                try
                {
                    using (var db = new FotogramContextDb())
                    {
                        var usuario = await
                            db.UsuarioModel.FirstOrDefaultAsync(f =>
                                f.NomeUsuario == nomeUsuario);

                        if (usuario == null)
                        {
                            context.SetError("invalid_grant", "Falha na autenticação! Usuário inválido.");
                            return;
                        }

                        var kriptho = new Kriptho();

                        if (!kriptho.VerifyPassword(senha, usuario.Senha))
                        {
                            context.SetError("invalid_grant", "Falha na autenticação! Senha inválida.");
                            return;
                        }

                        var nameClaim = new Claim(ClaimTypes.Name, nomeUsuario);
                        var nameIdentifier = new Claim(ClaimTypes.NameIdentifier, new Guid().ToString());
                        var identityProvider = new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", new Guid().ToString());

                        var claims = new List<Claim> { nameClaim, nameIdentifier, identityProvider };

                        // não há necessidade de incluir permissões, pois as
                        // permissões são as mesmas para todos.
                        var oAuthIdentity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                        var cookiesIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);

                        var properties = CreateProperties(nomeUsuario);

                        var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                        context.Validated(ticket);
                        context.Request.Context.Authentication.SignIn(cookiesIdentity);
                    }
                }
                catch
                {
                    context.SetError("invalid_grant", "Falha na autenticação!");
                }
            }
            else
            {
                context.SetError("invalid_grant", "Falha na autenticação! Nome de usuário e senha são obrigatórios.");
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
    }
}
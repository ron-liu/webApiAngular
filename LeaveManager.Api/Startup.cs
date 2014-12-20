using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Owin;

[assembly: OwinStartup(typeof(LeaveManager.Api.Startup))]
namespace LeaveManager.Api
{
	public class Startup
	{
		public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();
			ConfigureOAuth(app);

			WebApiConfig.Register(config);
			app.UseWebApi(config);
		}

		public void ConfigureOAuth(IAppBuilder app)
		{
			app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

			app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions()
			{
				AllowInsecureHttp = true,
				TokenEndpointPath = new PathString("/token"),
				AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
				Provider = new OAuthAuthorizationServerProvider
				{
					OnValidateClientAuthentication = async c=>c.Validated(),
					OnGrantResourceOwnerCredentials = async c =>
					{
						using (var repo = new AuthRepository())
						{
							var user = await repo.FindUser(c.UserName, c.Password);
							if (user == null)
							{
								c.Rejected();
								throw new ApiException("User not existed or wrong password.");
							}
						}
						var identity = new ClaimsIdentity(c.Options.AuthenticationType);
						identity.AddClaims(new[] {new Claim(ClaimTypes.Name, c.UserName), new Claim(ClaimTypes.Role, "user")});

						c.Validated(identity);
					}
				},
			});
		}
	}


}
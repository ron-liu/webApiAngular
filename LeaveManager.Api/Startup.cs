using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web.Http;
using LeaveManager.Api.Domain;
using LeaveManager.Api.Infrastructure;
using LeaveManager.Api.Query;
using Microsoft.Owin;
using Microsoft.Owin.Security.OAuth;
using NEventStore;
using NEventStore.Dispatcher;
using NEventStore.Persistence;
using NEventStore.Persistence.Sql.SqlDialects;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
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
			app.UseNinjectMiddleware(CreateKernel).UseNinjectWebApi(config);
		}

		private static StandardKernel CreateKernel()
		{
			var kernel = new StandardKernel(new NinjectSettings { InjectParentPrivateProperties = true, InjectNonPublic = true});

			kernel.Bind<IQueryHandler<ListLeavesByUserName, IEnumerable<Leave>>>()
				.To<ListLeavesByUserNameQueryHandler>();

			kernel.Bind<IEventPublisher, ICommandSender>().To<FakeBus>().InSingletonScope();

			kernel.Bind<IStoreEvents>().ToMethod(x => Wireup.Init()
				.UsingSqlPersistence("EventStore")
				.WithDialect(new MsSqlDialect())
				.InitializeStorageEngine()
				.UsingJsonSerialization()
				.UsingSynchronousDispatchScheduler()
				// it is not nice to use service locator due to cannot figure out how to let NEventStore to use injected stuff
				.DispatchTo(new FakeBusDispatcher(kernel.Get<IEventPublisher>())) 
				.Build()).InSingletonScope();

			kernel.Bind(typeof(IEventSteamRepository<>)).To(typeof(EventSteamRepository<>));

			// Bind all commands and events handler, those actually can be done by reflect the assembly itself.
			// But here, I just want to do it explicitly
			kernel.Bind<ICommandHandler<ApplyLeaveCommand>, ICommandHandler<EvaluateLeaveCommand>>().To<LeaveCommandHandler>();


			return kernel;
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
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.StaticFiles;
using Microsoft.Practices.Unity;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace VideoOverlay.Web
{
	public class Startup
	{
		public void Configuration( IAppBuilder app )
		{
			var webApiConfiguration = ConfigureWebApi();

			var fileServerOptions = new FileServerOptions
			{
				FileSystem = new PhysicalFileSystem( "wwwroot" ),
				EnableDefaultFiles = true
			};
			fileServerOptions.DefaultFilesOptions.DefaultFileNames = new[]
			{
				"index.htm"
			};

			app.UseFileServer( fileServerOptions );
			//app.UseStaticFiles( new StaticFileOptions
			//{
			//	FileSystem = new PhysicalFileSystem( "wwwroot" )
			//} );

			app.UseWebApi( webApiConfiguration );

			ConfigureUnity( webApiConfiguration );
		}

		private HttpConfiguration ConfigureWebApi()
		{
			var config = new HttpConfiguration();

			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				"DefaultApi",
				"api/{controller}/{id}",
				new { id = RouteParameter.Optional } );

			return config;
		}

		Lazy<UnityContainer> _UnityConfiguration { get; } = new Lazy<UnityContainer>( GetUnityConfiguration );

		public UnityContainer UnityConfiguration => _UnityConfiguration.Value;

		public event EventHandler<EventArgs<UnityContainer>> UnityConfigured;

		public void ConfigureUnity( HttpConfiguration config )
		{
			var container = UnityConfiguration;

			UnityConfigured?.Invoke( null, new EventArgs<UnityContainer>( container ) );

			config.DependencyResolver = new UnityResolver( container );

			// Other Web API configuration not shown.
		}

		static UnityContainer GetUnityConfiguration()
		{
			var container = new UnityContainer();

			container.RegisterInstance<IEntryRepository>( new EntryRepository() );

			return container;
		}
	}
}
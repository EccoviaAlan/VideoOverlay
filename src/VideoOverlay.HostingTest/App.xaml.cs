using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VideoOverlay.Web;
using web = VideoOverlay.Web;

namespace VideoOverlay.HostingTest
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		//CancellationTokenSource CancellationSource;

		//bool WebHostActive;

		//Task Hostingtask;

		IDisposable WebAPI;

		protected override void OnStartup( StartupEventArgs e )
		{
			base.OnStartup( e );
			var startup = new web.Startup();
			startup.UnityConfigured += Web_UnityConfigured;
			WebAPI = WebApp.Start( "http://localhost:10080", startup.Configuration );
			//CancellationSource = new CancellationTokenSource();

			//Hostingtask = Task.Factory.StartNew( () =>
			//{
			//	WebHostActive = true;

			//	//Console.WriteLine( "hosting ffoqsi" );

			//	//var host = new WebHostBuilder()
			//	//	.UseKestrel()
			//	//	.UseContentRoot( pathToContentRoot )
			//	//	.UseIISIntegration()
			//	//	.UseStartup<Startup>()
			//	//	.Build();

			//	//host.Run( CancellationSource.Token );

			//	WebHostActive = true;
			//} );
		}

		private void Web_UnityConfigured( object sender, EventArgs<Microsoft.Practices.Unity.UnityContainer> e )
		{
			var repository = e.Data.Resolve<IEntryRepository>();
			repository.Store( new Entry( "Test Entry 1" ), new DateTime( 2017, 8, 6 ) );
			repository.Store( new Entry( "Test Entry 2" ), new DateTime( 2017, 8, 7 ) );
			repository.Store( new Entry( "Test Entry 3" ), new DateTime( 2017, 8, 8 ) );
		}

		protected override void OnExit( ExitEventArgs e )
		{
			//	if( CancellationSource != null )
			//		CancellationSource.Cancel();

			//	var timeout = DateTime.UtcNow.AddMinutes( 2d );
			//	while( WebHostActive && DateTime.UtcNow < timeout )
			//		Thread.Sleep( 50 );
			if( WebAPI != null )
			{
				try
				{
					WebAPI.Dispose();
					WebAPI = null;
				}
				catch { }
			}
			base.OnExit( e );
		}
}
}
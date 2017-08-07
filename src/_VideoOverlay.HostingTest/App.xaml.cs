using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using VideoOverlay.Web;

namespace VideoOverlay.HostingTest
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		CancellationTokenSource CancellationSource;

		bool WebHostActive;

		Task Hostingtask;

		protected override void OnStartup( StartupEventArgs e )
		{
			base.OnStartup( e );

			string pathToContentRoot = Path.GetDirectoryName( System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName );

			CancellationSource = new CancellationTokenSource();

			Hostingtask = Task.Factory.StartNew( () =>
			{
				WebHostActive = true;
				Console.WriteLine( "hosting ffoqsi" );

				var host = new WebHostBuilder()
					.UseKestrel()
					.UseContentRoot( pathToContentRoot )
					.UseIISIntegration()
					.UseStartup<Startup>()
					.Build();

				host.Run( CancellationSource.Token );

				WebHostActive = true;
			} );
		}

		protected override void OnExit( ExitEventArgs e )
		{
			if( CancellationSource != null )
				CancellationSource.Cancel();

			var timeout = DateTime.UtcNow.AddMinutes( 2d );
			while( WebHostActive && DateTime.UtcNow < timeout )
				Thread.Sleep( 50 );
			base.OnExit( e );
		}
	}
}

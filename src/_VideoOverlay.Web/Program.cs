using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace VideoOverlay.Web
{
	public class Program
	{
		public static void Main( string[] args )
		{
			//bool isService = args.Contains( "-service", StringComparer.OrdinalIgnoreCase );

			string pathToContentRoot = //isService ? Path.GetDirectoryName( System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName ) : 
				Directory.GetCurrentDirectory();

			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot( pathToContentRoot )
				.UseIISIntegration()
				.UseStartup<Startup>()
				.Build();

			//if( isService )
			//	host.RunAsService()
			host.Run();
		}
	}
}
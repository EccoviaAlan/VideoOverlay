using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public class Config
	{
		[JsonProperty( "files" )]
		public ObservableCollection<IncludedFile> IncludedFiles { get; set; } = new ObservableCollection<IncludedFile>();

		[JsonProperty( "folders" )]
		public ObservableCollection<IncludedFolder> IncludedFolders { get; set; } = new ObservableCollection<IncludedFolder>();

		/// <summary>
		/// The path to the folder containing the app configuration.
		/// </summary>
		public static string ConfigFolder { get; }

		/// <summary>
		/// The path to the config file.
		/// </summary>
		public static string ConfigFile { get; }

		static Config()
		{
			ConfigFolder = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ), "Eccovia Solutions", "Video Overlay" );
			ConfigFile = Path.Combine( ConfigFolder, "config.json" );
		}

		private static readonly Lazy<Config> _Singleton = new Lazy<Config>( Load );

		public static Config Default => _Singleton.Value;

		public static Config Load()
		{
			try
			{
				if( File.Exists( ConfigFile ) )
					return JsonConvert.DeserializeObject<Config>( File.ReadAllText( ConfigFile ) );
			}
			catch { }
			return new Config();
		}

		public void Save()
		{
			if( !Directory.Exists( ConfigFolder ) )
				Directory.CreateDirectory( ConfigFolder );
			File.WriteAllText( ConfigFile, JsonConvert.SerializeObject( this ) );
		}
	}
}
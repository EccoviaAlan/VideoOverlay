using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public class IncludedFolder : IIncludedPath
	{
		public string FullPath { get; set; }

		public string FolderName => string.IsNullOrEmpty( FullPath ) ? "" : Path.GetFileName( Path.GetDirectoryName( FullPath ) );

		public bool Recursive { get; set; }

		public IncludedFolder()
		{

		}

		public IncludedFolder( string FullPath )
		{
			this.FullPath = FullPath;
		}

		public IEnumerable<string> GetPaths()
		{
			foreach( var file in Directory.EnumerateFiles( FullPath, "*.mp4", Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly ) )
				yield return file;
			foreach( var file in Directory.EnumerateFiles( FullPath, "*.wmv", Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly ) )
				yield return file;
		}
	}
}
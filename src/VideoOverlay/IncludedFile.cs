using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public class IncludedFile : IIncludedPath
	{
		public string FullPath { get; set; }

		public string FileName => string.IsNullOrEmpty( FullPath ) ? "" : Path.GetFileName( FullPath );

		public string PathName => string.IsNullOrEmpty( FullPath ) ? "" : Path.GetDirectoryName( FullPath );

		public IncludedFile()
		{

		}

		public IncludedFile( string FullPath )
		{
			this.FullPath = FullPath;
		}

		public IEnumerable<string> GetPaths()
		{
			yield return FullPath;
		}
	}
}
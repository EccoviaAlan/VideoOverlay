using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public interface IIncludedPath
	{
		IEnumerable<string> GetPaths();
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public static class EnumerableExtensions
	{
		public static void AddRange<T>( this IList<T> List, IEnumerable<T> Items )
		{
			if( Items != null )
				foreach( var item in Items )
					List.Add( item );
		}
	}
}

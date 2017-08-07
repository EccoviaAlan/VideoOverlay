using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public static class EntryExtensions
	{
		public static DatedEntry Dated( this Entry Entry, DateTime Date ) => new DatedEntry( Date, Entry.Text, Entry.SubText );

		public static Entry Copy( this Entry Entry ) => new Entry( Entry.Text, Entry.SubText );
	}
}

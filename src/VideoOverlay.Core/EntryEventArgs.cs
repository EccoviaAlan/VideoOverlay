using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public class EntryEventArgs : EventArgs
	{
		public DateTime Date { get; }

		public Entry Entry { get; }

		public EntryEventArgs( DateTime Date, Entry Entry )
		{
			this.Date = Date;
			this.Entry = Entry;
		}
	}
}
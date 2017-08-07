using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public interface IEntryRepository
	{
		event EventHandler<EntryEventArgs> Updated;

		int Count { get; }

		Entry GetEntry( DateTime EntryDate );

		IEnumerable<DatedEntry> GetEntriesInRange( DateTime StartDate, DateTime EndDate );

		IEnumerable<DatedEntry> List();

		void Store( DatedEntry Entry );

		void Store( Entry Entry, DateTime Date );

		void BulkLoad( IEnumerable<DatedEntry> Entries );
	}
}

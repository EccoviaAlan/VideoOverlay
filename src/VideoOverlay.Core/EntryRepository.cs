using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	/// <summary>
	/// A default implementation of <see cref="IEntryRepository"/> that simply stores <see cref="Entry"/> objects in a <see cref="SortedList{DateTime, Entry}"/>,
	/// by date. Optimized for a small number of entries.
	/// </summary>
	public class EntryRepository : IEntryRepository
	{
		public event EventHandler<EntryEventArgs> Updated;

		public int Count => Entries.Count;

		public SortedList<DateTime, Entry> Entries { get; } = new SortedList<DateTime, Entry>();

		object Lock = new object();

		public Entry GetEntry( DateTime EntryDate )
		{
			lock( Lock )
			{
				if( Entries.TryGetValue( EntryDate, out var entry ) )
					return entry;
			}
			return null;
		}

		public IEnumerable<DatedEntry> GetEntriesInRange( DateTime StartDate, DateTime EndDate )
		{
			var result = new List<DatedEntry>();
			DateTime current = StartDate;
			lock( Lock )
			{
				while( current < EndDate )
				{
					if( Entries.TryGetValue( current, out var entry ) )
						result.Add( new DatedEntry( current, entry.Text, entry.SubText ) );
					current = current.AddDays( 1 );
				}
			}
			return result;
		}

		public IEnumerable<DatedEntry> List()
		{
			lock( Lock )
			{
				foreach( var entry in Entries )
					yield return new DatedEntry( entry.Key, entry.Value.Text, entry.Value.SubText );
			}
		}

		public void Store( DatedEntry Entry ) => Store( Entry.Copy(), Entry.Date );

		public void Store( Entry Entry, DateTime Date )
		{
			lock( Lock )
			{
				Entries[ Date ] = Entry;
			}
			Updated?.Invoke( this, new EntryEventArgs( Date, Entry ) );
		}

		public void BulkLoad( IEnumerable<DatedEntry> Entries )
		{
			lock( Lock )
			{
				foreach( var entry in Entries )
					this.Entries[ entry.Date ] = entry.Copy();
			}
		}
	}
}
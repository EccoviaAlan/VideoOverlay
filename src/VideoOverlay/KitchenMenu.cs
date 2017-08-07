using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public class KitchenMenu
	{
		public DateTime StartDate { get; }

		public DateTime EndDate { get; }

		public ObservableCollection<DatedEntry> Entries { get; } = new ObservableCollection<DatedEntry>();

		public KitchenMenu()
		{
			var start = DateTime.Now.Date;
			start = start.AddDays( -(int)start.DayOfWeek );
			StartDate = start;
			EndDate = start.AddDays( 6d );
		}
	}
}
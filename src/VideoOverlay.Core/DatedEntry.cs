using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public class DatedEntry : Entry
	{
		[JsonProperty( "date" )]
		public DateTime Date { get; set; }

		[JsonIgnore]
		public int State => Date == DateTime.Now.Date ? 1 : (string.IsNullOrWhiteSpace( Text ) ? 2 : 0 );

		public DatedEntry()
		{

		}

		public DatedEntry( DateTime Date, string Text = null, string SubText = null )
			: base( Text, SubText )
		{
			this.Date = Date;
		}
	}
}
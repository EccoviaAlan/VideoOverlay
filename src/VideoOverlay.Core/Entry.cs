using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOverlay
{
	public class Entry
	{
		[JsonProperty( "text" )]
		public string Text { get; set; }

		[JsonProperty( "sub", DefaultValueHandling = DefaultValueHandling.Ignore, NullValueHandling = NullValueHandling.Ignore )]
		public string SubText { get; set; }

		public Entry()
		{

		}

		public Entry( string Text, string SubText = null )
		{
			this.Text = Text;
			this.SubText = SubText;
		}
	}
}

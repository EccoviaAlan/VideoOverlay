using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace VideoOverlay.Web.Controllers
{
	[RoutePrefix( "api/entry" )]
	public class EntryController : ApiController
	{
		[HttpGet, Route( "hello" )]
		public IHttpActionResult Get()
		{
			return Ok( new
			{
				message = "Hello World"
			} );
		}

		[HttpGet, Route( "{Date:datetime}" )]
		public IHttpActionResult Get( DateTime Date )
		{
			var result = EntryRepository.GetEntry( Date );
			if( result == null )
				return NotFound();
			return Ok( result );
		}

		[HttpGet, Route( "{StartDate:datetime}/{EndDate:datetime}" )]
		public IHttpActionResult Get( DateTime StartDate, DateTime EndDate ) => Ok( EntryRepository.GetEntriesInRange( StartDate, EndDate ) );

		[HttpPost, Route( "{Date:datetime}" )]
		public IHttpActionResult Post( DateTime Date, Entry Entry )
		{
			EntryRepository.Store( Entry, Date );
			return Ok( Entry.Dated( Date ) );
		}

		public IEntryRepository EntryRepository { get; }

		public EntryController( IEntryRepository EntryRepository )
		{
			this.EntryRepository = EntryRepository;
		}
	}
}

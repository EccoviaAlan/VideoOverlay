using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace VideoOverlay.Web.Controllers
{
	[Produces( "application/json" )]
	[Route( "api/Entry" )]
	public class EntryController : Controller
	{
		[HttpGet, Route( "{Date:datetime}" )]
		public ActionResult Get( DateTime Date )
		{
			var result = EntryRepository.GetEntry( Date );
			if( result == null )
				return NotFound();
			return Ok( result );
		}

		[HttpGet, Route( "{StartDate:datetime}/{EndDate:datetime}" )]
		public ActionResult Get( DateTime StartDate, DateTime EndDate ) => Ok( EntryRepository.GetEntriesInRange( StartDate, EndDate ) );

		public IEntryRepository EntryRepository { get; }

		public EntryController( IEntryRepository EntryRepository )
		{
			this.EntryRepository = EntryRepository;
		}
	}
}
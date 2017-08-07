using Microsoft.Owin.Hosting;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using web = VideoOverlay.Web;

namespace VideoOverlay
{
	public class ApiHost : IDisposable
	{
		IDisposable WebAPI;

		public event EventHandler<EntryEventArgs> EntryUpdated;

		public Func<IEnumerable<DatedEntry>> InitialLoad { get; set; }

		public IEntryRepository EntryRepository { get; private set; }

		public ApiHost Start()
		{
			if( WebAPI != null )
				Stop();

			var startup = new web.Startup();
			startup.UnityConfigured += Web_UnityConfigured;
			WebAPI = WebApp.Start( "http://localhost:10080", startup.Configuration );
			return this;
		}

		public ApiHost Stop()
		{
			if( WebAPI != null )
			{
				try
				{
					WebAPI.Dispose();
					WebAPI = null;
				}
				catch { }
			}
			return this;
		}

		private void Web_UnityConfigured( object sender, EventArgs<UnityContainer> e )
		{
			EntryRepository = e.Data.Resolve<IEntryRepository>();
			if( InitialLoad != null )
				EntryRepository.BulkLoad( InitialLoad() );
			EntryRepository.Updated += Repository_Updated;
		}

		private void Repository_Updated( object sender, EntryEventArgs e )
		{
			EntryUpdated?.Invoke( sender, e );
		}

		public void Dispose() => Stop();
	}
}
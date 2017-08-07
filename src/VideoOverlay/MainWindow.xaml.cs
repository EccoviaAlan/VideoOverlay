using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using win32 = Microsoft.Win32;
using wpf = System.Windows;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace VideoOverlay
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ApiHost Host;

		public KitchenMenu KitchenMenu { get; set; }

		public MainWindow()
		{
			InitializeComponent();

			KitchenMenu = new KitchenMenu();

			Files.ItemsSource = Config.Default.IncludedFiles;
			Folders.ItemsSource = Config.Default.IncludedFolders;
			Host = new ApiHost
			{
				InitialLoad = LoadEntries
			}.Start();

			if( Host.EntryRepository != null )
				KitchenMenu.Entries.AddRange( Host.EntryRepository.GetEntriesInRange( KitchenMenu.StartDate, KitchenMenu.EndDate ) );

			Host.EntryUpdated += Host_EntryUpdated;
		}

		private void Host_EntryUpdated( object sender, EntryEventArgs e )
		{
			var repository = sender as IEntryRepository;
			if( repository != null )
				SaveEntries( repository );
			Dispatcher.Invoke( delegate
			{
				if( e.Date >= KitchenMenu.StartDate && e.Date <= KitchenMenu.EndDate )
				{
					for( int i = 0; i < KitchenMenu.Entries.Count; i++ )
						if( KitchenMenu.Entries[ i ].Date == e.Date )
						{
							KitchenMenu.Entries[ i ] = e.Entry.Dated( e.Date );
							return;
						}
						else if( KitchenMenu.Entries[i].Date > e.Date )
						{
							KitchenMenu.Entries.Insert( i, e.Entry.Dated( e.Date ) );
							return;
						}
					KitchenMenu.Entries.Add( e.Entry.Dated( e.Date ) );
				}
			} );
		}

		protected override void OnClosing( CancelEventArgs e )
		{
			if( Host != null )
			{
				try
				{
					Host.Stop();
				}
				catch { }
			}
		}

		string EntryFile = Path.Combine( Config.ConfigFolder, "entries.json" );

		IEnumerable<DatedEntry> LoadEntries()
		{
			if( File.Exists( EntryFile ) )
				return JsonConvert.DeserializeObject<List<DatedEntry>>( File.ReadAllText( EntryFile ) );
			return new List<DatedEntry>();
		}

		void SaveEntries( IEntryRepository Repository )
		{
			File.WriteAllText( EntryFile, JsonConvert.SerializeObject( Repository.List() ) );
		}

		private void Run_Click( object sender, RoutedEventArgs e )
		{
			LoadQueue();

			if( Queue.Count > 0 )
			{
				if( Screen.AllScreens.Length == 1 )
					new DisplayVideo( KitchenMenu, NextVideo ).Show();
				else
					foreach( var screen in Screen.AllScreens )
					{
						if( screen != Screen.PrimaryScreen )
						{
							var display = new DisplayVideo( KitchenMenu, NextVideo );
							display.Top = screen.WorkingArea.Top;
							display.Left = screen.WorkingArea.Left;
							display.Width = screen.WorkingArea.Width;
							display.Height = screen.WorkingArea.Height;
							display.Show();
						}
					}
			}
			else
				wpf.MessageBox.Show( "No videos found.", "No Videos" );
		}

		Queue<string> Queue { get; } = new Queue<string>();

		private void LoadQueue()
		{
			Queue.Clear();

			var random = new Random();
			var deDuped = new Dictionary<string, int>( StringComparer.OrdinalIgnoreCase );
			//var allFiles = new List<Tuple<string, int>>();

			foreach( var file in Config.Default.IncludedFiles )
				deDuped[ file.FullPath ] = random.Next();
			//allFiles.Add( new Tuple<string, int>( file.FullPath, random.Next() ) );

			foreach( var folder in Config.Default.IncludedFolders )
				foreach( var file in folder.GetPaths() )
					deDuped[ file ] = random.Next();
			//if( !allFiles.Any( tuple => tuple.Item1.Equals( file, StringComparison.OrdinalIgnoreCase ) ) )
			//allFiles.Add( new Tuple<string, int>( file, random.Next() ) );

			foreach( var item in deDuped.OrderBy( kvp => kvp.Value ) )
				Queue.Enqueue( item.Key );

			//if( allFiles.Count > 0 )
			//{
			//	allFiles.Sort( ( i1, i2 ) => i1.Item2.CompareTo( i2.Item2 ) );

			//	foreach( var item in allFiles )
			//		Queue.Enqueue( item.Item1 );
			//}
		}

		private string NextVideo()
		{
			if( Queue.Count == 0 )
				LoadQueue();

			if( Queue.Count > 0 )
				return Queue.Dequeue();

			return null;
		}

		private void AddFile_Click( object sender, RoutedEventArgs e )
		{
			var dialog = new win32.OpenFileDialog()
			{
				CheckFileExists = true,
				DefaultExt = ".mp4",
				Filter = ".mp4 Files|*.mp4|.wmv Files|*.wmv|All Files|*.*",
				Multiselect = true,
				Title = "Select Video File(s)"
			};
			if( dialog.ShowDialog() == true )
			{
				foreach( var fileName in dialog.FileNames )
					if( !Config.Default.IncludedFiles.Any( file => file.FullPath.Equals( fileName, StringComparison.OrdinalIgnoreCase ) ) )
						Config.Default.IncludedFiles.Add( new IncludedFile( fileName ) );
				try
				{
					Config.Default.Save();
				}
				catch( Exception x )
				{
					wpf.MessageBox.Show( x.ToString(), "Error Saving Configuration", MessageBoxButton.OK, MessageBoxImage.Error );
				}
				this.Files.ItemsSource = Config.Default.IncludedFiles;
			}
		}

		private void AddFolder_Click( object sender, RoutedEventArgs e )
		{
			var dialog = new FolderBrowserDialog
			{
				Description = "Select Folder Containing Video File(s)"
			};
			if( dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK )
			{
				if( !Config.Default.IncludedFolders.Any( folder => folder.FullPath.Equals( dialog.SelectedPath, StringComparison.OrdinalIgnoreCase ) ) )
					Config.Default.IncludedFolders.Add( new IncludedFolder( dialog.SelectedPath ) );
				try
				{
					Config.Default.Save();
				}
				catch( Exception x )
				{
					wpf.MessageBox.Show( x.ToString(), "Error Saving Configuration", MessageBoxButton.OK, MessageBoxImage.Error );
				}
				this.Folders.ItemsSource = Config.Default.IncludedFolders;
			}
		}
	}
}

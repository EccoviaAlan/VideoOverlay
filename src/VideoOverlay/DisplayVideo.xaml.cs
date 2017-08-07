using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VideoOverlay
{
	/// <summary>
	/// Interaction logic for DisplayVideo.xaml
	/// </summary>
	public partial class DisplayVideo : Window
	{
		public Func<string> NextVideo { get; set; }

		DispatcherTimer Timer;

		KitchenMenu MenuItems { get; }

		public DisplayVideo( KitchenMenu MenuItems, Func<string> NextVideo = null )
		{
			this.MenuItems = MenuItems ?? new KitchenMenu();
			this.NextVideo = NextVideo;

			InitializeComponent();

			Loaded += DisplayVideo_Loaded;
			Items.ItemsSource = MenuItems.Entries;

		}

		private void DisplayVideo_Loaded( object sender, RoutedEventArgs e )
		{
			this.WindowState = WindowState.Maximized;
			LoadNextVideo();
			VideoPlayer.MediaEnded += VideoPlayer_MediaEnded;
		}

		private void VideoPlayer_MediaEnded( object sender, RoutedEventArgs e )
		{
			LoadNextVideo();
		}

		private void LoadNextVideo()
		{
			if( NextVideo == null )
			{
				if( Timer == null )
				{
					Timer = new DispatcherTimer( DispatcherPriority.Normal );
					Timer.Tick += Timer_Tick;
					Timer.Interval = TimeSpan.FromSeconds( 2d );
					Timer.Start();
				}
				return;
			}
			if( Timer != null )
				StopTimer();

			string video = NextVideo();
			if( video != null )
			{
				VideoPlayer.Source = new Uri( video );
				VideoPlayer.Play();
			}
		}

		void StopTimer()
		{
			if( Timer != null )
			{
				Timer.Stop();
				Timer = null;
			}
		}

		private void Timer_Tick( object sender, EventArgs e )
		{
			if( NextVideo != null )
			{
				StopTimer();
				LoadNextVideo();
			}
		}

		private void CloseButton_Click( object sender, RoutedEventArgs e )
		{
			this.Close();
		}
	}
}

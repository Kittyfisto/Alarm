using System;
using System.Windows;
using System.Windows.Threading;
using Alarm.UI;

namespace Alarm
{
	public partial class MainWindow
	{
		private readonly MainWindowViewModel _viewModel;
		private readonly DispatcherTimer _timer;

		public MainWindow(MainWindowViewModel viewModel)
		{
			InitializeComponent();

			DataContext = _viewModel = viewModel;
			_timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(100)};
			_timer.Tick += TimerOnTick;

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_timer.Start();
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_timer.Stop();
		}

		private void TimerOnTick(object sender, EventArgs e)
		{
			_viewModel.Update();
		}
	}
}

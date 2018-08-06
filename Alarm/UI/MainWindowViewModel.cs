using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Alarm.BusinessLogic;
using Alarm.UI.Devices;
using log4net;

namespace Alarm.UI
{
	public sealed class MainWindowViewModel
		: INotifyPropertyChanged
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly Configuration _configuration;
		private readonly AlarmSound _alarmSound;
		private readonly Dispatcher _dispatcher;
		private readonly Storage _storage;
		private readonly Stack<IPageViewModel> _pages;

		private IPageViewModel _currentPage;
		private bool _addingAlarm;

		public MainWindowViewModel(Dispatcher dispatcher, Storage storage, Configuration configuration, AlarmSound alarmSound)
		{
			_dispatcher = dispatcher;
			_storage = storage;
			_configuration = configuration;
			_alarmSound = alarmSound;
			_pages = new Stack<IPageViewModel>();

			ShowPage(new MainPageViewModel(dispatcher, storage, configuration, alarmSound, this));
		}

		public IPageViewModel CurrentPage
		{
			get => _currentPage;
			private set
			{
				if (value == _currentPage)
					return;

				_currentPage = value;
				EmitPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void AddAlarm(BusinessLogic.Alarm alarm)
		{
		}

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Update()
		{
			CurrentPage?.Update();
		}

		public void ShowPage(IPageViewModel page)
		{
			_pages.Push(page);
			CurrentPage = page;
		}

		public void Back(IPageViewModel page)
		{
			if (!Equals(CurrentPage, page))
			{
				Log.WarnFormat("Expected current page '{0}' to be '{1}'! Won't navigate back since it's obviously not!", CurrentPage, page);
				return;
			}

			_pages.Pop();
			if (_pages.Count > 0)
			{
				CurrentPage = _pages.Peek();
			}
			else
			{
				CurrentPage = null;
			}
		}
	}
}
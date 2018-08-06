using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Alarm.BusinessLogic;
using Alarm.UI.Alarms;
using Alarm.UI.Devices;
using Alarm.UI.Programs;

namespace Alarm.UI
{
	public sealed class MainPageViewModel
		: IPageViewModel
		, INotifyPropertyChanged
	{
		private readonly AlarmsViewModel _alarms;
		private readonly DevicesViewModel _devices;
		private readonly ProgramsViewModel _programs;

		public MainPageViewModel(Dispatcher dispatcher,
		                         Storage storage,
		                         Configuration configuration,
		                         AlarmSound alarmSound,
		                         MainWindowViewModel mainWindowViewModel)
		{
			_alarms = new AlarmsViewModel(dispatcher, storage, configuration, alarmSound, mainWindowViewModel);
			_devices = new DevicesViewModel(dispatcher, storage, mainWindowViewModel);
			_programs = new ProgramsViewModel();

			SelectedTab = _alarms;
		}

		public IReadOnlyList<ITabViewModel> Tabs => new ITabViewModel[] {_alarms, _devices, _programs};

		private ITabViewModel _selectedTab;

		public ITabViewModel SelectedTab
		{
			get => _selectedTab;
			set
			{
				if (Equals(_selectedTab, value))
					return;

				_selectedTab = value;
				EmitPropertyChanged();
			}
		}

		#region Implementation of IPageViewModel

		public string Name => "Hauptseite";

		public void Update()
		{
			SelectedTab?.Update();
		}

		#endregion

		public event PropertyChangedEventHandler PropertyChanged;

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
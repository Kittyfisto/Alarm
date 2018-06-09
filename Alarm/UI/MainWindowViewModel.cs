using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Alarm.BusinessLogic;
using Metrolib;

namespace Alarm.UI
{
	public sealed class MainWindowViewModel
		: INotifyPropertyChanged
	{
		private readonly AddAlarmViewModel _addAlarmViewModel;
		private readonly AlarmsViewModel _alarmsViewModel;
		private readonly Storage _storage;
		private bool _addingAlarm;

		public MainWindowViewModel(Dispatcher dispatcher, Storage storage, Configuration configuration, AlarmSound alarmSound)
		{
			_storage = storage;
			_alarmsViewModel = new AlarmsViewModel(dispatcher, storage, configuration, alarmSound);
			_addAlarmViewModel = new AddAlarmViewModel(configuration);
			_addAlarmViewModel.AddAlarm += AddAlarm;
			_addAlarmViewModel.Cancel += CancelAddAlarm;
		}

		public AlarmsViewModel AlarmsViewModel => _alarmsViewModel;

		public AddAlarmViewModel AddAlarmViewModel => _addAlarmViewModel;

		public bool AddingAlarm
		{
			get => _addingAlarm;
			private set
			{
				if (value == _addingAlarm)
					return;

				_addingAlarm = value;
				EmitPropertyChanged();
			}
		}

		public ICommand AddAlarmCommand => new DelegateCommand2(OnAddAlarm);

		public event PropertyChangedEventHandler PropertyChanged;

		private void CancelAddAlarm()
		{
			AddingAlarm = false;
		}

		private void AddAlarm(BusinessLogic.Alarm alarm)
		{
			var id = _storage.Add(alarm);
			_alarmsViewModel.Add(id, alarm);
			AddingAlarm = false;
		}

		private void OnAddAlarm()
		{
			AddingAlarm = true;
		}

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void Update()
		{
			_alarmsViewModel.Update();
		}
	}
}
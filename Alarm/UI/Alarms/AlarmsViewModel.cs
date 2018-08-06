using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Alarm.BusinessLogic;
using Metrolib;

namespace Alarm.UI.Alarms
{
	public sealed class AlarmsViewModel
		: ITabViewModel
		, INotifyPropertyChanged
	{
		private readonly ObservableCollection<AlarmViewModel> _alarms;
		private readonly Configuration _configuration;
		private readonly AlarmSound _alarmSound;
		private readonly MainWindowViewModel _mainWindow;
		private readonly Dispatcher _dispatcher;
		private readonly Storage _storage;
		private bool _hasAlarms;
		private AddAlarmViewModel _addAlarms;

		public AlarmsViewModel(Dispatcher dispatcher,
		                       Storage storage,
		                       Configuration configuration,
		                       AlarmSound alarmSound,
		                       MainWindowViewModel mainWindow)
		{
			_dispatcher = dispatcher;
			_storage = storage;
			_configuration = configuration;
			_alarmSound = alarmSound;
			_mainWindow = mainWindow;
			_alarms = new ObservableCollection<AlarmViewModel>();

			AddAllAlarmsAsync();
		}

		public IEnumerable<AlarmViewModel> Alarms => _alarms;

		public bool HasAlarms
		{
			get => _hasAlarms;
			private set
			{
				if (value == _hasAlarms)
					return;

				_hasAlarms = value;
				EmitPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public ICommand AddAlarmCommand => new DelegateCommand2(OnAddAlarm);

		private void OnAddAlarm()
		{
			_addAlarms = new AddAlarmViewModel(_configuration);
			_addAlarms.AddAlarm += AddAlarmsOnAddAlarm;
			_addAlarms.Cancel += AddAlarmsOnCancel;

			_mainWindow.ShowPage(_addAlarms);
		}

		private void AddAlarmsOnAddAlarm(BusinessLogic.Alarm alarm)
		{
			var id = _storage.Add(alarm);
			_alarms.Add(new AlarmViewModel(_storage, id, alarm, null));

			_mainWindow.Back(_addAlarms);
			_addAlarms = null;
		}

		private void AddAlarmsOnCancel()
		{
			_mainWindow.Back(_addAlarms);
			_addAlarms = null;
		}

		private async void AddAllAlarmsAsync()
		{
			var alarms = await _storage.GetAllAlarms();
			var viewModels = new List<AlarmViewModel>();
			foreach (var pair in alarms) viewModels.Add(CreateViewModel(pair.Key, pair.Value));

			await _dispatcher.BeginInvoke(new Action(() =>
			{
				foreach (var viewModel in viewModels)
					Add(viewModel);
			}));
		}

		private void PlayAlarm()
		{
			_alarmSound.Play();
		}

		private void StopAlarm()
		{
			_alarmSound.Stop();
		}

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void Add(AlarmViewModel viewModel)
		{
			_alarms.Add(viewModel);
			_alarms.Sort((lhs, rhs) => lhs.EndTime.CompareTo(rhs.EndTime));
			HasAlarms = true;
		}

		private AlarmViewModel CreateViewModel(Guid id, BusinessLogic.Alarm alarm)
		{
			_configuration.TryGetDevice(alarm.DeviceId, out var device);
			var viewModel = new AlarmViewModel(_storage, id, alarm, device);
			viewModel.Remove += OnRemoveAlarm;
			return viewModel;
		}

		private void OnRemoveAlarm(AlarmViewModel alarm)
		{
			_storage.Remove(alarm.Id);
			_alarms.Remove(alarm);
		}

		#region Implementation of ITabViewModel

		public string Title => "Alarme";

		public void Update()
		{
			var playAlarm = false;
			foreach (var alarm in _alarms)
			{
				alarm.Update();
				if (alarm.IsOverdue && alarm.SoundAlarm)
					playAlarm = true;
			}

			if (playAlarm)
				PlayAlarm();
			else
				StopAlarm();
		}

		#endregion
	}
}
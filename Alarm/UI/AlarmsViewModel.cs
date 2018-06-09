using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using Alarm.BusinessLogic;

namespace Alarm.UI
{
	public sealed class AlarmsViewModel
		: INotifyPropertyChanged
	{
		private readonly ObservableCollection<AlarmViewModel> _alarms;
		private readonly Configuration _configuration;
		private readonly AlarmSound _alarmSound;
		private readonly Dispatcher _dispatcher;
		private readonly Storage _storage;
		private bool _hasAlarms;

		public AlarmsViewModel(Dispatcher dispatcher,
		                       Storage storage,
		                       Configuration configuration,
		                       AlarmSound alarmSound)
		{
			_dispatcher = dispatcher;
			_storage = storage;
			_configuration = configuration;
			_alarmSound = alarmSound;
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

		public void Add(Guid id, BusinessLogic.Alarm alarm)
		{
			var viewModel = CreateViewModel(id, alarm);
			Add(viewModel);
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
	}
}
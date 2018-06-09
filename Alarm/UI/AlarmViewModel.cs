using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Alarm.BusinessLogic;
using Metrolib;

namespace Alarm.UI
{
	public sealed class AlarmViewModel
		: INotifyPropertyChanged
	{
		private readonly BusinessLogic.Alarm _alarm;
		private readonly Device _device;
		private readonly Guid _id;
		private readonly ICommand _removeCommand;
		private readonly Storage _storage;
		private bool _isOverdue;
		private TimeSpan _remainingTime;

		public AlarmViewModel(Storage storage,
		                      Guid id,
		                      BusinessLogic.Alarm alarm,
		                      Device device)
		{
			_storage = storage;
			_id = id;
			_alarm = alarm;
			_device = device;
			_removeCommand = new DelegateCommand2(OnRemove);
		}

		public string SampleId => _alarm.SampleId;

		public DateTime EndTime => _alarm.EndTime;

		public TimeSpan RemainingTime
		{
			get => _remainingTime;
			private set
			{
				if (value == _remainingTime)
					return;

				_remainingTime = value;
				EmitPropertyChanged();

				IsOverdue = value <= TimeSpan.Zero;
			}
		}

		public ICommand RemoveCommand => _removeCommand;

		public Guid Id => _id;

		public string DeviceName => _device?.Name;

		public string Temperature => string.Format("{0}°C", _alarm.Temperature);

		public bool IsOverdue
		{
			get => _isOverdue;
			private set
			{
				if (value == _isOverdue)
					return;

				_isOverdue = value;
				EmitPropertyChanged();
			}
		}

		public bool SoundAlarm
		{
			get => _alarm.SoundAlarm;
			set
			{
				if (value == _alarm.SoundAlarm)
					return;

				_alarm.SoundAlarm = value;
				EmitPropertyChanged();

				_storage.Update(_id, _alarm);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public override string ToString()
		{
			return _alarm.ToString();
		}

		private void OnRemove()
		{
			Remove?.Invoke(this);
		}

		public void Update()
		{
			RemainingTime = _alarm.EndTime.ToLocalTime() - DateTime.Now;
		}

		public event Action<AlarmViewModel> Remove;

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
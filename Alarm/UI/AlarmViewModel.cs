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
		private TimeSpan _remainingTime;
		private readonly ICommand _removeCommand;
		private readonly Guid _id;
		private readonly BusinessLogic.Alarm _alarm;
		private readonly Device _device;
		private bool _isOverdue;

		public AlarmViewModel(Guid id, BusinessLogic.Alarm alarm, Device device)
		{
			_id = id;
			_alarm = alarm;
			_device = device;
			_removeCommand = new DelegateCommand2(OnRemove);
		}

		private void OnRemove()
		{
			Remove?.Invoke(this);
		}

		public string SampleId => _alarm.SampleId;

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

		public void Update()
		{
			RemainingTime = _alarm.EndTime.ToLocalTime() - DateTime.Now;
		}

		public ICommand RemoveCommand => _removeCommand;

		public Guid Id => _id;

		public string DeviceName => _device?.Name;

		public string Temperature => string.Format("{0}°C", _alarm.Temperature);

		public event Action<AlarmViewModel> Remove;

		public event PropertyChangedEventHandler PropertyChanged;

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

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
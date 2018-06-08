using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Alarm.BusinessLogic;
using Metrolib;

namespace Alarm.UI
{
	public sealed class AddAlarmViewModel
		: INotifyPropertyChanged
	{
		private readonly DelegateCommand2 _addAlarmCommand;
		private readonly IReadOnlyList<DeviceViewModel> _availableDevices;
		private readonly ICommand _cancelCommand;
		private readonly Devices _devices;

		private TimeSpan? _estimatedRuntime;
		private int? _numberOfIterations;
		private string _sampleId;
		private DeviceViewModel _selectedDevice;
		private int? _temperature;

		public AddAlarmViewModel(Devices devices)
		{
			_devices = devices;
			_addAlarmCommand = new DelegateCommand2(OnAddAlarm);
			_cancelCommand = new DelegateCommand2(OnCancel);

			_availableDevices = devices.All.Select(x => new DeviceViewModel(x)).ToList();
			SelectedDevice = _availableDevices.FirstOrDefault();

			UpdateAddButton();
		}

		public DeviceViewModel SelectedDevice
		{
			get => _selectedDevice;
			set
			{
				if (value == _selectedDevice)
					return;

				_selectedDevice = value;
				EmitPropertyChanged();
				UpdateEstimatedRuntime();
			}
		}

		public IReadOnlyList<DeviceViewModel> AvailableDevices => _availableDevices;

		public string SampleId
		{
			get => _sampleId;
			set
			{
				if (value == _sampleId)
					return;

				_sampleId = value;
				EmitPropertyChanged();
				UpdateAddButton();
			}
		}

		public int? NumberOfIterations
		{
			get => _numberOfIterations;
			set
			{
				if (value == _numberOfIterations)
					return;

				_numberOfIterations = value;
				EmitPropertyChanged();
				UpdateAddButton();
				UpdateEstimatedRuntime();
			}
		}

		public TimeSpan? EstimatedRuntime
		{
			get => _estimatedRuntime;
			private set
			{
				if (value == _estimatedRuntime)
					return;

				_estimatedRuntime = value;
				EmitPropertyChanged();
			}
		}

		public ICommand AddAlarmCommand => _addAlarmCommand;
		public ICommand CancelCommand => _cancelCommand;

		public DateTime EndTime => DateTime.Now + EstimatedRuntime.Value;

		public int? Temperature
		{
			get => _temperature;
			set
			{
				if (value == _temperature)
					return;

				_temperature = value;
				EmitPropertyChanged();
				UpdateAddButton();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public event Action<BusinessLogic.Alarm> AddAlarm;
		public event Action Cancel;

		private void UpdateAddButton()
		{
			_addAlarmCommand.CanBeExecuted = _sampleId != null &&
			                                 _numberOfIterations != null &&
			                                 _selectedDevice != null &&
			                                 _temperature != null;
		}

		private void OnCancel()
		{
			Cancel?.Invoke();
		}

		private void OnAddAlarm()
		{
			var alarm = new BusinessLogic.Alarm
			{
				SampleId = _sampleId,
				EndTime = EndTime,
				DeviceId = _selectedDevice.Id,
				Temperature = Temperature.Value
			};
			AddAlarm?.Invoke(alarm);
			SampleId = null;
			NumberOfIterations = null;
		}

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		private void UpdateEstimatedRuntime()
		{
			if (_selectedDevice != null && _numberOfIterations != null)
				EstimatedRuntime = _selectedDevice.Runtime(_numberOfIterations.Value);
			else
				EstimatedRuntime = null;
		}
	}
}
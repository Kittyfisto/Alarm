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
		private readonly IReadOnlyList<int> _availableTemperatures;
		private readonly ICommand _cancelCommand;

		private TimeSpan? _estimatedRuntime;
		private int? _numberOfIterations;
		private string _sampleId;
		private DeviceViewModel _selectedDevice;

		private int? _selectedTemperature;

		public AddAlarmViewModel(Configuration configuration)
		{
			_addAlarmCommand = new DelegateCommand2(OnAddAlarm);
			_cancelCommand = new DelegateCommand2(OnCancel);

			_availableDevices = configuration.Devices.Select(x => new DeviceViewModel(x)).ToList();
			SelectedDevice = _availableDevices.FirstOrDefault();

			_availableTemperatures = configuration.Temperatures.OrderBy(x => x).ToList();
			SelectedTemperature = _availableTemperatures.FirstOrDefault();

			UpdateAddButton();
		}

		public int? SelectedTemperature
		{
			get => _selectedTemperature;
			set
			{
				if (value == _selectedTemperature)
					return;

				_selectedTemperature = value;
				EmitPropertyChanged();
				UpdateAddButton();
			}
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

		public IReadOnlyList<int> AvailableTemperatures => _availableTemperatures;

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

		public DateTime EndTime => DateTime.UtcNow + EstimatedRuntime.Value;

		public event PropertyChangedEventHandler PropertyChanged;
		public event Action<BusinessLogic.Alarm> AddAlarm;
		public event Action Cancel;

		private void UpdateAddButton()
		{
			_addAlarmCommand.CanBeExecuted = _sampleId != null &&
			                                 _numberOfIterations != null &&
			                                 _selectedDevice != null &&
			                                 _selectedTemperature != null;
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
				Temperature = _selectedTemperature.Value,
				SoundAlarm = false //< Customer wants sounds disabled by default
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
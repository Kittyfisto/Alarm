using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Threading;
using Alarm.BusinessLogic;
using Metrolib;

namespace Alarm.UI.Devices
{
	public sealed class DevicesViewModel
		: ITabViewModel
		, INotifyPropertyChanged
	{
		private readonly Dispatcher _dispatcher;
		private readonly Storage _storage;
		private readonly ObservableCollection<DeviceViewModel2> _devices;

		private AddDeviceViewModel _addDeviceViewModel;
		private bool _addingDevice;

		public AddDeviceViewModel AddDeviceViewModel
		{
			get => _addDeviceViewModel;
			set
			{
				if (value == _addDeviceViewModel)
					return;

				_addDeviceViewModel = value;
				EmitPropertyChanged();
			}
		}

		public DevicesViewModel(Dispatcher dispatcher, Storage storage, MainWindowViewModel mainPageViewModel)
		{
			_dispatcher = dispatcher;
			_storage = storage;
			_devices = new ObservableCollection<DeviceViewModel2>();
			GetAllDevicesAsync();
		}

		public ICommand AddDeviceCommand => new DelegateCommand2(AddDevice);

		public IEnumerable<DeviceViewModel2> Devices => _devices;

		private void AddDevice()
		{
			if (AddDeviceViewModel == null)
				AddDeviceViewModel = new AddDeviceViewModel();
			AddingDevice = true;
		}

		public bool AddingDevice
		{
			get => _addingDevice;
			set
			{
				if (value == _addingDevice)
					return;

				_addingDevice = value;
				EmitPropertyChanged();
			}
		}

		private async void GetAllDevicesAsync()
		{
			var devices = await _storage.GetAllDevices();
			await _dispatcher.BeginInvoke(new Action(() =>
			{
				_devices.Clear();
				foreach (var pair in devices)
				{
					var deviceViewModel = new DeviceViewModel2(pair.Value);
					_devices.Add(deviceViewModel);
				}
			}));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void EmitPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#region Implementation of ITabViewModel

		public string Title => "Geräte";

		public void Update()
		{}

		#endregion
	}
}
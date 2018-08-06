using System;
using System.Diagnostics.Contracts;
using Alarm.BusinessLogic;

namespace Alarm.UI.Devices
{
	public sealed class DeviceViewModel
	{
		private readonly Device _device;

		public DeviceViewModel(Device device)
		{
			_device = device;
		}

		public string Name => _device.Name;

		public int Id => _device.Id;

		[Pure]
		public TimeSpan? Runtime(int numIterations)
		{
			return _device.Runtime(numIterations);
		}
	}
}
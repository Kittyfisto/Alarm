using Alarm.BusinessLogic;

namespace Alarm.UI.Devices
{
	public sealed class DeviceViewModel2
	{
		private readonly Device2 _device;

		public DeviceViewModel2(Device2 device)
		{
			_device = device;
		}

		public string Name => _device.Name;
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using log4net;

namespace Alarm.BusinessLogic
{
	/// <summary>
	///     Responsible for reading the list of available devices.
	/// </summary>
	public sealed class Devices
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly List<Device> _all;
		private readonly Dictionary<int, Device> _devices;

		public Devices()
		{
			_devices = ReadDevices();
			_all = _devices.Values.ToList();
		}

		public bool TryGetDevice(int id, out Device device)
		{
			return _devices.TryGetValue(id, out device);
		}

		public IReadOnlyList<Device> All => _all;

		private static Dictionary<int, Device> ReadDevices()
		{
			var filePath = Constants.DevicesFile;
			var devices = new Dictionary<int, Device>();
			try
			{
				using (var fileStream = File.OpenRead(filePath))
				using (var reader = XmlReader.Create(fileStream))
				{
					while (reader.Read())
						switch (reader.Name)
						{
							case "device":
								if (Device.TryRead(reader.ReadSubtree(), out var device))
									devices.Add(device.Id, device);
								break;
						}
				}
			}
			catch (FileNotFoundException e)
			{
				Log.WarnFormat("Expected devices file at '{0}' but none was found!", filePath);
				Log.Debug(e);

				TryWriteSampleFileTo(filePath);
			}
			catch (Exception e)
			{
				Log.ErrorFormat("Unable to read devices file at '{0}': {1}", filePath, e);
			}

			return devices;
		}

		private static void TryWriteSampleFileTo(string filePath)
		{
			using (var writer = XmlWriter.Create(filePath))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("devices");

				writer.WriteStartElement("device");
				writer.WriteAttributeString("id", "1");
				writer.WriteAttributeString("name", "Beispiel");
				writer.WriteEndElement();

				writer.WriteEndElement();
			}
		}
	}
}
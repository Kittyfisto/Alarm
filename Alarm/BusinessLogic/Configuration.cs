using System;
using System.Collections.Generic;
using System.Globalization;
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
	public sealed class Configuration
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly List<Device> _devices;
		private readonly Dictionary<int, Device> _devicesById;
		private readonly List<int> _temperatures;

		public Configuration()
		{
			_devicesById = new Dictionary<int, Device>();
			_temperatures = new List<int>();
			Read();
			_devices = _devicesById.Values.ToList();
		}

		public bool TryGetDevice(int id, out Device device)
		{
			return _devicesById.TryGetValue(id, out device);
		}

		public IReadOnlyList<Device> Devices => _devices;

		public IReadOnlyList<int> Temperatures => _temperatures;

		private void Read()
		{
			var filePath = Constants.ConfigurationFile;
			Log.InfoFormat("Reading devices from '{0}'...", filePath);

			try
			{
				if (!File.Exists(filePath))
				{
					Log.WarnFormat("Expected devices file at '{0}' but none was found!", filePath);
					TryWriteSampleFileTo(filePath);
				}

				using (var fileStream = File.OpenRead(filePath))
				using (var reader = XmlReader.Create(fileStream))
				{
					while (reader.Read())
						switch (reader.Name)
						{
							case "device":
								if (Device.TryRead(reader.ReadSubtree(), out var device))
									_devicesById.Add(device.Id, device);
								break;
							case "temperature":
								if (TryReadTemperature(reader.ReadSubtree(), out var temperature))
									_temperatures.Add(temperature);
								break;
						}
				}

				Log.InfoFormat("Read {0} devices in total", _devicesById.Count);
			}
			catch (Exception e)
			{
				Log.ErrorFormat("Unable to read devices file at '{0}': {1}", filePath, e);
			}
		}

		private static bool TryReadTemperature(XmlReader reader, out int temperature)
		{
			reader.Read();
			var value = reader.ReadElementContentAsString();
			if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out temperature))
				return true;

			for (int i = 0; i < reader.AttributeCount; ++i)
			{
				reader.MoveToAttribute(i);
				switch (reader.Name)
				{
					case "degrees":
						value = reader.Value;
						if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out temperature))
							return true;
						break;
				}
			}

			temperature = -1;
			return false;
		}

		private static void TryWriteSampleFileTo(string filePath)
		{
			using (var writer = XmlWriter.Create(filePath))
			{
				writer.WriteStartDocument();
				writer.WriteStartElement("configuration");

				writer.WriteStartElement("device");
				writer.WriteAttributeString("id", "1");
				writer.WriteAttributeString("name", "Beispiel");
				writer.WriteAttributeString("cycletime", "00:01:00");
				writer.WriteEndElement();

				writer.WriteElementString("temperature", "10");

				writer.WriteEndElement();
			}
		}
	}
}
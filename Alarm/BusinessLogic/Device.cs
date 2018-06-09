using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Reflection;
using System.Xml;
using log4net;

namespace Alarm.BusinessLogic
{
	public sealed class Device
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public int Id { get; set; }
		public string Name { get; set; }
		public TimeSpan CycleTime { get; set; }

		[Pure]
		public TimeSpan Runtime(int numIterations)
		{
			return TimeSpan.FromSeconds(CycleTime.TotalSeconds * numIterations);
		}

		public static bool TryRead(XmlReader reader, out Device device)
		{
			int? id = null;
			string name = null;
			TimeSpan? cycleTime = null;

			reader.Read();
			for (int i = 0; i < reader.AttributeCount; ++i)
			{
				reader.MoveToAttribute(i);
				switch (reader.Name)
				{
					case "id":
						if (int.TryParse(reader.Value, out var tmp))
							id = tmp;
						break;
					case "name":
						name = reader.Value;
						break;
					case "cycletime":
						if (TimeSpan.TryParse(reader.Value, CultureInfo.InvariantCulture, out var tmp2))
							cycleTime = tmp2;
						break;
				}
			}

			IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;
			int lineNumber = xmlInfo.LineNumber;
			if (id == null)
			{
				Log.WarnFormat("Missing id, skipping device on line '{0}'...", lineNumber);
				device = null;
				return false;
			}

			if (name == null)
			{
				Log.WarnFormat("Missing name, skipping device on line '{0}'...", lineNumber);
				device = null;
				return false;
			}

			if (cycleTime == null)
			{
				Log.WarnFormat("Missing cycleTime, skipping device on line '{0}'...", lineNumber);
				device = null;
				return false;
			}

			device = new Device
			{
				Id = id.Value,
				Name = name,
				CycleTime = cycleTime.Value
			};
			return true;
		}
	}
}
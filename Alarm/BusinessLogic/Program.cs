using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Alarm.BusinessLogic
{
	[DataContract]
	public sealed class Program
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Dictionary<int, DeviceProgram> DeviceConfiguration { get; set; }
	}
}

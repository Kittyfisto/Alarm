using System;
using System.Runtime.Serialization;

namespace Alarm.BusinessLogic
{
	[DataContract]
	public class DeviceProgram
	{
		[DataMember]
		public TimeSpan CycleTime { get; set; }
	}
}
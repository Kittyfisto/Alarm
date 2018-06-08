using System;
using System.Runtime.Serialization;

namespace Alarm.BusinessLogic
{
	[DataContract]
	public sealed class Alarm
	{
		[DataMember]
		public string SampleId { get; set; }

		private DateTime _endTime;

		[DataMember]
		public DateTime EndTime
		{
			get => _endTime;
			set => _endTime = value.Kind == DateTimeKind.Unspecified ? value.ToUniversalTime() : value;
		}

		[DataMember]
		public int DeviceId { get; set; }

		[DataMember]
		public int Temperature { get; set; }
	}
}

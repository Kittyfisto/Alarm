using System;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

namespace Alarm.BusinessLogic
{
	[DataContract]
	public sealed class Alarm
		: ICloneable
	{
		public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		[DataMember]
		public string SampleId { get; set; }

		[DataMember]
		public long EndTimeTimestamp { get; set; }

		public DateTime EndTime
		{
			get => Epoch + TimeSpan.FromTicks(EndTimeTimestamp);
			set => EndTimeTimestamp = (value - Epoch).Ticks;
		}

		[DataMember]
		public int DeviceId { get; set; }

		[DataMember]
		public int Temperature { get; set; }

		[DataMember]
		public bool SoundAlarm { get; set; }

		#region Implementation of ICloneable

		[Pure]
		public Alarm Clone()
		{
			return new Alarm
			{
				SampleId = SampleId,
				DeviceId = DeviceId,
				EndTime = EndTime,
				Temperature = Temperature,
				SoundAlarm = SoundAlarm
			};
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0} in {1}, {2} (Alarm {3})",
			                     SampleId, DeviceId,
			                     EndTime,
			                     SoundAlarm ? "on" : "off");
		}
	}
}

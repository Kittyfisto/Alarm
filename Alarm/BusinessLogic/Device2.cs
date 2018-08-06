using System.Runtime.Serialization;

namespace Alarm.BusinessLogic
{
	[DataContract(Name = "Device")]
	public sealed class Device2
	{
		[DataMember]
		public string Name { get; set; }
	}
}
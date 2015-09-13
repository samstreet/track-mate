using System;
using System.Runtime.Serialization;

namespace TrackMate
{
	[DataContract]
	public class RegisterDeviceRequest : Base
	{

		[DataMember (Name="auth_token")]
		public string auth_token { get; set;}

		[DataMember (Name="deviceId")]
		public string deviceId { get; set;}

	}
}


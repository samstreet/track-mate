using System;
using System.Runtime.Serialization;

namespace TrackMate
{
	[DataContract]
	public class LoginRequest : Base
	{

		[DataMember (Name="username")]
		public string userName { get; set;}

		[DataMember (Name="password")]
		public string password { get; set;}

	}
}


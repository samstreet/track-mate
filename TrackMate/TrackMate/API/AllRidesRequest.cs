using System;
using System.Runtime.Serialization;

namespace TrackMate
{
	[DataContract]
	public class AllRidesRequest : Base
	{
		[DataMember (Name="auth_token")]
		public string auth_token { get; set;}
	}
}


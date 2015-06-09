using System;
using System.Runtime.Serialization;

namespace TrackMate
{
	[DataContract]
	public class ReAuthenticateRequest : Base
	{

		[DataMember (Name="refresh_token")]
		public string refresh_token { get; set;}

	}
}


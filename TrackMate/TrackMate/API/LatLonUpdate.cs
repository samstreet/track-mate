using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.Generic;

namespace TrackMate
{
	[DataContract]
	public class LatLonUpdates : Base
	{
		[DataMember (Name="lat")]
		public string Lat { get; set;}

		[DataMember (Name="lon")]
		public string Lon {get; set;}

		[DataMember (Name="token")]
		public string token { get; set;}

		[DataMember (Name="auth_token")]
		public string auth_token { get; set;}

		[DataMember (Name="date")]
		public string date { get; set;}

	}
}


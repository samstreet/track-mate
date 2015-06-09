using System;
using System.Runtime.Serialization;

namespace TrackMate
{
	[DataContract]
	public class NewRideRequest : Base{

		// user auth token
		[DataMember (Name="auth_token")]
		public string auth_token { get; set;}

		[DataMember (Name="start")]
		public DateTime start_time { get; set;}

		[DataMember (Name="end")]
		public DateTime end_time { get; set;}

		[DataMember (Name="duration")]
		public string duration { get; set;}

		[DataMember (Name="distance")]
		public double distance { get; set;}

		[DataMember (Name="token")]
		public string token { get; set;}

		[DataMember (Name="lat_lon_points")]
		public List<string> lat_lon_points { get; set;}

		[DataMember (Name="viewable")]
		public string viewable { get; set;}

		[DataMember (Name="password")]
		public string password { get; set;}


	}
}


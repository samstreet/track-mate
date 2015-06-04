using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections.Generic;

namespace TrackMate
{
	[DataContract]
	public class Register
	{
		public string CreateJson(){
			MemoryStream mem = new MemoryStream ();
			DataContractJsonSerializer json = new DataContractJsonSerializer (this.GetType());
			json.WriteObject (mem, this);
			mem.Seek (0, SeekOrigin.Begin);
			StreamReader stream = new StreamReader (mem);
			return stream.ReadToEnd ();
		}
	}

	[DataContract]
	public class RegisterRequest : Register{

		[DataMember (Name="firstName")]
		public string firstName { get; set;}

		[DataMember (Name="lastName")]
		public string lastName { get; set;}

		[DataMember (Name="userName")]
		public string userName { get; set;}

		[DataMember (Name="password")]
		public string password { get; set;}

		[DataMember (Name="email")]
		public string email { get; set;}

	}

	[DataContract]
	public class LoginRequest : Register{

		[DataMember (Name="username")]
		public string userName { get; set;}

		[DataMember (Name="password")]
		public string password { get; set;}

	}

	[DataContract]
	public class ReAuthenticateRequest : Register{

		[DataMember (Name="refresh_token")]
		public string refresh_token { get; set;}

	}

	[DataContract]
	public class NewRideRequest : Register{

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


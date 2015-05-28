using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

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

		[DataMember (Name="userName")]
		public string userName { get; set;}

		[DataMember (Name="password")]
		public string password { get; set;}

	}
}


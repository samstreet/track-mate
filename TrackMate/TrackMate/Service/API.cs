using System;
using System.Net;

namespace TrackMate
{
	public class API
	{
		public async static HttpWebRequest CreateWebRequest()
		{
			// local
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://samstreet.wearenifty.co.uk/api/");
			// live 
			// HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"http://samstreet.wearenifty.co.uk/api/");
			webRequest.ContentType = "text/json";
			webRequest.Accept = "text/json";
			webRequest.Method = "POST";
			return webRequest;
		}
	}
}


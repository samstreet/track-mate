using System;
using System.Net;
using Android.App;


namespace TrackMate
{

	// this class will allow for updates to be posted to the server upon completion of the tracking
	[Service]
	public class Request : Service
	{
		public Request ()
		{
			throw new NotImplementedException ();
		}

		// create a web request
		// need to change the params of this shiz
		public static HttpWebRequest CreateWebRequest()
		{
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(@"https://lite.realtime.nationalrail.co.uk/OpenLDBWS/ldb6.asmx");
			webRequest.Headers.Add(@"SOAP:Action");
			webRequest.ContentType = "text/xml;charset=\"utf-8\"";
			webRequest.Accept = "text/xml";
			webRequest.Method = "POST";
			return webRequest;
		}
	}
}


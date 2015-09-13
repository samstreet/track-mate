using System;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace TrackMate
{
	public class API
	{

		public static String MakeAPICall(string endpoint){

			string result;

			var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://samstreet.niftydigital.co.uk/api/v1/" + endpoint);
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";

			using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string json = "{\"user\":\"test\"," +
					"\"password\":\"bla\"}";

				streamWriter.Write(json);
				streamWriter.Flush();
				streamWriter.Close();
			}

			var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
			{
				result = streamReader.ReadToEnd();
			}

			Android.Util.Log.Info ("result", result);

			return result;
		}

		public static HttpWebRequest CreateWebRequest(string endpoint)
		{
			// local
			HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://samstreet.niftydigital.co.uk/api/v1/" + endpoint);
			// live 
			// HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://samstreet.wearenifty.co.uk/api/");
			webRequest.ContentType = "application/json";
			webRequest.Accept = "application/json";
			webRequest.Method = "POST";
			return webRequest;
		}
	}
}


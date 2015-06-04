using System;
using System.Net;
using Android.App;
using System.Threading.Tasks;
using System.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http;
using System.Text;


namespace TrackMate
{

	// this class will allow for updates to be posted to the server upon completion of the tracking
	public class Request
	{
		
		public async Task<string> makeRequest (string endpoint, string method, string data)
		{
			HttpClient request = new HttpClient (); 
			
			// default params
			request.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
			string url = "http://samstreet.niftydigital.co.uk/api/v1/" + endpoint + "";

			// force method to be upper using - method.ToUpper();
			if (method.ToUpper() == "POST") {

				var jsonResponse = await request.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json")); 
				return jsonResponse.Content.ReadAsStringAsync().Result;

			} else {
				var jsonResponse = await request.GetAsync(url);

				return jsonResponse.Content.ReadAsStringAsync().Result;
			}

			// else { write a seperate GET handler }
		}
}
}

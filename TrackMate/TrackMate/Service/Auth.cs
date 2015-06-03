using System;
using System.Threading.Tasks;
using Xamarin.Auth;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Android.Content;

namespace TrackMate
{
	public static class Auth
	{
		public static bool isAuthTokenValid(DateTime auth_expires){
			var now = new DateTime ();

			if (now < auth_expires)
				return true;

			return false;
		}

		public static bool isRefreshTokenValid(DateTime refresh_token) {
			var now = new DateTime ();

			if (now < refresh_token)
				return true;

			return false;
		}



		public static async Task<bool> isUserValid(Account user, Context context){
			// we have an account, now we need to see if the auth_token is still valid
			// if it isn't we ask for a refresh by using our refresh token
			// if the refresh_token isn't valid, then we log them out

			var request = new Request ();

			// push the account properties to a Dictionary, meaning we can access key value pairs
			Dictionary<string, string> user_properties = user.Properties;

			var auth_token = user_properties["auth_token"];
			var refresh_token = user_properties["refresh_token"];
			var auth_expires = Convert.ToDateTime(user_properties["auth_token_expires"]);
			var refresh_token_expires = Convert.ToDateTime(user_properties["refresh_token_expires"]);

			// monster if statement
			if (isAuthTokenValid (auth_expires) && isRefreshTokenValid (refresh_token_expires)) {
				return true;
			} else if (isRefreshTokenValid (refresh_token_expires) && !isAuthTokenValid(auth_expires)) {
				// update the auth token so we can keep the user perpetually logged in

				var re_auth = new ReAuthenticateRequest ();
				re_auth.refresh_token = user_properties ["refresh_token"];

				string json = re_auth.CreateJson ();

				Task<string> response = request.makeRequest ("re-authenticate", "POST", json);

				string data = await response;

				var auth = JObject.Parse(data);


				var success = Convert.ToBoolean(auth["success"]);

				if(success){
					user.Properties["auth_token"] = auth["data"]["auth_token"]["token"].ToString();
					user.Properties["refresh_token"] = auth["data"]["refresh_token"]["token"].ToString();
					user.Properties["auth_token_expires"] = auth["data"]["auth"]["auth_expires"]["date"].ToString();
					user.Properties["refresh_token_expires"] = auth["data"]["refresh"]["refresh_expires"]["date"].ToString();

					AccountStore.Create(context).Save(user, "TrackMate");
				}

				return true;
			} else {
				return false;
			}
		}
	}
}


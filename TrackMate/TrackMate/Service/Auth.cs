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
		// check to see if the auth token on the device is still valid
		// compare a DateTime of now against a DateTime of the token expire
		public static bool isAuthTokenValid(DateTime auth_expires){
			var now = DateTime.Now;

			if (now < auth_expires)
				return true;

			return false;
		}

		// check to see if the refresh token on the device is still valid
		// compare a DateTime of now against a DateTime of the token expire
		public static bool isRefreshTokenValid(DateTime refresh_token) {
			var now = DateTime.Now;

			if (now < refresh_token)
				return true;

			return false;
		}


		// validate if the user is still valid via the auth and refresh tokens.
		// pass in the user account to validate and the context.
		// checks if the tokens are valid
		// if it isn't we ask for a refresh by using our refresh token
		// if the refresh_token isn't valid, then we log them out
		public static async Task<bool> isUserValid(Account user, Context context){

			// new Request instance
			var request = new Request ();

			// push the account properties to a Dictionary, meaning we can access key value pairs
			Dictionary<string, string> user_properties = user.Properties;

			// only need to get the user current 
			// theses are currently in a string format, make them a DateTime
			var auth_expires = Convert.ToDateTime(user_properties["auth_token_expires"]);
			var refresh_token_expires = Convert.ToDateTime(user_properties["refresh_token_expires"]);

			// If both tokens are valid, return true
			if (isAuthTokenValid (auth_expires) && isRefreshTokenValid (refresh_token_expires)) {
				return true;

				// if the refresh token is valid and the auth token isn't make a re-authenticate request
				// if the update is successful return tue, if not, return false (this will log them out
			} else if (isRefreshTokenValid (refresh_token_expires) && !isAuthTokenValid(auth_expires)) {

				// new ReAuthenticateRequest
				var re_auth = new ReAuthenticateRequest ();
				re_auth.refresh_token = user_properties ["refresh_token"];

				// generate the JSON
				string json = re_auth.CreateJson ();

				// prep the async method with the data
				Task<string> response = request.makeRequest ("re-authenticate", "POST", json);

				// await a response
				string data = await response;

				// parse the data
				var auth = JObject.Parse(data);

				// start getting params out of the JSON
				// convert the string to a bool
				var success = Convert.ToBoolean(auth["success"]);

				// if the update was successful, update the user params
				if (success) {
					user.Properties ["auth_token"] = auth ["data"] ["auth_token"] ["token"].ToString ();
					user.Properties ["refresh_token"] = auth ["data"] ["refresh_token"] ["token"].ToString ();
					user.Properties ["auth_token_expires"] = auth ["data"] ["auth"] ["auth_expires"] ["date"].ToString ();
					user.Properties ["refresh_token_expires"] = auth ["data"] ["refresh"] ["refresh_expires"] ["date"].ToString ();

					// update the account with the new details
					AccountStore.Create (context).Save (user, "TrackMate");
				} else {
					// if we get this far, something has gone wrong on the server and we assume
					// the tokens are not valid, therefore we log them out
					return false;
				}

				// all went well, return true
				return true;
			} else {
				// nothing is valid at this point and the user must log in again
				return false;
			}
		}
	}
}


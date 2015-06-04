
using System;
using System.Collections.Generic;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace TrackMate
{
	[Activity (Label = "TrackMate", MainLauncher = true,Icon = "@drawable/icon")]			
	public class LoginActivity : Activity
	{
		protected async override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			bool isValid = false;

			// check if an account exists if true: force to main activ
			IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");

			if(accounts.Any())
				isValid = await Auth.isUserValid (accounts.FirstOrDefault (), this);

			if (accounts.Any () && isValid) {
				var mainActivity = new Intent (this, typeof(MainActivity));
				StartActivity (mainActivity);

			} else {



				SetContentView (Resource.Layout.Login);

				Button login = FindViewById<Button> (Resource.Id.login);
				Button register = FindViewById<Button> (Resource.Id.register);
				TextView username = FindViewById<TextView> (Resource.Id.username);
				TextView password = FindViewById<TextView> (Resource.Id.password);

				// testing only 
				TextView output = FindViewById<TextView> (Resource.Id.output);

				register.Click += delegate {
					var registerActivity = new Intent (this, typeof(RegisterActivity));
					StartActivity (registerActivity);
				};

				login.Click += async (object sender, EventArgs e) => {
					var request = new Request ();

					var loginRequest = new LoginRequest ();
					loginRequest.userName = username.Text;
					loginRequest.password = password.Text;

					string postBody = loginRequest.CreateJson (); 

					Task<string> response = request.makeRequest ("authenticate", "POST", postBody);

					string value = await response;

					var auth = JObject.Parse (value);

					string success = auth ["success"].ToString ();

					if (success.ToLower () == "true") {

						// generate a new account with the details we pass back form the api
						var user = new Account ();
						user.Username = username.Text;
						user.Properties.Add ("auth_token", auth ["data"] ["auth"] ["token"].ToString ());
						user.Properties.Add ("auth_token_expires", auth ["data"] ["auth"] ["auth_expires"] ["date"].ToString ());
						user.Properties.Add ("refresh_token", auth ["data"] ["refresh"] ["token"].ToString ());
						user.Properties.Add ("refresh_token_expires", auth ["data"] ["refresh"] ["refresh_expires"] ["date"].ToString ());

						// run in the background?
						// I think this will work?
						AccountStore.Create (this).Save (user, "TrackMate");

						// push out to the main activity
						var mainActivity = new Intent (this, typeof(MainActivity));
						StartActivity (mainActivity);

					} else {
						string error = auth ["error"].ToString ();
						output.Text = error;
					}


				};

			}
		}
	}
}


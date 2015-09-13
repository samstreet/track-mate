
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
using Gcm.Client;
using Geolocator.Plugin;
using Android.Util;

namespace TrackMate
{
	[Activity (Label = "TrackMate", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar")]			
	public class LoginActivity : Activity
	{

		bool isValid = false;



		protected async override void OnCreate (Bundle bundle)
		{
			

			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Login);

			// check if an account exists if true: force to main activ
			IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");

			if (accounts.Any ())
				isValid = await Auth.isUserValid (accounts.FirstOrDefault (), this);

			if (accounts.Any () && isValid) {
				var loginActivity = new Intent (this, typeof(LoginActivity));
				StartActivity (loginActivity);

			} else {

				Button login = FindViewById<Button> (Resource.Id.login);
				Button register = FindViewById<Button> (Resource.Id.register);
				TextView username = FindViewById<TextView> (Resource.Id.username);
				TextView password = FindViewById<TextView> (Resource.Id.password);
				ProgressBar attempting = FindViewById<ProgressBar> (Resource.Id.progress);
				register.Visibility = Android.Views.ViewStates.Gone;

				// errors only 
				TextView output = FindViewById<TextView> (Resource.Id.output);

				register.Click += delegate {
					var registerActivity = new Intent (this, typeof(RegisterActivity));
					StartActivity (registerActivity);
				};

				login.Click += async (object sender, EventArgs e) => {

					// reset the value to empty
					output.Visibility = Android.Views.ViewStates.Gone;
					attempting.Visibility = Android.Views.ViewStates.Visible;

					var request = new Request ();

					var loginRequest = new LoginRequest ();
					loginRequest.userName = username.Text;
					loginRequest.password = password.Text;

					string postBody = loginRequest.CreateJson (); 

					try {
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
							user.Properties.Add ("user_device_id", ""); // add this as default


							// run in the background?
							// I think this will work?
							AccountStore.Create (this).Save (user, "TrackMate");

							// GCM 
							GcmClient.CheckDevice(this);
							GcmClient.CheckManifest(this);
							// wait until now to register the device as we need to be able to have an account store with the users details
							GcmClient.Register(this, GcmBroadcastReceiver.SENDER_IDS);


							// push out to the main activity
							var mainActivity = new Intent (this, typeof(MainActivity));
							StartActivity (mainActivity);


						} else {
							attempting.Visibility = Android.Views.ViewStates.Gone;
							string error = auth ["error"].ToString ();
							output.Text = error;
						}

					} catch (Exception ex) {
						attempting.Visibility = Android.Views.ViewStates.Gone;
						output.Text = "There appears to be a problem, try again later";
					}
				};

			}
		}
	}
}


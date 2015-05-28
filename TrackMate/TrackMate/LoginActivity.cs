
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
	[Activity (Label = "TrackMate" , Icon = "@drawable/icon")]			
	public class LoginActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Login);

			Button login = FindViewById<Button>(Resource.Id.login);
			Button register = FindViewById<Button>(Resource.Id.register);
			TextView username = FindViewById<TextView> (Resource.Id.username);
			TextView password = FindViewById<TextView> (Resource.Id.password);

			// testing only 
			TextView output = FindViewById<TextView> (Resource.Id.output);

			register.Click += delegate {
				var registerActivity = new Intent (this, typeof(RegisterActivity));
				StartActivity(registerActivity);
			};

			login.Click += async (object sender, EventArgs e) => {
				var request = new Request();

				var loginRequest = new LoginRequest();
				loginRequest.userName = username.Text;
				loginRequest.password = password.Text;

				string postBody = loginRequest.CreateJson(); 

				Task<string> response = request.makeRequest("authenticate", "POST", postBody);

				string value = await response;

				var auth = JObject.Parse(value);

				string success = auth["success"].ToString();

				if(success.ToLower() == "true"){

					User userObject = new User();
					userObject.firstName = auth["data"]["user"]["firstName"].ToString();

					var user = new Account();
					user.Username = auth["data"]["user"]["userName"].ToString();
					user.Properties.Add("username", auth["data"]["user"]["userName"].ToString());
					user.Properties.Add("password", auth["data"]["user"]["password"].ToString());
					user.Properties.Add("allDetails", auth.ToString());

					AccountStore.Create(this).Save(user, "TrackMate");

					var mainActivity = new Intent (this, typeof(MainActivity));
					StartActivity(mainActivity);

				} else {
					string error = auth["error"].ToString();
					output.Text = error;
				}


			};

			// Create your application here
		}
	}
}


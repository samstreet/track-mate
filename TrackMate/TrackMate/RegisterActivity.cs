
using System;

using Android.App;
using Android.OS;
using Android.Widget;
using System.Threading.Tasks;

namespace TrackMate
{
	[Activity (Label = "Register", Theme = "@android:style/Theme.NoTitleBar")]			
	public class RegisterActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Register);

			TextView firstName = FindViewById<TextView> (Resource.Id.firstName);
			TextView lastName = FindViewById<TextView> (Resource.Id.lastName);
			TextView username = FindViewById<TextView> (Resource.Id.username);
			TextView password = FindViewById<TextView> (Resource.Id.password);
			TextView email = FindViewById<TextView> (Resource.Id.email);

			// testing only 
			TextView output = FindViewById<TextView> (Resource.Id.output);

			Button registerButton = FindViewById<Button>(Resource.Id.submit);

			registerButton.Click += async (object sender, EventArgs e) => {
				var request = new Request();

				var register = new RegisterRequest();
				register.email = email.Text;
				register.firstName = firstName.Text;
				register.lastName = lastName.Text;
				register.password = password.Text;
				register.userName = username.Text;

				string postBody = "{ \"user\" : " + register.CreateJson() + "}"; 

				Task<string> response = request.makeRequest("new-user", "POST", postBody);

				string value = await response;

				output.Text = value;

				// need to implement the new account data here, store auth tokens etc.
			};
		}
	}
}


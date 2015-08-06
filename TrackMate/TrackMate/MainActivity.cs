using System;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Xamarin.Auth;
using System.Linq;
using System.Threading.Tasks;

namespace TrackMate
{
	[Activity (Label = "TrackMate", Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar")]
	public class MainActivity : Activity
	{

		protected async override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// check if an account exists if true: force to main activ
			IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");

			bool isValid = await Auth.isUserValid (accounts.FirstOrDefault (), this);

			if (!accounts.Any () && !isValid) {
				var loginActivity = new Intent (this, typeof(LoginActivity));
				StartActivity (loginActivity);

			} else {
				

				if (isValid) {
					// assume everything is okay and continue with the process



					// Get our button from the layout resource,
					// and attach an event to it
					Button button = FindViewById<Button> (Resource.Id.myButton);
					Button logout = FindViewById<Button> (Resource.Id.logout);
					Button rides = FindViewById<Button> (Resource.Id.rides);

					button.Click += async (object sender, EventArgs e) => {
						var startTrackingActivity = new Intent (this, typeof(StartTrackingActivity));
						StartActivity (startTrackingActivity);
					};

					rides.Click += async (object sender, EventArgs e) => {
						var startRidesActivity = new Intent (this, typeof(RidesActivity));
						StartActivity (startRidesActivity);
					};

					logout.Click += delegate {
						var account = AccountStore.Create (this).FindAccountsForService ("TrackMate");

						if (account.Any ()) {
							AccountStore.Create (this).Delete (account.FirstOrDefault (), "TrackMate");

							var startLoginActivity = new Intent (this, typeof(LoginActivity));
							StartActivity (startLoginActivity);
						}
					};
				} else {
					var startLoginActivity = new Intent (this, typeof(LoginActivity));
					StartActivity (startLoginActivity);
				}
			}
		}

		protected override void OnPause()
		{
			base.OnPause();
		}


		// the user is resuming the application, check if the user is still valid
		protected async override void OnResume()
		{
			// check if an account exists if true: force to main activ
			IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");

			bool isValid = await Auth.isUserValid (accounts.FirstOrDefault (), this);

			if (!accounts.Any () && !isValid) {
				var loginActivity = new Intent (this, typeof(LoginActivity));
				StartActivity (loginActivity);

			}

			base.OnPause();
		}

		protected override void OnDestroy ()
		{
			base.OnDestroy ();
		}

	}
}



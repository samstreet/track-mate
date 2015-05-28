using System;

using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Xamarin.Auth;
using System.Linq;

namespace TrackMate
{
	[Activity (Label = "TrackMate", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// check if an account exists if true: force to main activ
			IEnumerable<Account> accounts = AccountStore.Create(this).FindAccountsForService("TrackMate");
			if (!accounts.Any ()) {
				var loginActivity = new Intent (this, typeof(LoginActivity));
				StartActivity(loginActivity);
			}

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			button.Click += delegate {
				var startTrackingActivity = new Intent (this, typeof(StartTrackingActivity));
				StartActivity(startTrackingActivity);
			};
		}


	}
}



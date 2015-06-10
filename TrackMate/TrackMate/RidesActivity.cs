
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

// a collection of rides
using Xamarin.Auth;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace TrackMate
{
	[Activity (Label = "RidesActivity")]			
	public class RidesActivity : Activity
	{

		bool isValid = false;
		// check if an account exists if true: force to main activ

		protected async override void OnCreate (Bundle bundle)
		{
			IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");

			if(accounts.Any())
				isValid = await Auth.isUserValid (accounts.FirstOrDefault (), this);

			if (accounts.Any () && isValid) {
				var mainActivity = new Intent (this, typeof(MainActivity));
				StartActivity (mainActivity);

			} else {

				base.OnCreate (bundle);

				SetContentView (Resource.Layout.Rides);

				var getAllRides = new AllRidesRequest ();
				getAllRides.auth_token = accounts.FirstOrDefault().Properties["auth_token"];

				var json = getAllRides.CreateJson ();

				var request = new Request ();

				Task<string> response = request.makeRequest ("rides/all", "POST", json);

				string value = await response;

				var rides = JObject.Parse (value);

				// should conv. to bool
				if(rides["success"].ToString() == "true"){
					// echo out all rides
				} else {
					// something went wrong
				}

			}

			// Create your application here
		}
	}
}


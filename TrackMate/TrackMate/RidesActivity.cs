
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
using Android.Util;


namespace TrackMate
{
	[Activity (Label = "RidesActivity")]			
	public class RidesActivity : Activity
	{

		bool isValid = false;
		// check if an account exists if true: force to main activ

		protected async override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");

			if(accounts.Any())
				isValid = await Auth.isUserValid (accounts.FirstOrDefault (), this);

			if (!accounts.Any () && !isValid) {
				var loginActivity = new Intent (this, typeof(LoginActivity));
				StartActivity (loginActivity);

			} else {

//				SetContentView (Resource.Layout.Rides);

				var getAllRides = new AllRidesRequest ();
				getAllRides.auth_token = accounts.FirstOrDefault().Properties["auth_token"];

				var json = getAllRides.CreateJson ();

				var request = new Request ();

				try{
					var url = "rides/all/" + accounts.FirstOrDefault().Properties["auth_token"];

					Task<string> response = request.makeRequest (url, "GET", json);

					string value = await response;

					var rides = JObject.Parse (value);

					var user_rides = rides["data"]["rides"];



				} catch(Exception ex){
					
				}


			}

			// Create your application here
		}
	}
}


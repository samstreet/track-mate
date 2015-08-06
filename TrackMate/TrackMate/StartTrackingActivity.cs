
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Util;
using Android.Provider;
using System.Collections.Generic;
using Xamarin.Auth;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Timers;

namespace TrackMate
{
	[Activity (Label = "TrackMate", Icon = "@drawable/icon")]			
	public class StartTrackingActivity : Activity
	{
		readonly string logTag = "MainActivity";

		// variables accessible by the class
		LocationManager locMgr;
		Button start;
		Button stop;
		TextView longitude;
		TextView latitude;
		TextView locationStatus;
		TextView distanceTravelled;
		TextView output;
		//string Provider; // the GPS Provider
		List<string> lat_lon_points = new List<string>();
		double travelled = 0;
		double lastLat = 0.0;
		double lastLon = 0.0;
		Journey thisJourney = new Journey();
		string ride_token = "";

		protected async override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.StartTracking);

			// check if an account exists if true: force to main activ
			IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");

			bool isValid = await Auth.isUserValid (accounts.FirstOrDefault (), this);

			var ride = new NewRideRequest();
			var request = new Request();

			if (!accounts.Any () && !isValid) {
				var loginActivity = new Intent (this, typeof(LoginActivity));
				StartActivity (loginActivity);

			} else {



				// get the location service 
				locMgr = GetSystemService (Context.LocationService) as LocationManager;

				// could move this onto its own method
				if (!locMgr.IsProviderEnabled (LocationManager.GpsProvider) ||
				    !locMgr.IsProviderEnabled (LocationManager.NetworkProvider)) {

					notifyUser_gps_disabled ();

				} else {
					notififyUser_location_attained ();
				}



				App.Current.LocationServiceConnected += (object sender, ServiceConnectedEventArgs e) => {
					Log.Debug (logTag, "ServiceConnected Event Raised");
					// notifies us of location changes from the system
					App.Current.LocationService.LocationChanged += HandleLocationChanged;
					//notifies us of user changes to the location provider (ie the user disables or enables GPS)
					App.Current.LocationService.ProviderEnabled += HandleProviderEnabled;
					// notifies us of the changing status of a provider (ie GPS no longer available)
					App.Current.LocationService.StatusChanged += HandleStatusChanged;
				};

				// view related vars
				start = FindViewById<Button>(Resource.Id.startTracking);
				stop = FindViewById<Button>(Resource.Id.stopTracking);
				latitude = FindViewById<TextView> (Resource.Id.latitude);
				longitude = FindViewById<TextView> (Resource.Id.longitude);
				locationStatus = FindViewById<TextView> (Resource.Id.locationStatus);
				distanceTravelled = FindViewById<TextView> (Resource.Id.distanceTravelled);

				output = FindViewById<TextView> (Resource.Id.output);

				// hide it all
				stop.Visibility = ViewStates.Invisible;

				// handle if the button can be clicked if the location isn't enabled
				start.Visibility = ViewStates.Visible;

				// work in progress, doesn't seem to update on the fly
//				if (!locMgr.IsProviderEnabled (LocationManager.GpsProvider) ||
//				    !locMgr.IsProviderEnabled (LocationManager.NetworkProvider)) {
//					// Build the alert dialog
//					start.Enabled = false;
//				} else {
//					start.Enabled = true;
//				}

				latitude.Visibility = ViewStates.Invisible;
				longitude.Visibility = ViewStates.Invisible;
				distanceTravelled.Visibility = ViewStates.Invisible;

				start.Click += async (object sender, EventArgs e) => {
					start.Enabled = false;
					// firstly make a request to create a new ride
					// the user auth_token as we are valid at this point
					ride.auth_token = accounts.FirstOrDefault().Properties["auth_token"];

					var json = "{ \"user\" : " + ride.CreateJson() + "}";

					Task<string> response = request.makeRequest ("new-ride", "POST", json);

					string value = await response;

					var auth = JObject.Parse (value);

					if(Convert.ToBoolean(auth["success"].ToString())){

						// notify the user that it is in progress
						notififyUser_activity_in_progress();

						ride_token = auth["data"]["ride"]["token"].ToString();

						// start the timer going
						// Create a 5 min timer 
						var timer = new Timer(3000);


						// Hook up the Elapsed event for the timer.
						timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

						timer.Enabled = true;

						stop.Visibility = ViewStates.Visible;
						start.Visibility = ViewStates.Visible;
						latitude.Visibility = ViewStates.Visible;
						longitude.Visibility = ViewStates.Visible;
						distanceTravelled.Visibility = ViewStates.Visible;
					} else {

						// 
						system_response(auth.ToString());


					}

				};

				stop.Click += delegate {



					//var stopTrackingActivity = new Intent (this, typeof(StopTrackingActivity));

					// finsih the ride and ammend the data to be sent
					ride.lat_lon_points = lat_lon_points;
					ride.distance = travelled;
					ride.end_time = DateTime.Now.ToString("G");

					// make the request
					var ride_json = "{ \"ride\" : " + ride.CreateJson() + "}";

					// this is just a holder to hold see the data and response
					AlertDialog.Builder builder = new AlertDialog.Builder(this);
					builder.SetTitle("Lat Lon points");
					builder.SetMessage(ride_json);
					Dialog alertDialog = builder.Create();
					alertDialog.SetCanceledOnTouchOutside(true);

					// update on the UI thread
					RunOnUiThread (() => {
						builder.Show();
					} );

					//var save_ride = request.makeRequest("save-ride", "POST", ride_json);

					//StartActivity(stopTrackingActivity);
				};
			}
		}

		protected override void OnPause()
		{
			Log.Debug (logTag, "Location app is moving to background");
			base.OnPause();
		}

		protected override void OnResume()
		{
			Log.Debug (logTag, "Location app is moving into foreground");
			base.OnPause();
		}

		protected override void OnDestroy ()
		{
			Log.Debug (logTag, "Location app is becoming inactive");
			base.OnDestroy ();
		}

		// calculations for distance travelled
		// there are some issues with the way this is calculated and will require further investigation
		// the bacground process seems to want to constantly update this even if the lats and lons
		// are the same.
		public void calculateDistanceTravelled(double currentLat, double currentLon, double lastLat, double lastLon, char unit = 'K'){
			double rlat1 = Math.PI*currentLat/180;
			double rlat2 = Math.PI*lastLat/180;

			double theta = lastLon - currentLon ;

			double rtheta = Math.PI*theta/180;
			double dist = 
				Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) * 
				Math.Cos(rlat2) * Math.Cos(rtheta);


			dist = Math.Acos(dist);
			dist = dist*180/Math.PI;

			double newDistance = travelled + dist;

			string travelUnit = "Km";

			switch (unit)
			{
			case 'K': //Kilometers -> default
				travelled = Math.Round(newDistance * 1.609344, 2);
				travelUnit = "Km";
				break;
			case 'N': //Nautical Miles 
				travelled = Math.Round(newDistance * 0.8684, 2);
				travelUnit = "Nm";
				break;
			case 'M': //Miles
				travelled = Math.Round(newDistance *60*1.1515, 2);
				travelUnit = "Miles";
				break;
			}


			distanceTravelled.Text = string.Format ("Distance travelled: {0} {1}", travelled, travelUnit);
		}
		
		public void HandleLocationChanged(object sender, LocationChangedEventArgs e)
		{
			Android.Locations.Location location = e.Location;
			Log.Debug (logTag, "Foreground updating");

			latitude.Text = "Latitude: " + location.Latitude;
			longitude.Text = "Longitude: " + location.Longitude;
			locationStatus.Text = "Location: Attained";

			// set them up
			if (lastLat == 0.0)
				lastLat = location.Latitude;
			thisJourney.startLat = location.Latitude;
			lastLat = location.Latitude;

			if (lastLon == 0.0)
				lastLon = location.Longitude;
			thisJourney.startLon = location.Longitude;
			lastLon = location.Longitude;

			// generate a new lat lon DataContract object in order to have params pushed into it
			string points = new LatLonPoints (){ Lat = location.Latitude.ToString(), Lon = location.Longitude.ToString() }.ToJSON();
 			
			// not working
			if(location.Latitude != lastLat || location.Longitude != lastLon){
				// push the lat and lon updates to the list, we only want to do this once a change has occured
				lat_lon_points.Add(points);
				// calculate the distance travelled
				calculateDistanceTravelled (location.Latitude, location.Longitude, lastLat, lastLon);
			}
			// these events are on a background thread, need to update on the UI thread
			RunOnUiThread (() => {
				latitude.Text = String.Format ("Latitude: {0}", location.Latitude);
				longitude.Text = String.Format ("Longitude: {0}", location.Longitude);
			});

		}

		public void HandleProviderDisabled(object sender, ProviderDisabledEventArgs e)
		{
			Log.Debug (logTag, "Location provider disabled event raised");
			locationStatus.Text = "provider disabled";

			// Build the alert dialog
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle("Location Services Not Active");
			builder.SetMessage("Please enable Location Services and GPS");
			builder.SetPositiveButton ("Good", (senderAlert, args) => {
				Intent intent = new Intent(Settings.ActionLocationSourceSettings);
				StartActivity(intent);
			} );
			Dialog alertDialog = builder.Create();
			alertDialog.SetCanceledOnTouchOutside(false);

			RunOnUiThread (() => {
				builder.Show();
			} );

		
		}

		public void HandleProviderEnabled(object sender, ProviderEnabledEventArgs e)
		{
			Log.Debug (logTag, "Location provider enabled event raised");
			locationStatus.Text = "provider enabled";
		}

		public void HandleStatusChanged(object sender, StatusChangedEventArgs e)
		{
			Log.Debug (logTag, "Location status changed, event raised");
			locationStatus.Text = "status changed";
		}

		// when the timer has run it's course, we fire off a request to the server with the 
		// latest known position of a rider
		// this will return a response that may sugegst the rider has been in the location 
		// for an extended period of time and we may need to provide assisstance.
		public void OnTimedEvent(object source, ElapsedEventArgs e){
			updateLatLonPoints(ride_token, lastLat.ToString(), lastLon.ToString(), DateTime.Now.ToString());
		}

		public async void updateLatLonPoints(string token, string lat, string lon, string date){
			var request = new Request();

			var auth_token = AccountStore.Create (this).FindAccountsForService ("TrackMate").FirstOrDefault ().Properties ["auth_token"];

			var update = new LatLonUpdates ();
			update.Lat = lat;
			update.Lon = lon;
			update.token = token;
			update.date = date;
			update.auth_token = auth_token.ToString ();

			var json = update.CreateJson ();

			Task<string> update_points = request.makeRequest ("update/location", "POST", json);

			await update_points;
		}


		public void notififyUser_location_attained(){

			Notification.Builder builder = new Notification.Builder(this)
				.SetAutoCancel (false)
				.SetContentTitle ("Location attained")
				.SetSmallIcon(Resource.Drawable.Icon)
				.SetContentText ("we have your location");

			// Finally publish the notification
			NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Notify(1, builder.Build());

		}

		public void notififyUser_activity_in_progress(){

			Notification.Builder builder = new Notification.Builder(this)
				.SetAutoCancel (false)
				.SetContentTitle ("Activityin progress")
				.SetSmallIcon(Resource.Drawable.Icon)
				.SetOngoing(true)
				.SetContentText ("Your activity is in progress");

			// Finally publish the notification
			NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Notify(1, builder.Build());

		}


		public void notifyUser_gps_disabled(){
			// Build the alert dialog
			AlertDialog.Builder builder = new AlertDialog.Builder (this);
			builder.SetTitle ("Location Services Not Active");
			builder.SetMessage ("Please enable Location Services and GPS");
			builder.SetPositiveButton ("Ok", (senderAlert, args) => {
				Intent intent = new Intent (Settings.ActionLocationSourceSettings);
				StartActivity (intent);
			});
			Dialog alertDialog = builder.Create ();
			alertDialog.SetCanceledOnTouchOutside (false);

			RunOnUiThread (() => {
				builder.Show ();
			});
		}

		public void system_response(string message){
			AlertDialog.Builder builder = new AlertDialog.Builder(this);
			builder.SetTitle("Server Response");
			builder.SetMessage(message);
			Dialog alertDialog = builder.Create();
			alertDialog.SetCanceledOnTouchOutside(true);

			RunOnUiThread (() => {
				builder.Show();
			} );
		}
	}
}


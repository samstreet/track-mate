
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Util;
using Android.Provider;

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
		//string Provider; // the GPS Provider
		double travelled = 0;
		double lastLat = 0.0;
		double lastLon = 0.0;
		Journey thisJourney = new Journey();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.StartTracking);

			// get the location service 
			locMgr = GetSystemService (Context.LocationService) as LocationManager;

			// could move this onto its own method
			if (!locMgr.IsProviderEnabled (LocationManager.GpsProvider) ||
				!locMgr.IsProviderEnabled (LocationManager.NetworkProvider)) {
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

			start.Click += delegate {
				stop.Visibility = ViewStates.Visible;
			};

			stop.Click += delegate {
				var stopTrackingActivity = new Intent (this, typeof(StopTrackingActivity));

				// push to a model
				thisJourney.distanceTravelled = travelled;
				thisJourney.endLat = lastLat;
				thisJourney.endLon = lastLon;

				StartActivity(stopTrackingActivity);
			};

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

			double currentDistance = travelled;

			double newDistance = currentDistance + dist;

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

		///<summary>
		/// Updates UI with location data
		/// </summary>
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

			if (lastLon == 0.0)
				lastLon = location.Longitude;
			thisJourney.startLon = location.Longitude;

			// calculate the distance travelled
			calculateDistanceTravelled (location.Latitude, location.Longitude, lastLat, lastLon);

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
	}
}


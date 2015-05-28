
using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Locations;
using Android.Util;

namespace TrackMate
{
	[Activity (Label = "TrackMate", Icon = "@drawable/icon")]			
	public class StartTrackingActivity : Activity, ILocationListener
	{
		// variables accessible by the class
		LocationManager locMgr;
		Button start;
		Button stop;
		TextView longitude;
		TextView latitude;
		TextView locationStatus;
		TextView distanceTravelled;
		string Provider; // the GPS Provider
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


			Criteria criteriaForLocationService = new Criteria{
				//A constant indicating an approximate accuracy
				Accuracy = Accuracy.Fine,
				PowerRequirement = Power.Low
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

				// pass vars to new Activity

				API.MakeAPICall("save-journey");

				StartActivity(stopTrackingActivity);
			};

		}

		protected override void OnResume ()
		{
			base.OnResume ();
			Provider = LocationManager.GpsProvider;

			if(locMgr.IsProviderEnabled(Provider))
			{
				// make a request to update the location every 2 seconds or if the distance travelled is greater than 1m
				locMgr.RequestLocationUpdates (Provider, 2000, 1, this);
			}
			else
			{
				Log.Info("provider", Provider + " is not available. Does the device have location services enabled?");
			}
		}

		// remove location based updtes when the device is paused
		protected override void OnPause ()
		{
			base.OnPause ();
			locMgr.RemoveUpdates (this);
		}


		// implement the interface of: ILocationListener

		public void OnLocationChanged (Location location)
		{
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
		}

		public void OnProviderDisabled (string provider)
		{
			locationStatus.Text = provider + " is not available. Does the device have location services enabled?";
		}

		public void OnProviderEnabled (string provider)
		{
			locationStatus.Text = provider + " is available.";
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			locationStatus.Text = "OnStatsChanged";
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
	}
}


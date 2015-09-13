using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.Media;
using Android.Util;
using Gcm.Client;
using Geolocator.Plugin;
using Xamarin.Auth;
using System.Net.Http;
using Android.Widget;
using Android.Net;


namespace TrackMate
{
	//You must subclass this!
	[BroadcastReceiver(Permission=Constants.PERMISSION_GCM_INTENTS)]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK }, Categories = new string[] { "@PACKAGE_NAME@" })]
	[IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY }, Categories = new string[] { "@PACKAGE_NAME@" })]
	public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<PushHandlerService>
	{
		//IMPORTANT: Change this to your own Sender ID!
		//The SENDER_ID is your Google API Console App Project ID.
		//  Be sure to get the right Project ID from your Google APIs Console.  It's not the named project ID that appears in the Overview,
		//  but instead the numeric project id in the url: eg: https://code.google.com/apis/console/?pli=1#project:785671162406:overview
		//  where 785671162406 is the project id, which is the SENDER_ID to use!
		public static string[] SENDER_IDS = new string[] {"1064923699237"};

		public const string TAG = "PushSharp-GCM";
	}

	[Service] //Must use the service tag
	public class PushHandlerService : GcmServiceBase
	{

		public PushHandlerService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

		const string TAG = "GCM-SAMPLE";

		protected override void OnRegistered (Context context, string registrationId)
		{
			// check if an account exists if true: force to main activ
			IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");
			Account user = accounts.FirstOrDefault();

			Dictionary<string, string> user_properties = user.Properties;
			user_properties ["user_device_id"] = registrationId;
			var registerDevice = new RegisterDeviceRequest();
			registerDevice.auth_token = user_properties ["auth_token"];
			registerDevice.deviceId = registrationId;

			// save the Account data
			AccountStore.Create (this).Save (user, "TrackMate");

			var deviceData = registerDevice.CreateJson();

			//var wc = new WebClient ();
			//var uri = new System.Uri ("http://samstreet.niftydigital.co.uk/api/v1/register-device");
			//wc.UploadStringAsync (uri, deviceData);

			var request = new Request();
			// Ideally I would like to use my own request, but should work either way
			Task.Run(() => request.makeRequest("register-device", "POST", deviceData));

		}

		protected override void OnUnRegistered (Context context, string registrationId)
		{
			// we have no use for this method. A user device will always be registered/overwritten in the db
			return;
		}

		protected override void OnMessage (Context context, Intent intent)
		{
			// check if an account 
			// generate a request
			if (intent != null && intent.Extras != null)
			{

				// mix out the the type of push
				// there should only ever be one push
				foreach (var key in intent.Extras.KeySet()) {
					switch (key) {
					case "message":
							// push onto some activity
						break;
					case "ping":
						// ping back to the api to let the pinger know we have received the ping
						/* START OF WEBHOOK */
						String guid = intent.Extras.Get (key).ToString ();
						IEnumerable<Account> accounts = AccountStore.Create (this).FindAccountsForService ("TrackMate");
						Account user = accounts.FirstOrDefault ();
						var device_id = user.Properties ["user_device_id"];
						String url = "webhook?ping_received=" + device_id + "&guid=" + guid;
						
						var request = new Request ();
						// we want to run an ASYNC task at this point
						Task.Run (() => request.makeRequest (url, "GET", null));
						/* END OF WEBHOOK */

						var update = new LatLonUpdates ();

						this.enableGPS ();

						try{
							
							var locator = CrossGeolocator.Current;
							locator.DesiredAccuracy = 50;

							var task = Task.Run(() => locator.GetPositionAsync());
							var position = task.Result;
							update.Lat = position.Latitude.ToString();
							update.Lon = position.Longitude.ToString();
							update.auth_token = accounts.FirstOrDefault ().Properties ["auth_token"];
							update.date = DateTime.Now.ToString ();
							var json = update.CreateJson ();

							Task.Run(() => request.makeRequest ("update/location", "POST", json));


						} catch(Exception e){
							var error = e.Message;
							Log.Error ("ERROR", e.Message);
						}
						break;
					}
				}
			}
		}

		protected override bool OnRecoverableError (Context context, string errorId)
		{
			Log.Warn(TAG, "Recoverable Error: " + errorId);

			return base.OnRecoverableError (context, errorId);
		}

		protected override void OnError (Context context, string errorId)
		{
			Log.Error(TAG, "GCM Error: " + errorId);
		}

		void createNotification(string title, string desc)
		{
			//Create notification
			var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;

			//Create an intent to show ui
			var uiIntent = new Intent(this, typeof(MainActivity));

			//Create the notification
			var notification = new Notification(Android.Resource.Drawable.SymActionChat, title);

			//Auto cancel will remove the notification once the user touches it
			notification.Flags = NotificationFlags.AutoCancel;

			notification.Sound = RingtoneManager.GetDefaultUri (RingtoneType.Notification);
			//Set the notification info
			//we use the pending intent, passing our ui intent over which will get called
			//when the notification is tapped.
			notification.SetLatestEventInfo(this, title, desc, PendingIntent.GetActivity(this, 0, uiIntent, 0));

			//Show the notification
			notificationManager.Notify(1, notification);
		}

		void enableGPS(){
			Intent intent = new Intent("android.location.GPS_ENABLED_CHANGE");
			intent.PutExtra ("enabled", true);
			SendBroadcast (intent);
		}
			

		void HandleLocationChanged(object sender, LocationChangedEventArgs e)
		{
			Android.Locations.Location location = e.Location;

			Log.Debug ("lat", location.Latitude.ToString());

		}
	}
}


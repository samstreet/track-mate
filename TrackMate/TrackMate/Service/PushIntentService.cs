using System;
using Android.App;
using Android.OS;
using Android.Content;

namespace TrackMate
{
	[Service]
	public class PushIntentService : IntentService
	{
		static PowerManager.WakeLock sWakeLock;
		static object LOCK = new object();

		static void RunIntentInService(Context context, Intent intent)
		{
			lock (LOCK)
			{
				if (sWakeLock == null)
				{
					// This is called from BroadcastReceiver, there is no init.
					var pm = PowerManager.FromContext(context);
					sWakeLock = pm.NewWakeLock(
						WakeLockFlags.Partial, "My WakeLock Tag");
				}
			}

			sWakeLock.Acquire();
			intent.SetClass(context, typeof(PushIntentService));
			context.StartService(intent);
		}

		protected override void OnHandleIntent(Intent intent)
		{
			try
			{
				Context context = this.ApplicationContext;
				string action = intent.Action;

				if (action.Equals("com.google.android.c2dm.intent.REGISTRATION"))
				{
					string senders = "1064923699237";
					Intent register = new Intent("com.google.android.c2dm.intent.REGISTER");
					register.SetPackage("com.google.android.gsf");
					register.PutExtra("app", PendingIntent.GetBroadcast(context, 0, new Intent(), 0));
					register.PutExtra("sender", senders);
					context.StartService(register);
				}
				else if (action.Equals("com.google.android.c2dm.intent.RECEIVE"))
				{
					HandleMessage(intent);
				}
			}
			finally
			{
				lock (LOCK)
				{
					//Sanity check for null as this is a public method
					if (sWakeLock != null)
						sWakeLock.Release();
				}
			}
		}

		private void HandleMessage(Intent intent)
		{
			string score = intent.GetStringExtra("score");
			// do something with the score
		}
	}

}


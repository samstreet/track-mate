using System;
using Android.OS;

namespace TrackMate
{
	public class ServiceConnectedEventArgs : EventArgs
	{
		public IBinder Binder { get; set; }
	}
}


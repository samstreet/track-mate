package md55a0d52af901586059d3aed079f94fb17;


public class LocationServiceBinder
	extends android.os.Binder
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("TrackMate.LocationServiceBinder, TrackMate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", LocationServiceBinder.class, __md_methods);
	}


	public LocationServiceBinder () throws java.lang.Throwable
	{
		super ();
		if (getClass () == LocationServiceBinder.class)
			mono.android.TypeManager.Activate ("TrackMate.LocationServiceBinder, TrackMate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	public LocationServiceBinder (md55a0d52af901586059d3aed079f94fb17.LocationService p0) throws java.lang.Throwable
	{
		super ();
		if (getClass () == LocationServiceBinder.class)
			mono.android.TypeManager.Activate ("TrackMate.LocationServiceBinder, TrackMate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "TrackMate.LocationService, TrackMate, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0 });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}

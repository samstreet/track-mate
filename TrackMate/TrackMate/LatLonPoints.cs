
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

namespace TrackMate
{
	class LatLonPoints
	{
		public string Lat { get; set;}
		public string Lon { get; set;}

		public string ToJSON(){
			return "{ \"lat\" : "+ Lat +", \"lon\" " + Lon + "}";
		}
	}

}


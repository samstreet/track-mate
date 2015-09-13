
using System;

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


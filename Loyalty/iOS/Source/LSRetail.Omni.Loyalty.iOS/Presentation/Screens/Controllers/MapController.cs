using System;
using UIKit;
using System.Collections.Generic;
using MapKit;
using Presentation.Utils;
using CoreLocation;
using System.Linq;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation
{
    public partial class MapController : UIViewController
	{
		private MKMapView map;
		private MapDelegate mapDelegate;
		private CLLocationManager locationManager;
		private Store currentLocation;
		private List<Store> allLocations;
		private bool clickCollect;

		public List<Store> GetAllLocations { get { return this.allLocations; } }

		/// <summary>
		/// Constructor. Current location only.
		/// </summary>
		/// <param name="currentLocation">Current location.</param>
		public MapController (Store currentLocation, List<Store> allLocations, bool clickCollect) : base ("MapViewScreen", null)
		{
			this.locationManager = new CLLocationManager ();
			this.currentLocation = currentLocation;
			this.allLocations = allLocations;
			this.clickCollect = clickCollect;

			this.Title = currentLocation.Description;
		}

		/// <summary>
		/// Constructor. All locations only, no current location.
		/// </summary>
		/// <param name="allLocations">All locations.</param>
		public MapController (List<Store> allLocations, bool clickCollect) : base ("MapViewScreen", null)
		{
			this.locationManager = new CLLocationManager ();
			this.allLocations = allLocations;
			this.Title = LocalizationUtilities.LocalizedString("MapView_AllLocations", "All locations");
			this.clickCollect = clickCollect;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void LoadView ()
		{
			this.map = new MKMapView (UIScreen.MainScreen.Bounds);
			mapDelegate = new MapDelegate (this, this.clickCollect);
			map.Delegate = mapDelegate;
			View = this.map;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.map.MapType = MKMapType.Standard;
			this.map.ShowsUserLocation = true;
			this.map.ZoomEnabled = true;
			this.map.ScrollEnabled = true;

			// Mark all locations
			foreach(Store store in this.allLocations)
			{
				MKPointAnnotation annotation = new MKPointAnnotation ()
				{
					Title = store.Description
				};
				annotation.SetCoordinate(new CLLocationCoordinate2D (Convert.ToDouble(store.Latitude), Convert.ToDouble(store.Longitude)));

				map.AddAnnotation (annotation);
			}

			if(Utils.Util.GetOSVersion().Major >= 8)
			{
				locationManager.RequestWhenInUseAuthorization();
			}

			locationManager.StartUpdatingLocation();

			locationManager.LocationsUpdated += (sender, e) => 
			{
				// We can access user's location

				locationManager.StopUpdatingLocation();

				CLLocationCoordinate2D currentLocationCoord;
				bool zoomMapInToOneLocation;
				bool zoomOnUserLocation;

				if (this.currentLocation != null)
				{
					currentLocationCoord = new CLLocationCoordinate2D (Convert.ToDouble (this.currentLocation.Latitude), Convert.ToDouble (this.currentLocation.Longitude));
					zoomMapInToOneLocation = true;
					zoomOnUserLocation = false;
				}
				else
				{
					currentLocationCoord = locationManager.Location.Coordinate;
					zoomMapInToOneLocation = false;
					zoomOnUserLocation = true;
				}

				var visibleRegion = BuildVisibleRegion(currentLocationCoord, zoomMapInToOneLocation, zoomOnUserLocation);
				map.SetRegion (visibleRegion, true);

			};

			locationManager.Failed += async (sender, e) => 
			{
				// We can't access user's locations - he probably didn't allow it

				locationManager.StopUpdatingLocation();

				await AlertView.ShowAlert(
				    this,
					LocalizationUtilities.LocalizedString("Location_CouldNotGetLocation", "Couldn't get current location"),
					LocalizationUtilities.LocalizedString("Location_CouldNotGetLocationInstructions", "Make sure Location Services are enabled. To enable, go to privacy settings and turn Location Services on for this app."),
					LocalizationUtilities.LocalizedString("General_OK", "OK")
				);

				CLLocationCoordinate2D currentLocationCoord;
				bool zoomMapInToOneLocation;

				if (this.currentLocation != null) 
				{
					currentLocationCoord = new CLLocationCoordinate2D (Convert.ToDouble (this.currentLocation.Latitude), Convert.ToDouble (this.currentLocation.Longitude));
					zoomMapInToOneLocation = true;
				}
				else
				{
					CLLocationCoordinate2D defaultCoordinate = AppData.DefaultLocationCoordinates;
					currentLocationCoord = defaultCoordinate;

					List<double> allLatitudes = new List<double> ();
					List<double> allLongitudes = new List<double> ();

					if (this.allLocations.Count > 0)
					{
						foreach (Store store in this.allLocations)
						{
                            allLatitudes.Add (store.Latitude);
							allLongitudes.Add (store.Longitude);
						}

						currentLocationCoord = new CLLocationCoordinate2D ((double)allLatitudes.Average(), (double)allLongitudes.Average());
					}

					zoomMapInToOneLocation = false;
				}

				var visibleRegion = BuildVisibleRegion(currentLocationCoord, zoomMapInToOneLocation, false);
				map.SetRegion (visibleRegion, true);
			};

		}

		private MKCoordinateRegion BuildVisibleRegion(CLLocationCoordinate2D currentLocationCoord, bool zoomedInOnOneLocation, bool zoomOnUserLocation)
		{
			double coordinateSpan;

			if (zoomOnUserLocation) 
			{
				double nearest = 10000;
				CLLocation currentLoc = new CLLocation (currentLocationCoord.Latitude, currentLocationCoord.Longitude);

				foreach(var store in this.allLocations)
				{
					CLLocation tempLoc = new CLLocation ((double)store.Latitude, (double)store.Longitude);

					var meters = currentLoc.DistanceFrom (tempLoc);

					if (meters < nearest) 
					{
						nearest = meters + 700;
					}
				}

				coordinateSpan = CalculateSpanInMeters (currentLocationCoord.Latitude, nearest);
			} 
			else 
			{
				if (zoomedInOnOneLocation)
					coordinateSpan = 0.02;
				else
					coordinateSpan = 1.0;
			}



			var span = new MKCoordinateSpan(coordinateSpan, coordinateSpan);
			var region = new MKCoordinateRegion(currentLocationCoord, span);


			return region;
		}

		private double CalculateSpanInMeters(double latDegrees, double distMeters)
		{
			double tanDegrees = Math.Tan ((Math.PI * latDegrees) / 180);
			double beta = tanDegrees * 0.99664719;
			double lengthOfDegree = (Math.Cos (Math.Atan (beta)) * 6378137 * Math.PI) / 180;
			double measuresInDegreeLength = lengthOfDegree / distMeters;

			return 1 / measuresInDegreeLength;
		}
	}
}


using System.Globalization;
using CoreLocation;
using Foundation;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation
{
    public class LocationDirectionsController : UIViewController
	{
		public Store Store { get; set; }
		private UIWebView webView;
		private CLLocationManager locationManager;

		public LocationDirectionsController(Store store)
		{
			this.Store = store;
			this.locationManager = new CLLocationManager();
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
		}

		public override void LoadView ()
		{
			this.webView = new UIWebView(UIScreen.MainScreen.Bounds);
			this.View = this.webView;
		}

		public override void ViewDidLoad ()
		{
			this.Title = this.Store.Description;

			if(Utils.Util.GetOSVersion().Major >= 8)
			{
				locationManager.RequestWhenInUseAuthorization();
			}
			locationManager.StartUpdatingLocation();

			locationManager.LocationsUpdated += (sender, e) => 
			{
				// We got the user's location
				locationManager.StopUpdatingLocation();
				CLLocationCoordinate2D currentLocationCoord = locationManager.Location.Coordinate;
				LoadDirectionsWebView(currentLocationCoord);
			};

			locationManager.Failed += async (sender, e) => 
			{
				// We don't have the user's location
				locationManager.StopUpdatingLocation();

				await AlertView.ShowAlert(
				    this,
					LocalizationUtilities.LocalizedString("Location_CouldNotGetLocation", "Couldn't get current location"),
					LocalizationUtilities.LocalizedString("Location_CouldNotGetLocationInstructions", "Make sure Location Services are enabled. To enable, go to privacy settings and turn Location Services on for this app."),
					LocalizationUtilities.LocalizedString("General_OK", "OK")
				);
                LoadLocationPinWebView();
			};
		}

		private void LoadDirectionsWebView(CLLocationCoordinate2D fromCoordinates)
		{
			NSUrl url = new NSUrl ("https://maps.google.com/maps?saddr=" 
			                       + fromCoordinates.Latitude.ToString(CultureInfo.InvariantCulture) + "," + fromCoordinates.Longitude.ToString(CultureInfo.InvariantCulture) 
			                       + "&daddr=" + this.Store.Latitude.ToString(CultureInfo.InvariantCulture) + "," + this.Store.Longitude.ToString(CultureInfo.InvariantCulture));
			this.webView.LoadRequest(new NSUrlRequest(url));
		}

        private void LoadLocationPinWebView()
        {
            NSUrl url = new NSUrl("https://maps.google.com/maps?q="
                                     + this.Store.Latitude.ToString(CultureInfo.InvariantCulture)
                                     + ","
                                     + this.Store.Longitude.ToString(CultureInfo.InvariantCulture));
            this.webView.LoadRequest(new NSUrlRequest(url));
        }
	}
}


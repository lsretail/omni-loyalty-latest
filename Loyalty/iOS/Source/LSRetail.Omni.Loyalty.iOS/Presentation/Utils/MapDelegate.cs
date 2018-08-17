using MapKit;
using UIKit;
using System.Linq;
using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Utils
{
    public class MapDelegate : MKMapViewDelegate
	{
		MapController controller;
		static string annotationId = "Annotation";
		private bool clickCollect;

		public MapDelegate (MapController parentController, bool clickCollect)
		{
			this.controller = parentController;
			this.clickCollect = clickCollect;
		}

		public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{
			MKAnnotationView annotationView = null;

			if (annotation is MKUserLocation || annotation.Coordinate.Equals(mapView.UserLocation.Coordinate))
				return null;

			annotationView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation (annotationId);

			if (annotationView == null)
				annotationView = new MKPinAnnotationView (annotation, annotationId);

			//((MKPinAnnotationView)annotationView).PinColor = MKPinAnnotationColor.Red;
			((MKPinAnnotationView)annotationView).Image = ImageUtilities.FromFile ("/Branding/Standard/MapLocationIcon.png");

			annotationView.CanShowCallout = true;
			UIImageView calloutImage = new UIImageView (ImageUtilities.FromFile ("/Branding/Standard/MapLocationIcon.png"));
			calloutImage.Frame = new CoreGraphics.CGRect (0, 0, 34, 34);
			annotationView.LeftCalloutAccessoryView = calloutImage;
			annotationView.RightCalloutAccessoryView = UIButton.FromType (UIButtonType.DetailDisclosure);

			return annotationView;
		}

		public override void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control)
		{
			var navCtrl = this.controller.NavigationController;
			string annotationTitle = view.Annotation.GetTitle();

			// TODO Map together using something else than the description and annotation title?
			List<Store> allLocations = controller.GetAllLocations;
			Store selectedLocation = allLocations.Where (x => x.Description == annotationTitle).FirstOrDefault ();

			if(navCtrl.ViewControllers[0] is LocationDetailController)
			{
				navCtrl.PopToRootViewController (false);
				navCtrl.PushViewController (new LocationDetailController (selectedLocation, allLocations, false), true);
			}
			else if(navCtrl.ViewControllers[0] is LocationDetailController)
			{
				LocationDetailController restaurantDetailsScreen = navCtrl.ViewControllers [0] as LocationDetailController;
				restaurantDetailsScreen.store = selectedLocation;
				navCtrl.PopToRootViewController (true);
			}
			else
			{
				if(this.clickCollect)
				{
					navCtrl.PushViewController (new LocationDetailController (selectedLocation, allLocations, true), true);
				}
				else
				{
					navCtrl.PushViewController (new LocationDetailController (selectedLocation, allLocations, false), true);
				}
			}
		}
	}
}


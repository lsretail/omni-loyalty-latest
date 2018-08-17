using System;
using MapKit;
using Foundation;
using UIKit;
using Presentation.Screens;
using System.Linq;
using System.Collections.Generic;
using LSRetail.Omni.Domain.DataModel.Base.Setup;

namespace Presentation.Utils
{
	public class MapDelegate : MKMapViewDelegate
	{
		MapController controller;
		static string annotationId = "Annotation";

		public MapDelegate (MapController parentController)
		{
			this.controller = parentController;
		}

		public override MKAnnotationView GetViewForAnnotation (MKMapView mapView, IMKAnnotation annotation)
		{
			MKAnnotationView annotationView = null;

			if (annotation is MKUserLocation || annotation.Coordinate.Equals(mapView.UserLocation.Coordinate))
				return null; 

			annotationView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation (annotationId);

			if (annotationView == null)
			{
				annotationView = new MKPinAnnotationView(annotation, annotationId);
			}

			//((MKPinAnnotationView)annotationView).PinColor = MKPinAnnotationColor.Red;
			((MKPinAnnotationView)annotationView).Image = Presentation.Utils.Image.FromFile ("/Branding/Standard/default_map_location_image.png");

			annotationView.CanShowCallout = true;
			UIImageView calloutImage = new UIImageView (Presentation.Utils.Image.FromFile ("/Branding/Standard/default_map_location_image.png"));
			calloutImage.Frame = new CoreGraphics.CGRect (0, 0, 34, 34);
			annotationView.LeftCalloutAccessoryView = calloutImage;
			annotationView.RightCalloutAccessoryView = UIButton.FromType (UIButtonType.DetailDisclosure);

			return annotationView;
		}

		public override void CalloutAccessoryControlTapped (MKMapView mapView, MKAnnotationView view, UIControl control)
		{
			var navCtrl = this.controller.NavigationController;
			string annotationTitle = view.Annotation.GetTitle();

			List<Store> allLocations = controller.GetAllLocations;
			Store selectedLocation = allLocations.Where (x => x.Description == annotationTitle).FirstOrDefault ();

			if(navCtrl.ViewControllers[0] is LocationsCardCollectionController)
			{
				navCtrl.PopToRootViewController (false);
				navCtrl.PushViewController (new LocationDetailsController (selectedLocation, allLocations), true);
			}
			else
			{
				LocationDetailsController restaurantDetailsController = navCtrl.ViewControllers [0] as LocationDetailsController;
				restaurantDetailsController.store = selectedLocation;
				navCtrl.PopToRootViewController (true);
			}
		}
	}
}


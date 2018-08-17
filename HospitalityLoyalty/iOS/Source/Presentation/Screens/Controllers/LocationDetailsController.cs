using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using Presentation.Models;
using UIKit;

namespace Presentation.Screens
{
	public class LocationDetailsController : UIViewController, LocationDetailsView.ILocationDetailsListeners
	{
		private LocationDetailsView rootView;

		public Store store;
		private List<Store> stores { get; set; }
		private List<UIImage> images;

		public LocationDetailsController(Store store, List<Store> stores)
		{
			this.store = store;
			this.stores = stores;   // TODO Don't keep all the store in here. Should get this from somewhere else.
			this.rootView = new LocationDetailsView(this.store, this);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.rootView.TopLayoutGuideLength = this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = Utils.Util.AppDelegate.RootTabBarController.TabBar.Frame.Height;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			this.Title = this.store.Description;

			UIButton mapButton = new UIButton(UIButtonType.Custom);
			mapButton.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("PinDrop"), UIColor.White), UIControlState.Normal);
			mapButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			mapButton.Frame = new CGRect(0, 0, 30, 30);
			mapButton.TouchUpInside += (sender, e) =>
			{
				MapButtonPressed();
			};

			UIButton directionsButton = new UIButton(UIButtonType.Custom);
			directionsButton.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("Directions"), UIColor.White), UIControlState.Normal);
			directionsButton.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			directionsButton.Frame = new CGRect(0, 0, 30, 30);
			directionsButton.TouchUpInside += (sender, e) =>
			{
				DirectionsButtonPressed();
			};

			this.NavigationItem.RightBarButtonItems = new UIBarButtonItem[] { new UIBarButtonItem(mapButton), new UIBarButtonItem(directionsButton) }; 


			this.View = this.rootView;
			LoadImages();
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			//this.rootView.UpdateData();
		}

		public void HandleTap(UITapGestureRecognizer tap, nint displayImageIndex)
		{
			Console.WriteLine("TAP");
			if (this.images.Count != 0)
			{
				ImageSliderController imagesScreen = new ImageSliderController(this.images, displayImageIndex);
				this.NavigationController.PushViewController(imagesScreen, true);
			}
		}

		public void MapButtonPressed()
		{
			MapController map = new MapController(this.store, this.stores);
			this.NavigationController.PushViewController(map, true);
		}

		public void DirectionsButtonPressed()
		{
			RestaurantDirectionsDialogController directions = new RestaurantDirectionsDialogController(this.store);
			this.NavigationController.PushViewController(directions, true);
		}

		public void LoadImages(List<UIImageView> imageViews)
		{
			ImageModel imageModel = new ImageModel();
			this.images = new List<UIImage>();
			int imageCount = 0;

			foreach (var storeImage in this.store.Images)
			{
				imageModel.ImageGetById(storeImage.Id, new ImageSize(700, 500),
					(x, destinationId) =>
					{

						UIImage image = Utils.Image.FromBase64(x.Image);
						this.images.Add(image);
						UIImageView imageView = imageViews[imageCount];

						if (imageView != null)
						{
							imageView.Image = image;

							CATransition transition = new CATransition();
							transition.Duration = 0.5f;
							transition.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseInEaseOut);
							transition.Type = CATransition.TransitionFade;
							imageView.Layer.AddAnimation(transition, null);
						}

						imageCount++;
					},
					() => { /* Do nothing */ }
				);
			}
		}

		public void LoadImages()
		{
			ImageModel imageModel = new ImageModel();
			this.images = new List<UIImage>();

			foreach (var storeImage in this.store.Images)
			{
				imageModel.ImageGetById(storeImage.Id, new ImageSize(700, 500),
					(x, destinationId) =>
					{
						UIImage image = Utils.Image.FromBase64(x.Image);
						this.images.Add(image);
						this.rootView.LoadImage(image);
					},
					() => { /* Do nothing */ }
				);
			}
		}
	}
}


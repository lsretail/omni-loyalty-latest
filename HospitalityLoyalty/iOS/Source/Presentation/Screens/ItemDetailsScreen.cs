using System;
using CoreGraphics;
using System.Linq;
using Foundation;
using UIKit;
using CoreAnimation;
using System.Collections.Generic;
using Presentation.Utils;
using Domain.Menus;
using Presentation.Models;
using System.Timers;

namespace Presentation.Screens
{
	public class ItemDetailsScreen : UIViewController
	{
		private float headerImageHeight = 220f;
		private float changeQtyViewHeight = 40f;

		private UIButton transparentView;
		private UIScrollView scrollImgView;
		private UIView transparentImgView;
		private UIView detailsView;
		private UIView changeQtyView;

		private CGPoint currentImgPoint;
		private CGPoint currentImgOffset;
		private CGPoint beginningPoint;
		private CGPoint newPoint;

		private UIPageControl pageControlImage;
		private List<UIImage> images;
		private List<UIImageView> imageViews;
		//private Dictionary<string, UIImageView> imageViewIdToUIImageViewMap;
	
		private MenuItem menuItem;
		private decimal quantityToAddToBasket;

		public ItemDetailsScreen ()
		{
			this.currentImgPoint = new CGPoint (0, 0);
			this.currentImgOffset = new CGPoint (0, 0);
			this.quantityToAddToBasket = 1;
		}

		public ItemDetailsScreen(MenuItem menuItem)
		{
			if (menuItem != null) 
			{
				this.menuItem = menuItem;
			}
				
			this.currentImgPoint = new CGPoint (0, 0);
			this.currentImgOffset = new CGPoint (0, 0);
			this.quantityToAddToBasket = 1;
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			this.View.BackgroundColor = UIColor.White;

			// Navigationbar
			this.Title = this.menuItem != null ? this.menuItem.Description : string.Empty; 

			#region Details view

			nfloat scrollViewContentHeight = 0f;	// Determine height of details and scrollview dynamically to fit all the content they need to display
			nfloat detailsViewContentHeight = 0f;
			nfloat xMargin = 10f;
			nfloat yMargin = 10f;

			UIScrollView scrollView = new UIScrollView ();
			scrollView.Frame = this.View.Frame;
			scrollView.BackgroundColor = UIColor.Clear;
			scrollView.ShowsVerticalScrollIndicator = false;
			scrollView.ContentInset = new UIEdgeInsets(0f, 0f, yMargin, 0f);

			// Transparent view (to see imageview through)
			this.transparentImgView = new UIView ();
			this.transparentImgView.Frame = new CGRect (0f, 0f, scrollView.Frame.Width, this.headerImageHeight);
			this.transparentImgView.BackgroundColor = UIColor.Clear;
			this.transparentImgView.UserInteractionEnabled = true;

			// Gesture recognizer to swipe between images
			if(this.menuItem.Images != null & this.menuItem.Images.Count > 1)
			{
				UIPanGestureRecognizer swipe = new UIPanGestureRecognizer();
				swipe.AddTarget(() => { HandleDrag(swipe); });
				transparentImgView.AddGestureRecognizer(swipe);
			}

			// Gesture recognizer, click to view images larger
			UITapGestureRecognizer tap = new UITapGestureRecognizer();
			tap.AddTarget(() => { HandleTap(tap);} );
			transparentImgView.AddGestureRecognizer(tap);

			// The detailsview itself
			this.detailsView = new UIView ();
			detailsView.Frame = new CGRect (0f, this.transparentImgView.Frame.Bottom, scrollView.Frame.Width, 1); // Set arbitrary height
			detailsView.BackgroundColor = UIColor.Clear;

			// Title
			UILabel title = new UILabel ();
			title.Frame = new CGRect (xMargin, yMargin, detailsView.Frame.Width - 2 * xMargin, 20f);
			title.UserInteractionEnabled = false;
			title.Text = this.menuItem != null? this.menuItem.Description : string.Empty;
			title.TextColor = Utils.AppColors.PrimaryColor;
			title.Font = UIFont.BoldSystemFontOfSize (17);
			title.TextAlignment = UITextAlignment.Left;
			title.Tag = 100;
			detailsView.AddSubview (title);
			detailsViewContentHeight += yMargin;
			detailsViewContentHeight += title.Frame.Height;

			// Price
			UILabel price = new UILabel();
			price.Frame = new CGRect(xMargin, title.Frame.Bottom, detailsView.Frame.Width - 2 * xMargin, 20f);
			price.UserInteractionEnabled = false;
			price.Text = this.menuItem != null? AppData.MobileMenu.Currency.FormatDecimal(this.menuItem.Price) : string.Empty;
			price.TextColor = Utils.AppColors.PrimaryColor;
			price.Font = UIFont.SystemFontOfSize (14);
			price.TextAlignment = UITextAlignment.Left;
			price.Tag = 200;
			detailsView.AddSubview (price);
			detailsViewContentHeight += price.Frame.Height;

			// Deal items
			UILabel dealItems = new UILabel();
			dealItems.Frame = new CGRect(xMargin, price.Frame.Bottom, detailsView.Frame.Width - 2 * xMargin, 200f); //arbitrary height
			dealItems.UserInteractionEnabled = false;
			dealItems.Text = GetDealLineItemsText(this.menuItem);
			dealItems.Font = UIFont.SystemFontOfSize(14f);
			dealItems.Lines = 0;
			dealItems.LineBreakMode = UILineBreakMode.WordWrap;
			dealItems.SizeToFit();
			detailsView.AddSubview(dealItems);
			detailsViewContentHeight += dealItems.Frame.Height;

			nfloat addToBasketButtonWidth = 0f;
			nfloat modifyAndAddButtonWidth = 0f;

			if(this.menuItem is Product || !this.menuItem.AnyModifiers)
			{
				// Products don't have any modifiers, don't give the "Modify" option
				addToBasketButtonWidth = detailsView.Frame.Width - 2 * xMargin;
			}
			else
			{
				//Both visible
				addToBasketButtonWidth = (detailsView.Frame.Width - 2 * xMargin)/2 - xMargin/2;
				modifyAndAddButtonWidth = addToBasketButtonWidth;
			}

			if (this.menuItem is Product || !this.menuItem.AnyModifiers)
			{
				// Products don't have any modifiers, don't give the "Modify" option
				addToBasketButtonWidth = detailsView.Frame.Width - 2 * xMargin;
			}
			else
			{
				addToBasketButtonWidth = (detailsView.Frame.Width - 2 * xMargin)/2 - xMargin/2;
				modifyAndAddButtonWidth = addToBasketButtonWidth;
			}

			UIButton btnModifyAndAdd = new UIButton();
			btnModifyAndAdd.Frame = new CGRect(xMargin, dealItems.Frame.Bottom + yMargin, modifyAndAddButtonWidth, 40f);
			btnModifyAndAdd.SetTitle(NSBundle.MainBundle.LocalizedString("EditBasketItem_EditItem", "Modify item"), UIControlState.Normal);
			btnModifyAndAdd.SetTitleColor(UIColor.White, UIControlState.Normal);
			if(Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				btnModifyAndAdd.Font = UIFont.SystemFontOfSize(16f);

			btnModifyAndAdd.BackgroundColor = Utils.AppColors.PrimaryColor;
			btnModifyAndAdd.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			btnModifyAndAdd.ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f);
			btnModifyAndAdd.Layer.CornerRadius = 2;
			btnModifyAndAdd.Tag = 400;
			btnModifyAndAdd.TouchUpInside += (object sender, EventArgs e) => {

				// Go to modifiers screen
				AddToBasketController addToBasketController = new AddToBasketController(this.menuItem.Clone(), this.quantityToAddToBasket, false);
				this.PresentViewController(new UINavigationController(addToBasketController), true, null);
			};

			UIImageView btnModifyImageView = new UIImageView();
			btnModifyImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			btnModifyImageView.Image = Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconEdit.png"), UIColor.White);
			btnModifyImageView.BackgroundColor = UIColor.Clear;
			btnModifyAndAdd.AddSubview(btnModifyImageView);

			detailsView.AddSubview(btnModifyAndAdd);

			//Add to basket button
			UIButton btnAddToBasket = new UIButton();
			btnAddToBasket.Frame = new CGRect(
				modifyAndAddButtonWidth != 0f ? btnModifyAndAdd.Frame.Right + xMargin : xMargin, 
				dealItems.Frame.Bottom + yMargin, 
				addToBasketButtonWidth, 
				40f
			);
			btnAddToBasket.SetTitle(NSBundle.MainBundle.LocalizedString("ItemDetails_AddToBasket", "Add to basket"), UIControlState.Normal);  
			btnAddToBasket.SetTitleColor(UIColor.White, UIControlState.Normal);
			if(Utils.Util.AppDelegate.DeviceScreenWidth < 321f)
				btnAddToBasket.Font = UIFont.SystemFontOfSize(16f);

			btnAddToBasket.BackgroundColor = Utils.AppColors.PrimaryColor;
			if(modifyAndAddButtonWidth != 0f)
			{
				btnAddToBasket.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
				btnAddToBasket.ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f);
			}
			btnAddToBasket.Layer.CornerRadius = 2;
			btnAddToBasket.Tag = 300;
			btnAddToBasket.TouchUpInside += (object sender, EventArgs e) => {

				AddToBasketPressed();
			};


			UIImageView btnAddToBasketImageView = new UIImageView();
			btnAddToBasketImageView.Frame = new CGRect(10f, 6f, 28f, 28f);
			btnAddToBasketImageView.Image = Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconShoppingBasketAdd.png"), UIColor.White);
			btnAddToBasketImageView.BackgroundColor = UIColor.Clear;
			btnAddToBasket.AddSubview(btnAddToBasketImageView);


			detailsView.AddSubview(btnAddToBasket);
			detailsViewContentHeight += btnAddToBasket.Frame.Height + yMargin + yMargin/2;


			// Detail text
			UITextView textDetails = new UITextView();
			nfloat textDetailsYPos = btnModifyAndAdd.Frame.Bottom + yMargin/2;
			textDetails.Frame = new CGRect(xMargin, textDetailsYPos, detailsView.Frame.Width - 2 * xMargin, 1f); // Set arbitrary height
			textDetails.Editable = false;
			textDetails.ScrollEnabled = false;
			textDetails.Text = this.menuItem == null ? string.Empty : this.menuItem.Details;

			textDetails.Font = UIFont.SystemFontOfSize(16f);
			textDetails.BackgroundColor = UIColor.Clear;
			CGSize newSizeThatFits = textDetails.SizeThatFits(textDetails.Frame.Size);
			CGRect tempFrame = textDetails.Frame;
			tempFrame.Size = new CGSize(tempFrame.Size.Width, newSizeThatFits.Height);	// Only adjust the height
			textDetails.Frame = tempFrame;
			detailsView.AddSubview(textDetails);	
			detailsViewContentHeight += textDetails.Frame.Height;

			// We want the scrollview to be scrollable even though the entire content fits on the screen.
			// Let's do this by setting a minimum height for the detailsViewContentHeight

			nfloat minDetailsViewContentHeight = Utils.Util.AppDelegate.DeviceScreenHeight - detailsView.Frame.Y;
			if (detailsViewContentHeight < minDetailsViewContentHeight)
				detailsViewContentHeight = minDetailsViewContentHeight;

			// Resize views to fit content (i.e. adjust height)
			scrollViewContentHeight += transparentImgView.Frame.Height;
			scrollViewContentHeight += detailsViewContentHeight;
			scrollViewContentHeight += Utils.Util.AppDelegate.StatusbarPlusNavbarHeight;
			scrollViewContentHeight += changeQtyViewHeight;
			scrollView.ContentSize = new CGSize (scrollView.Frame.Width, scrollViewContentHeight);
			detailsView.Frame = new CGRect(detailsView.Frame.Location, new CGSize(detailsView.Frame.Size.Width, detailsViewContentHeight));
				
			SetupPagingImageViews();
			LoadImages ();

			scrollView.AddSubview (transparentImgView);
			scrollView.AddSubview (detailsView);
			this.View.AddSubview (scrollView);

			// Add transparentwhite-to-white gradient to detailsview
			CoreAnimation.CAGradientLayer gradientLayer = new CoreAnimation.CAGradientLayer ();
			gradientLayer.Frame = detailsView.Bounds;
			CoreGraphics.CGColor[] colors = new CoreGraphics.CGColor[2];
			colors [0] = Utils.AppColors.TransparentWhite.CGColor;
			colors [1] = UIColor.White.CGColor;
			gradientLayer.Colors = colors;
			gradientLayer.EndPoint = new CGPoint (0.5f, 0.2f);
			detailsView.Layer.InsertSublayer (gradientLayer, 0);

			#endregion

			// Change quantity bar

			this.changeQtyView = new UIView();
			this.View.AddSubview(this.changeQtyView);
			this.changeQtyView.BackgroundColor = Utils.AppColors.TransparentWhite3;

			UILabel lblChangeQty = new UILabel();
			lblChangeQty.Text = NSBundle.MainBundle.LocalizedString("ItemDetails_Quantity", "Quantity") + ":";
			lblChangeQty.TextColor = AppColors.PrimaryColor;
			lblChangeQty.Tag = 100;
			this.changeQtyView.AddSubview(lblChangeQty);

			UIButton btnPlusQty = new UIButton();
			btnPlusQty.SetTitle("+", UIControlState.Normal);
			btnPlusQty.SetTitleColor(AppColors.PrimaryColor, UIControlState.Normal);
			btnPlusQty.BackgroundColor = UIColor.Clear;
			btnPlusQty.Tag = 200;
			btnPlusQty.TouchUpInside += (object sender, EventArgs e) => { IncreaseQuantityToAddToBasket(); };
			this.changeQtyView.AddSubview(btnPlusQty);

			UILabel lblItemQuantity = new UILabel();
			lblItemQuantity.Text = this.quantityToAddToBasket.ToString();
			lblItemQuantity.TextColor = AppColors.PrimaryColor;
			lblItemQuantity.Font = UIFont.SystemFontOfSize(14);
			lblItemQuantity.TextAlignment = UITextAlignment.Center;
			lblItemQuantity.Tag = 400;
			this.changeQtyView.AddSubview(lblItemQuantity);

			UIButton btnMinusQty = new UIButton();
			btnMinusQty.SetTitle("-", UIControlState.Normal);
			btnMinusQty.SetTitleColor(AppColors.PrimaryColor, UIControlState.Normal);
			btnMinusQty.BackgroundColor = UIColor.Clear;
			btnMinusQty.Tag = 300;
			btnMinusQty.TouchUpInside += (object sender, EventArgs e) => { DecreaseQuantityToAddToBasket(); };
			this.changeQtyView.AddSubview(btnMinusQty);

			SetRightBarButtonItems();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			// Navigation bar
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			// We want to show the toolbar in this screen, if basket is enabled (since the toolbar only contains the add to basket button)
			// Also want to show the change quantity view
			if (Util.AppDelegate.BasketEnabled)
			{
				this.NavigationController.ToolbarHidden = true;
				this.changeQtyView.Hidden = false;
			}
			else
			{
				this.NavigationController.ToolbarHidden = true;
				this.changeQtyView.Hidden = true;
			}
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);

			// Hide the toolbar again, we only want to show it for this screen, otherwise it stays there on other screens in the navigation hierarchy
			this.NavigationController.ToolbarHidden = true;
		}

		public override void ViewDidLayoutSubviews ()
		{
			// TODO Move all frame-handling-code in here

			base.ViewDidLayoutSubviews ();

			// Change quantity view

			// - this.BottomLayoutGuide.Length
			this.changeQtyView.Frame = new CGRect(0, this.View.Bounds.Bottom - changeQtyViewHeight, this.View.Frame.Width, changeQtyViewHeight);

			float changeQtyViewMargin = 10f;
			float changeQtyViewButtonWidth = 50f;

			UIView lblChangeQuantity = this.changeQtyView.ViewWithTag(100);
			lblChangeQuantity.Frame = new CGRect(changeQtyViewMargin, 0f, 80f, this.changeQtyView.Frame.Height);

			UIView btnPlusQty = this.changeQtyView.ViewWithTag(200);
			btnPlusQty.Frame = new CGRect(this.changeQtyView.Frame.Width - changeQtyViewButtonWidth, 0, changeQtyViewButtonWidth, this.changeQtyView.Frame.Height);

			UIView lblItemQuantity = this.changeQtyView.ViewWithTag(400);
			lblItemQuantity.Frame = new CGRect(btnPlusQty.Frame.Left - 40f, 0, 40f, this.changeQtyView.Frame.Height);

			UIView btnMinusQty = this.changeQtyView.ViewWithTag(300);
			btnMinusQty.Frame = new CGRect(lblItemQuantity.Frame.Left - changeQtyViewButtonWidth, 0, changeQtyViewButtonWidth, this.changeQtyView.Frame.Height);
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			UIButton btnFavorite = new UIButton (UIButtonType.Custom);
			btnFavorite.SetImage (GetFavoriteButtonIcon(), UIControlState.Normal);
			btnFavorite.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			btnFavorite.Frame = new CGRect (0, 0, 30, 30);
			btnFavorite.TouchUpInside += (sender, e) => 
			{
				new FavoriteModel().ToggleFavorite(this.menuItem.Clone());
				btnFavorite.SetImage (GetFavoriteButtonIcon(), UIControlState.Normal);
			};
			barButtonItemList.Add(new UIBarButtonItem(btnFavorite));

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		private UIImage GetFavoriteButtonIcon()
		{
			if ((new FavoriteModel()).IsFavorite(this.menuItem))
				return Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconFavoriteON.png"), UIColor.White);
			else
				return Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("/Icons/IconFavoriteOFF.png"), UIColor.White);
		}

		private string GetDealLineItemsText(MenuItem menuItem)
		{
			string dealLineItems = string.Empty;

			if(menuItem is Deal)
			{
				foreach(var dealLine in (menuItem as Deal).DealLines)
				{
					var dealLineItem = dealLine.DealLineItems.FirstOrDefault(x => x.DealLineMenuItem.Id == dealLine.SelectedId);

					if (dealLineItem != null)
					{
						dealLineItems += dealLineItem.Qty > 1 ? Utils.Util.FormatQty(dealLineItem.Qty) + " " + dealLineItem.DealLineMenuItem.Description : dealLineItem.DealLineMenuItem.Description;
						dealLineItems += System.Environment.NewLine;
					}
				}
			}

			return dealLineItems.TrimEnd();
		}

		private void AddToBasketPressed()
		{
			// Have to check if there are any required modifiers.
			// If there are, show the modifiersscreen but only display the required modifiers.
			// If there aren't, add the item straight to basket and bypass the modifiersscreen.

			bool hasRequiredModifiers = false;

			if (this.menuItem is Product)
				hasRequiredModifiers = (this.menuItem as Product).AnyRequired;
			else if (this.menuItem is Recipe)
				hasRequiredModifiers = (this.menuItem as Recipe).AnyRequired;
			else if (this.menuItem is Deal)
				hasRequiredModifiers = (this.menuItem as Deal).AnyRequired;

			if (hasRequiredModifiers)
			{
				// Go to modifiers screen, but show only required modifiers
				AddToBasketController addToBasketController = new AddToBasketController (this.menuItem.Clone(), this.quantityToAddToBasket, true);
				this.PresentViewController(new UINavigationController(addToBasketController), true, null);
			}
			else
			{
				// Add item straight to basket (no modifiers screen)
				new BasketModel().AddItemToBasket(this.menuItem.Clone(), this.quantityToAddToBasket);
				//Utils.Util.AppDelegate.SlideoutBasket.Refresh();

				Utils.UI.bannerViewTimer.Start();
				Utils.UI.ShowAddedToBasketBannerView (NSBundle.MainBundle.LocalizedString("SlideoutBasket_ItemAddedToBasket", "Vöru var bætt í körfuna!"), Utils.Image.FromFile("/Branding/Standard/default_map_location_image.png"));
			}
		}

		private void SetupPagingImageViews()
		{
			int pageSize = (int)Util.AppDelegate.DeviceScreenWidth;
			float offset = 0;
			int pageCount = this.menuItem.Images.Count;
			//this.imageViewIdToUIImageViewMap = new Dictionary<string, UIImageView>();

			this.imageViews = new List<UIImageView> ();
			this.scrollImgView = new UIScrollView (new CGRect (this.View.Frame.X, this.View.Frame.Y, this.View.Frame.Width, this.headerImageHeight));

			for (int i = 0; i < pageCount; i++)
			{
				if (i > 0)
				{
					offset += pageSize;
				}

				UIImageView imgView = new UIImageView ();
				imgView.Frame = new CGRect (this.View.Frame.X, this.View.Frame.Y, this.View.Frame.Width, this.headerImageHeight);
				imgView.ClipsToBounds = true;
				imgView.ContentMode = UIViewContentMode.ScaleAspectFill;
				imgView.BackgroundColor = Utils.UI.GetUIColorFromHexString (this.menuItem.Images[i].AvgColor);
				this.imageViews.Add (imgView);

				var clickableImageView = new UIButton(new CGRect(offset, 0, this.View.Frame.Width, this.headerImageHeight));
				clickableImageView.AddSubview (imgView);
				this.scrollImgView.Add(clickableImageView);

				//create the transparent image as a button
				this.transparentView = new UIButton(new CGRect(offset, 0, this.View.Frame.Width, this.headerImageHeight));
				this.transparentView.Alpha = 0.4f;
				this.scrollImgView.Add(this.transparentView);

				this.scrollImgView.ContentSize = new CGSize(pageCount * pageSize + 130, this.headerImageHeight);
				this.scrollImgView.ContentInset = new UIEdgeInsets(0f, 0f, 0f, 0f);

				/* TODO Rethink this. If the image IDs are not unique we get non-unique keys, crash. Fix it like this for now.
				if (!this.imageViewIdToUIImageViewMap.ContainsKey(this.menuItem.Images[i].Id))
					this.imageViewIdToUIImageViewMap.Add(this.menuItem.Images[i].Id, imgView);
					*/
			}

			this.pageControlImage = new UIPageControl(new CGRect(0, 60, this.View.Frame.Width, this.headerImageHeight));
			pageControlImage.HidesForSinglePage = true;
			pageControlImage.Pages = this.menuItem.Images.Count;

			this.View.AddSubview (this.scrollImgView);
			this.View.AddSubview (this.pageControlImage);
		}

		private void LoadImages()
		{
			this.images = new List<UIImage> ();

			ImageModel imageModel = new ImageModel();
			int imageCount = 0;

			foreach (var itemImageView in this.menuItem.Images)
			{
				imageModel.ImageGetById(itemImageView.Id, new Domain.Images.ImageSize(700, 500), 
					(x, destinationId) => {

						UIImage image = Utils.Image.FromBase64(x.Image);

						this.images.Add(image);

						// Add image to its UIImageView
						//UIImageView imageView = null;
						//this.imageViewIdToUIImageViewMap.TryGetValue(itemImageView.Id, out imageView);

						UIImageView imageView = this.imageViews[imageCount];

						if (imageView != null)
						{
							imageView.Image = image;

							CATransition transition = new CATransition ();
							transition.Duration = 0.5f;
							transition.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
							transition.Type = CATransition.TransitionFade;
							imageView.Layer.AddAnimation (transition, null);
						}

						imageCount++;
					},
					() => { /* Do nothing */ }
				);
			}
		}

		private void HandleDrag(UIPanGestureRecognizer recognizer)
		{
			if(recognizer.State == UIGestureRecognizerState.Began)
			{
				this.beginningPoint = recognizer.TranslationInView (this.transparentImgView);
				this.currentImgOffset.X = this.scrollImgView.ContentOffset.X;
			}

			if(recognizer.State != (UIGestureRecognizerState.Ended | UIGestureRecognizerState.Cancelled |UIGestureRecognizerState.Failed))
			{
				this.newPoint = recognizer.TranslationInView (this.transparentImgView);
				this.currentImgPoint.X = this.beginningPoint.X - this.newPoint.X + this.currentImgOffset.X;
				this.scrollImgView.SetContentOffset (this.currentImgPoint, false);
			}

			if(recognizer.State == UIGestureRecognizerState.Ended)
			{
				nfloat length = this.beginningPoint.X - this.newPoint.X;

				if(length >= 60f)
				{
					if (this.pageControlImage.Pages != (this.pageControlImage.CurrentPage + 1)) 
					{
						this.currentImgPoint.X = (this.pageControlImage.CurrentPage + 1) * scrollImgView.Frame.Width;
						this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
						this.pageControlImage.CurrentPage = this.pageControlImage.CurrentPage + 1;
					}
					else
					{
						this.currentImgPoint.X = this.pageControlImage.CurrentPage * scrollImgView.Frame.Width;
						this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
					}
				}
				else if(length <= -60f)
				{
					if (this.pageControlImage.CurrentPage != 0) 
					{
						this.currentImgPoint.X = (this.pageControlImage.CurrentPage - 1) * scrollImgView.Frame.Width;
						this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
						this.pageControlImage.CurrentPage = this.pageControlImage.CurrentPage - 1;
					}
					else
					{
						this.currentImgPoint.X = this.pageControlImage.CurrentPage * scrollImgView.Frame.Width;
						this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
					}
				}
				else
				{
					this.currentImgPoint.X = this.pageControlImage.CurrentPage * scrollImgView.Frame.Width;
					this.scrollImgView.SetContentOffset (this.currentImgPoint, true);
				}
			}
		}

		private void HandleTap(UITapGestureRecognizer tap)
		{
			System.Diagnostics.Debug.WriteLine ("Tap");

			ImageSliderController imagesScreen = new ImageSliderController (this.images, this.pageControlImage.CurrentPage);
			this.NavigationController.PushViewController (imagesScreen, true);
		}

		private void RefreshQuantityToAddToBasketLabel()
		{
			UILabel lblQtyToAddToBasket = (UILabel)this.changeQtyView.ViewWithTag(400);
			lblQtyToAddToBasket.Text = this.quantityToAddToBasket.ToString();
		}

		private void IncreaseQuantityToAddToBasket()
		{
			this.quantityToAddToBasket++;
			RefreshQuantityToAddToBasketLabel();
		}

		private void DecreaseQuantityToAddToBasket()
		{
			if (this.quantityToAddToBasket > 1)
				this.quantityToAddToBasket--;
			RefreshQuantityToAddToBasketLabel();
		}
	}
}
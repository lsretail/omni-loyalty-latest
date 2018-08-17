using System;
using System.Linq;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.GUIExtensions.iOS;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class ItemDetailView : UIView
	{
		//the data to display
		private MenuItem item;

		#region Constants
		//ImageSection height
		private float imageHeight = 220f;
		//Details content background gradient Color.
		private CGColor gradient = UIColor.FromRGBA(255, 255, 255, 0.6f).CGColor;
		//The gradient part of the background is 1/5 of the frame.
		private int ratioOfGradient = 5;

		private nfloat buttonWidth;
		private nfloat margin = 10f;
		#endregion

		#region ImageRelated
		//Custom class, check it for comment.
		private ImageSlider imgContainer;
		//Dots representing current image.
		private UIPageControl pageControlImage;
		#endregion

		#region Content
		//Scrolling part of ItemDetails
		private UIScrollView contentContainer;
		//Custom class, check it for comment
		private GradientView gradientBackground;
		//Data to display

		private UILabel title;
		private UILabel price;
		private UITextView textDetails;

		private IconButton addToBasket;
		private IconButton editItem;
		private InlineQtyModifier qtyModifier;

		private UIView details;
		#endregion
		#region Interface

		//Possible actions in view

		private readonly IItemDetailListeners listeners;

		public interface IItemDetailListeners
		{
			void ImageTap(int display);
			void AddToBasket();
			void ModifyItem();
			decimal ModQty(bool num);
		}

		#endregion

		public ItemDetailView(MenuItem menuItem, IItemDetailListeners listeners)
		{
			this.listeners = listeners;
			this.item = menuItem;
			this.BackgroundColor = UIColor.White;

			//define tap gesture to go to ImageSliderView
			UITapGestureRecognizer tap = new UITapGestureRecognizer();
			tap.AddTarget(() => { HandleTap(tap); });

			//ImgContainer that swipe images Horizontally.
			this.imgContainer = new ImageSlider(menuItem.Images.Count);

			//hook tap gesture to imgContainer
			imgContainer.AddGestureRecognizer(tap);
			//Define event when imge is slided.
			imgContainer.Scrolled += OnSwipe;

			title = new UILabel
			{
				UserInteractionEnabled = false,
				Text = item != null ? item.Description : string.Empty,
				TextColor = Utils.AppColors.PrimaryColor,
				Font = UIFont.BoldSystemFontOfSize(17),
				TextAlignment = UITextAlignment.Left,
				Tag = 100
			};

			price = new UILabel
			{
				UserInteractionEnabled = false,
				Text = item != null ? AppData.MobileMenu.Currency.FormatDecimal(item.Price.Value) : string.Empty,
				TextColor = Utils.AppColors.PrimaryColor,
				Font = UIFont.SystemFontOfSize(14),
				TextAlignment = UITextAlignment.Left,
				Tag = 200
			};

			textDetails = new UITextView
			{
				Editable = false,
				ScrollEnabled = false,
				Text = item == null ? string.Empty : item.Detail,
				Font = UIFont.SystemFontOfSize(16f),
				BackgroundColor = UIColor.Clear
			};

			//If modifiers, don't show qtyModifier since it's 
			//displayed on the screen after addToBasket click
			qtyModifier = new InlineQtyModifier(1);

			editItem = new IconButton(LocalizationUtilities.LocalizedString("EditBasketItem_EditItem", "Modify item"), Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile("/Icons/IconEdit.png"), UIColor.White))
			{
				Tag = 400,
			};


			addToBasket = new IconButton(LocalizationUtilities.LocalizedString("ItemDetails_AddToBasket", "Add to basket"), Utils.UI.GetColoredImage(UIImage.FromBundle("AddShoppingCart"), UIColor.White))
			{
				Tag = 300,
				HorizontalAlignment = UIControlContentHorizontalAlignment.Left,
				ContentEdgeInsets = new UIEdgeInsets(0f, 48f, 0f, 0f)
			};

			//Define the methods available in ItemDetailsContent. Some might not necessary exists
			addToBasket.TouchUpInside += (object sender, EventArgs e) =>
			{
				listeners.AddToBasket();
			};
			if (editItem != null)
			{
				editItem.TouchUpInside += (object sender, EventArgs e) =>
				{
					listeners.ModifyItem();
				};
			}

			if (qtyModifier != null)
			{
				qtyModifier.plus.TouchUpInside += (sender, e) =>
				{
					decimal newNum = listeners.ModQty(true);
					qtyModifier.qty.Text = newNum.ToString();
				};
				qtyModifier.minus.TouchUpInside += (sender, e) =>
				{
					decimal newNum = listeners.ModQty(false);
					qtyModifier.qty.Text = newNum.ToString();
				};
			}
			details = new UIView();
			details.AddSubviews(title, price, textDetails, addToBasket, editItem, qtyModifier);

			//The details background
			this.gradientBackground = new GradientView(UIColor.White.CGColor, gradient, ratioOfGradient);
			gradientBackground.AddSubview(details);
			//The Content Scroll
			this.contentContainer = new UIScrollView();
			contentContainer.AddSubview(gradientBackground);

			//Dots
			this.pageControlImage = new UIPageControl
			{
				HidesForSinglePage = true,
				Pages = this.item.Images.Count,
				UserInteractionEnabled = false,
			};


			AddSubviews(imgContainer, pageControlImage, contentContainer);
		}

		public void ShowOrHideQtyModifier(bool hide)
		{
			//qtyModifier = hasRequiredModifiers() ? null : new InlineQtyModifier(1);
			qtyModifier.Hidden = hide;
		}

		public void ShowOrHideEdit(bool hide)
		{
			//EditItem = (item is Product || !item.AnyModifiers) ?
			editItem.Hidden = hide;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			//Initial padding only if rest of content is not visible if scroll reaches up to 3/5 of image.
			nfloat contentHeight = 120f; 
			//Height of footer/tabBar
			nfloat tabBarHeight = Utils.UI.TabBarHeight;

			imgContainer.Frame = new CGRect(Frame.X, Frame.Y, Frame.Width, imageHeight);

			contentContainer.Frame = new CGRect(Frame.X, Frame.Y, Frame.Width, Frame.Height);
			contentContainer.ContentSize = new CGSize(Frame.Width, Frame.Height + (imageHeight / 5 * 3));

			gradientBackground.Frame = new CGRect(Frame.X, imgContainer.Frame.Bottom, Frame.Width, Frame.Height - (imageHeight / 5 * 3));

			details.Frame = new CGRect(0, 0, gradientBackground.Frame.Width, gradientBackground.Frame.Height);

			pageControlImage.Frame = new CGRect(0, 60, Frame.Width, imageHeight);

			//We layout the details subview to get it's arbitary height since we can't predict it
			details.LayoutSubviews();

			nfloat buttonHeight = 40f;
			nfloat contentHeaderHeight = 20f;
			nfloat contentHeaderWidth = Frame.Width - 2 * margin;
			nfloat qtyModifierHeight = 50f;
			buttonWidth = qtyModifier.Hidden ? Frame.Width - 2 * margin : (Frame.Width - 2 * margin) / 2 - margin / 2;

			title.Frame = new CGRect(margin, margin, contentHeaderWidth, contentHeaderHeight);
			price.Frame = new CGRect(margin, title.Frame.Bottom, contentHeaderWidth, contentHeaderHeight);


			qtyModifier.Frame = new CGRect(0, price.Frame.Bottom, details.Frame.Width, qtyModifierHeight);

			editItem.Frame = new CGRect(margin, qtyModifier.Frame.Bottom + margin, buttonWidth, buttonHeight);

			addToBasket.Frame = new CGRect(editItem.Frame.Right + margin, qtyModifier.Frame.Bottom + margin, buttonWidth, buttonHeight);

			textDetails.Frame = new CGRect(margin, editItem.Frame.Bottom + margin / 2, details.Frame.Width - 2 * margin, textDetails.SizeThatFits(textDetails.Frame.Size).Height);
			textDetails.ResizeTextViewHeightToFitContent();

			//Get all it's childrens height
			foreach (UIView child in details.Subviews)
				contentHeight += child.Frame.Height;

			//If it is heigher than the allowed scroll we resize the content scroll and gradientBackground size
			if (contentHeight > gradientBackground.Frame.Height)
			{
				gradientBackground.Frame = new CGRect(Frame.X, imgContainer.Frame.Bottom, Frame.Width, contentHeight);

				contentContainer.ContentSize = new CGSize(Frame.Width, contentHeight + (this.imageHeight / 5 * 3) + tabBarHeight);
			}
		}

		//Set current dot in pageController on swipe.
		private void OnSwipe(object sender, EventArgs e)
		{
			int page = Convert.ToInt32(Math.Floor(imgContainer.ContentOffset.X / imgContainer.Frame.Width));
			if (page > -1 && page < this.item.Images.Count)
				this.pageControlImage.CurrentPage = page;
		}

		//Give imgContainer each image when it is received.
		public void OnImageResponse(UIImage image)
		{
			if (image != null)
			{
				imgContainer.SetupSlide(image);
			}
		}

		//Gives Listener info on current displayed image and the 
		//listener decides what to do with that.
		private void HandleTap(UITapGestureRecognizer tap)
		{
			System.Diagnostics.Debug.WriteLine("Tap");
			string _index = pageControlImage.CurrentPage.ToString();
			int index = Convert.ToInt32(_index);
			listeners.ImageTap(index);
		}

		//The imgContainer needs to be set up behind the the scroll so
		//any touch event wouldn't be received by default so i get
		//the touch position and decide which section takes control of 
		//touch event. There is some mild calculation considering the scroll
		//position of the content section so the imgContainer wouldn't 
		//change image when covered by scroll.
		public override UIView HitTest(CGPoint point, UIEvent uievent)
		{
			var hitView = base.HitTest(point, uievent);
			CGPoint pointInImg = imgContainer.ConvertPointFromView(point, this);
			CGPoint scrollPos = contentContainer.ContentOffset;
			if (scrollPos.Y > 0)
				pointInImg.Y = pointInImg.Y + scrollPos.Y;
			if (imgContainer.PointInside(pointInImg, uievent))
			{
				return imgContainer;
			}

			return hitView;
		}
	}
}


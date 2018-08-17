using System.Collections.Generic;
using CoreGraphics;
using Presentation.Models;
using UIKit;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Utils;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Screens
{
	public class ItemDetailController : UIViewController, ItemDetailView.IItemDetailListeners
	{
		private ItemDetailView rootView;
		private MenuItem item;
		private List<UIImage> images;
		private decimal quantityToAddToBasket;
		private MenuService menuService;

		public ItemDetailController(MenuItem menuItem)
		{
			menuService = new MenuService();
			this.AutomaticallyAdjustsScrollViewInsets = false;
			this.item = menuItem;
			this.Title = menuItem.Description;
			this.rootView = new ItemDetailView(menuItem, this);
			this.images = new List<UIImage>();
			this.quantityToAddToBasket = 1;
		}

		public override void ViewWillLayoutSubviews()
		{
			base.ViewWillLayoutSubviews();
		}
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = UIColor.White;
			this.View = rootView;

			SetRightBarButtonItems();
			LoadImages();
		}
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			SetRightBarButtonItems();
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			UIButton btnFavorite = new UIButton(UIButtonType.Custom);
			btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
			btnFavorite.ImageEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
			btnFavorite.Frame = new CGRect(0, 0, 30, 30);
			btnFavorite.TouchUpInside += (sender, e) =>
			{
				new FavoriteModel().ToggleFavorite(this.item);
				btnFavorite.SetImage(GetFavoriteButtonIcon(), UIControlState.Normal);
			};
			barButtonItemList.Add(new UIBarButtonItem(btnFavorite));

			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray();
		}

		private UIImage GetFavoriteButtonIcon()
		{
			if ((new FavoriteModel()).IsFavorite(this.item))
				return Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOnIcon"), UIColor.White);
			else
				return Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOffIcon"), UIColor.White);
		}

		public void LoadImages()
		{
			ImageModel imageModel = new ImageModel();

			foreach (var itemImageView in this.item.Images)
			{
				imageModel.ImageGetById(itemImageView.Id, new ImageSize(700, 500),
					(x, destinationId) =>
					{
						UIImage image = Utils.Image.FromBase64(x.Image);
						this.images.Add(image);

						rootView.OnImageResponse(image);
					},
					() => { /* Do nothing */ }
				);
			}
		}

		public void ImageTap(int index)
		{
			ImageSliderController imagesScreen = new ImageSliderController(this.images, index);
			this.NavigationController.PushViewController(imagesScreen, true);
		}

		public void AddToBasket()
		{
			// Have to check if there are any required modifiers.
			// If there are, show the modifiersscreen but only display the required modifiers.
			// If there aren't, add the item straight to basket and bypass the modifiersscreen.

			bool hasRequiredModifiers = menuService.HasAnyRequiredModifers(AppData.MobileMenu, item);

			if (hasRequiredModifiers)
			{
				// Go to modifiers screen, but show only required modifiers
				AddToBasketController addToBasketController = new AddToBasketController(this.item, this.quantityToAddToBasket, true);
				this.PresentViewController(new UINavigationController(addToBasketController), true, null);
			}
			else
			{
				// Add item straight to basket (no modifiers screen)
				new BasketModel().AddItemToBasket(this.item, this.quantityToAddToBasket);
				Utils.UI.bannerViewTimer.Start();
				Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("SlideoutBasket_ItemAddedToBasket", "Vöru var bætt í körfuna!"), Utils.Image.FromFile("/Branding/Standard/default_map_location_image.png"));
			}
		}

		public void ModifyItem()
		{
			AddToBasketController addToBasketController = new AddToBasketController(this.item, this.quantityToAddToBasket, false);
			this.PresentViewController(new UINavigationController(addToBasketController), true, null);
		}

		public decimal ModQty(bool inc)
		{
			if (quantityToAddToBasket == 1 && !inc)
				return quantityToAddToBasket;
			
			quantityToAddToBasket = inc ? quantityToAddToBasket + 1 : quantityToAddToBasket - 1;
			return quantityToAddToBasket;
		}
	}
}
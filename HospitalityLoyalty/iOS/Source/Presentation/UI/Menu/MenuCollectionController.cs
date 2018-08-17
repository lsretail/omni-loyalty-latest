using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Infrastructure.Data.Omniservice.Loyalty.Hospitality.Menus;
using Presentation.Models;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation.UI
{
	public class MenuCollectionController : BaseCollectionViewController, IItemSelectedListener, IBasketSelectedListener
	{
		private int currentCellStyle;

		//private UIRefreshControl refreshControl;

		private ErrorGettingDataView errorGettingDataView;


		public int CurrentCellStyle{
			get { return currentCellStyle; }
			set {
				currentCellStyle = value;
				ChangeStyle();
			}
		}
		private UIButton changeLayoutButton;
		private MenuService menuService;
		private MenuModel menuModel;
		private MenuNode node;
		private LSRetail.Omni.Domain.DataModel.Base.Menu.Menu selectedMenu;

		private MenuCollectionViewSource menuCollectionSource;
		private MenuCollectionViewDelegateFlowLayout menuCollectionLayout;
		private string nodeId = string.Empty;
		private string menuId = string.Empty;

		public MenuCollectionController(string nodeId="", string menuId="")
		{
			int.TryParse(Settings.GetStyleId(), out currentCellStyle);

			changeLayoutButton = new UIButton();
			CollectionView.RefreshControl = new UIRefreshControl();
			CollectionView.RefreshControl.ValueChanged += RefreshControl_ValueChanged;
			CollectionView.RegisterClassForCell(typeof(MenuGroupCollectionRowCell), MenuGroupCollectionRowCell.CellIdentifier);
			CollectionView.RegisterClassForCell(typeof(MenuGroupCollectionThumbnailCell), MenuGroupCollectionThumbnailCell.CellIdentifier);
			CollectionView.RegisterClassForCell(typeof(MenuItemCollectionViewThumbnailCell), MenuItemCollectionViewThumbnailCell.CellIdentifier);
			CollectionView.RegisterClassForCell(typeof(MenuItemCollectionViewRowCell), MenuItemCollectionViewRowCell.CellIdentifier);

			CollectionView.BackgroundColor = Utils.AppColors.BackgroundGray;
			CollectionView.DataSource = menuCollectionSource = new MenuCollectionViewSource(this);
			CollectionView.Delegate = menuCollectionLayout = new MenuCollectionViewDelegateFlowLayout(this);

			menuCollectionLayout.ParentFrame = CollectionView.Frame;
			menuCollectionLayout.Style = (Presentation.UI.MenuCollectionCellLayouts)currentCellStyle;
			menuService = new MenuService();
			this.menuId = menuId;
			this.nodeId = nodeId;
			menuModel = new MenuModel();
			Title = string.IsNullOrEmpty(nodeId) ? LocalizationUtilities.LocalizedString("Menu_Menu", "Menu") : "";//"GET NODE DESCRIPT";
			ChangeStyle(); // Used to set style
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			SetRightBarButtonItems();
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (!menuCollectionSource.Loaded)
			{
				LoadMenu();
			}
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		async void RefreshControl_ValueChanged(object sender, EventArgs e)
		{
			await menuModel.GetMobileMenu();
			LoadMenu();
			CollectionView.RefreshControl.EndRefreshing();
		}

		public void ItemSelected(int index)
		{
			MobileMenuNode menu = menuCollectionSource.GetMenu(index);
			if (menu.NodeType == MobileMenuNodeType.Group)
			{
				NavigationController.PushViewController(new MenuCollectionController(menu.Id, ""), true);
			}
			else 
			{
				MenuService service = new MenuService();
				var item = service.GetMenuItem(AppData.MobileMenu, menu.Id, menu.NodeLineType); 
				ItemDetailController detailController = new ItemDetailController(item);

				this.NavigationController.PushViewController(detailController, true);	
			}
		}

		private void LoadMenu(int position)
		{
			selectedMenu = AppData.MobileMenu.MenuNodes[position];

			var mobileMenuNodes = menuService.GetMobileMenuNodes(selectedMenu);

			menuCollectionSource.SetData(mobileMenuNodes);
			CollectionView.ReloadData();
		}


		private async void LoadMenu()
		{
			if (string.IsNullOrEmpty(nodeId))
			{
				if (AppData.MobileMenuWasLoadedFromServer && AppData.MobileMenu.MenuNodes.Count > 1)
				{
					LoadMenu(string.IsNullOrEmpty(menuId) ? MenuUtils.DefaultMenu : MenuUtils.GetMenuPositionById(menuId));
				}
				else if (AppData.MobileMenuWasLoadedFromServer && AppData.MobileMenu.MenuNodes.Count == 1)
				{
					LoadMenu(0);
				}
				else
				{
					//TODO PROGRESS HUD
					if (await menuModel.GetMobileMenu())
					{
						LoadMenu();
					}
					else
					{
						ShowErrorGettingDataView();
						//TODO Unable to fetch menus.
					}
					//LoadMenu();
				}
			}
			else
			{
				if (string.IsNullOrEmpty(menuId))
				{
					foreach (var menu in AppData.MobileMenu.MenuNodes)
					{
						if (menu.MenuNodes.FirstOrDefault(x => x.Id == nodeId) != null)
						{
							menuId = menu.Id;
							break;
						}
					}
				}

				if (!string.IsNullOrEmpty(menuId))
				{
					selectedMenu = AppData.MobileMenu.MenuNodes.FirstOrDefault(x => x.Id == menuId);
				}
				else if (AppData.MobileMenu.MenuNodes != null && AppData.MobileMenu.MenuNodes.Count > 0)
				{
					selectedMenu = AppData.MobileMenu.MenuNodes[0];
				}

				if (selectedMenu == null)
				{
					return;
				}

				node = selectedMenu.MenuNodes.FirstOrDefault(x => x.Id == nodeId);

				if (node == null)
				{
					return;
				}

				var mobileMenuNodes = menuService.GetMobileMenuNodes(AppData.MobileMenu, node);

				menuCollectionSource.SetData(mobileMenuNodes);
				CollectionView.ReloadData();

				Title = node.Description;
			}
		}

		public void SetRightBarButtonItems()
		{
			changeLayoutButton.SetImage(Utils.UI.GetColoredImage(GetImage(currentCellStyle + 1), UIColor.White), UIControlState.Normal);
			changeLayoutButton.Frame = new CGRect(0, 0, 30, 30);

			changeLayoutButton.TouchUpInside += ChangeCollectionLayout;

			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(changeLayoutButton);
		}

		private void ChangeCollectionLayout(object sender, EventArgs e)
		{
			CurrentCellStyle++;
		}

		private void ChangeStyle()
		{
			if (currentCellStyle >= Enum.GetNames(typeof(MenuCollectionCellLayouts)).Length)
			{
				currentCellStyle = 0;
			}

			changeLayoutButton.SetImage(Utils.UI.GetColoredImage(GetImage(currentCellStyle + 1), UIColor.White), UIControlState.Normal);
			menuCollectionLayout.Style = (Presentation.UI.MenuCollectionCellLayouts)(currentCellStyle);
			menuCollectionSource.Style = (Presentation.UI.MenuCollectionCellLayouts)(currentCellStyle);
			CollectionView.ReloadData();
			Settings.SetStyleId(currentCellStyle.ToString());
		}

		public static UIImage GetImage(int cellStyle)
		{
			if (cellStyle >= Enum.GetNames(typeof(MenuCollectionCellLayouts)).Length)
			{
				cellStyle = 0;
			}

			switch ((MenuCollectionCellLayouts)cellStyle)
			{
				case MenuCollectionCellLayouts.ThumbnailLarge:
					return UIImage.FromBundle("ViewLargeCellsIcon");
				case MenuCollectionCellLayouts.ThumbnailSmall:
					return UIImage.FromBundle("ViewGridIcon");
				case MenuCollectionCellLayouts.Row:
					return UIImage.FromBundle("ViewSmallCellsIcon");
				default:
					return UIImage.FromBundle("ViewGridIcon");
			}
		}

		public void BasketSelected(MobileMenuNode menu)
		{
			MenuItem item = menuService.GetMenuItem(AppData.MobileMenu, menu.Id, menu.NodeLineType);
			// Have to check if there are any required modifiers.
			// If there are, show the modifiersscreen but only display the required modifiers.
			// If there aren't, add the item straight to basket and bypass the modifiersscreen.


			bool hasRequiredModifiers = menuService.HasAnyRequiredModifers(AppData.MobileMenu, item);
			if (hasRequiredModifiers)
			{
				// Go to modifiers screen, but show only required modifiers
				AddToBasketController addToBasketController = new AddToBasketController(item.Clone(), 1, true);
				this.PresentViewController(new UINavigationController(addToBasketController), true, null);
			}
			else
			{
				// Add item straight to basket (no modifiers screen)
				new Models.BasketModel().AddItemToBasket(item.Clone(), 1);
				//Utils.Util.AppDelegate.SlideoutBasket.Refresh();

				Utils.UI.bannerViewTimer.Start();
				Utils.UI.ShowAddedToBasketBannerView(LocalizationUtilities.LocalizedString("SlideoutBasket_ItemAddedToBasket", "Vöru var bætt í körfuna!"), Utils.Image.FromFile("/Branding/Standard/default_map_location_image.png"));
			}		
		}


		private void ShowErrorGettingDataView()
		{
			if (this.errorGettingDataView == null)
			{
				if(IsViewLoaded)
				{
					CGRect errorGettingDataViewFrame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Bounds.Width, this.View.Bounds.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length);
					this.errorGettingDataView = new ErrorGettingDataView(errorGettingDataViewFrame);
					this.errorGettingDataView.Retry += Retry;
					this.View.AddSubview(this.errorGettingDataView);
				}
			}
			else
			{
				this.errorGettingDataView.Hidden = false;
			}
		}

		private async void Retry(object sender, EventArgs e)
		{
			HideErrorGettingDataView();
			CollectionView.SetContentOffset(new CGPoint(0, -CollectionView.RefreshControl.Frame.Height), true);
			if (await menuModel.GetMobileMenu())
			{
				LoadMenu();
			}
			else
			{
				ShowErrorGettingDataView();
			}
			CollectionView.RefreshControl.EndRefreshing();
		}

		private void HideErrorGettingDataView()
		{
			if (this.errorGettingDataView != null)
				this.errorGettingDataView.Hidden = true;
		}
	}
}

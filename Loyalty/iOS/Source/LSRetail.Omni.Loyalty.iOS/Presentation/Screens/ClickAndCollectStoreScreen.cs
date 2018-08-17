using System;
using UIKit;
using System.Collections.Generic;
using Domain.Stores;
using CoreGraphics;
using Presentation.Utils;
using Foundation;
using System.Linq;
using CoreLocation;

namespace Presentation.Screens
{
	/*
	public class ClickAndCollectStoreScreen : UIViewController
	{
		private UITableView clickAndCollectTableView;
		private ClickAndCollectStoreScreenTableSource clickAndCollectTableViewSource;
		private ErrorGettingDataView errorGettingDataView;

		private CLLocationManager locationManager;

		public List<Store> Stores;
		private bool HasData { get; set; }

		public ClickAndCollectStoreScreen ()
		{
			this.Title = NSBundle.MainBundle.LocalizedString("ClickCollect_Stores", "Stores");

			this.Stores = new List<Store>() ;

			this.clickAndCollectTableView = new UITableView();
			this.clickAndCollectTableViewSource = new ClickAndCollectStoreScreenTableSource(this);
			this.clickAndCollectTableView.Source = this.clickAndCollectTableViewSource;
			this.HasData = false;
			this.locationManager = new CLLocationManager ();

			if (!HasData)
			{
				GetData ();
			}
		}
			
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);

			if (this.clickAndCollectTableView.Source == null)
				this.clickAndCollectTableView.Source = new ClickAndCollectStoreScreenTableSource(this);

			(this.clickAndCollectTableView.Source as ClickAndCollectStoreScreenTableSource).RefreshData();
			this.clickAndCollectTableView.ReloadData();
		}
			
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			this.View.BackgroundColor = UIColor.White;

			this.clickAndCollectTableView.BackgroundColor = Utils.AppColors.BackgroundGray;
			clickAndCollectTableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
			clickAndCollectTableView.Tag = 100;
			clickAndCollectTableView.Hidden = false;
			this.View.AddSubview(clickAndCollectTableView);

			this.View.ConstrainLayout(() =>

				clickAndCollectTableView.Frame.Top == this.View.Bounds.Top &&
				clickAndCollectTableView.Frame.Left == this.View.Bounds.Left &&
				clickAndCollectTableView.Frame.Right == this.View.Bounds.Right &&
				clickAndCollectTableView.Frame.Bottom == this.View.Bounds.Bottom
			);

			SetRightBarButtonItems();

			RefreshData();
		}

		private void SetRightBarButtonItems()
		{
			UIButton mapButton = new UIButton();
			mapButton.SetImage (Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("MapIcon.png"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
			mapButton.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
			mapButton.Frame = new CGRect (0, 0, 30, 30);
			mapButton.TouchUpInside += (sender, e) => 
			{
				MapController map = new MapController (this.Stores, true);
				this.NavigationController.PushViewController(map, true);
			};

			this.NavigationItem.RightBarButtonItem = new UIBarButtonItem(mapButton);
		}

		public void StoreSelected(Store store)
		{
			Console.WriteLine(store.Description + " selected");
			Utils.UI.ShowLoadingIndicator();

			new Models.ClickCollectModel().CheckAvailability(store.Id, 
			(newBasket, unavailableItems) =>
				{
					//success

					if(newBasket != null)
					{
						// not all items asked for are available

						if(newBasket.Items.Count == 0)
						{
							Utils.UI.HideLoadingIndicator();

							UIAlertView noItemsAvailableAlert = new UIAlertView(NSBundle.MainBundle.LocalizedString("ClickCollect_NotAvailableTitle", "Not available in this store"), 
																				NSBundle.MainBundle.LocalizedString("ClickCollect_NotAvailableMsg", "Item/s not available, please try another store"), 
																				null, 
																				NSBundle.MainBundle.LocalizedString("General_OK", "OK"), 
																				null);
							noItemsAvailableAlert.Show();
						}
						else
						{
							// calculate the new basket
							new Models.BasketModel ().CalculateBasket (newBasket,
								(calculatedBasket) => 
								{
									//success
									Utils.UI.HideLoadingIndicator();

									ConfirmOrderController confirmOrderController = new ConfirmOrderController (store, calculatedBasket, unavailableItems, false);
									this.NavigationController.PushViewController (confirmOrderController, true);
								},
								() => 
								{
									//failure
									Utils.UI.HideLoadingIndicator();
								}
							);
						}
					}
					else
					{
						Utils.UI.HideLoadingIndicator();

						// all items are available - the basket doesn't change
						ConfirmOrderController confirmOrderController = new ConfirmOrderController (store, AppData.Device.UserLoggedOnToDevice.Basket, null, true);
						this.NavigationController.PushViewController (confirmOrderController, true);
					}

				},
				() =>
				{
					//failure
					Utils.UI.HideLoadingIndicator();
				}
			);
		}

		public void StoreInfoButtonPressed(Store store)
		{
			LocationDetailController locationDetailsScreen = new LocationDetailController(store, this.Stores, true);
			this.NavigationController.PushViewController(locationDetailsScreen, true);
		}

		private void RefreshData()
		{
			this.clickAndCollectTableViewSource.RefreshData();
			this.clickAndCollectTableView.Source = this.clickAndCollectTableViewSource;
			this.clickAndCollectTableView.ReloadData();
		}

		public void GetData()
		{
			System.Diagnostics.Debug.WriteLine ("LocationsScreen.GetData running");
			Utils.UI.ShowLoadingIndicator();

			if(Utils.Util.GetOSVersion().Major >= 8)
			{
				locationManager.RequestWhenInUseAuthorization();
			}

			locationManager.StartUpdatingLocation();

			locationManager.LocationsUpdated += (sender, e) => 
			{
				// We can access user's location
				locationManager.StopUpdatingLocation();

				if(locationManager.Location != null)
				{
					CLLocationCoordinate2D coord = locationManager.Location.Coordinate;

					if(!HasData)
					{
						HasData = true;
						new Models.StoreModel().GetStoresByCoordinates(coord.Latitude, coord.Longitude, 100000, 100, GetStoresSuccess, GetStoresFailure);
					}
				}
			};

			locationManager.Failed += (sender, e) => 
			{
				// We can't access user's locations - he probably didn't allow it

				locationManager.StopUpdatingLocation();

				if(!HasData)
				{
					HasData = true;
					new Models.StoreModel ().GetAllStores (GetStoresSuccess, GetStoresFailure);
				}
			};
		}

		private void GetStoresSuccess(List<Store> stores)
		{
			System.Diagnostics.Debug.WriteLine ("LocationsScreen.GetData success");

			this.Stores = stores.FindAll(x => x.IsClickAndCollect == true);

			Utils.UI.HideLoadingIndicator();
			HideErrorGettingDataView();

			RefreshData ();
		}

		private void GetStoresFailure()
		{
			System.Diagnostics.Debug.WriteLine ("LocationsScreen.GetData failure");

			HasData = false;
			Utils.UI.HideLoadingIndicator();
			ShowErrorGettingDataView();
		}

		private void ShowErrorGettingDataView()
		{
			if (this.errorGettingDataView == null)
			{
				CGRect errorGettingDataViewFrame = new CGRect(0, this.TopLayoutGuide.Length, this.View.Bounds.Width, this.View.Bounds.Height - this.TopLayoutGuide.Length - this.BottomLayoutGuide.Length);
				this.errorGettingDataView = new ErrorGettingDataView(errorGettingDataViewFrame, GetData);
				this.View.AddSubview(this.errorGettingDataView);
			}
			else
			{
				this.errorGettingDataView.Hidden = false;
			}
		}

		private void HideErrorGettingDataView()
		{
			if (this.errorGettingDataView != null)
				this.errorGettingDataView.Hidden = true;
		}


		private class ClickAndCollectStoreScreenTableSource : UITableViewSource
		{
			private ClickAndCollectStoreScreen controller;
			private UIView headerView;
			private List<Store> stores;
			public bool HasData { get { return this.stores.Count > 0; } }

			public ClickAndCollectStoreScreenTableSource (ClickAndCollectStoreScreen controller)
			{
				this.controller = controller;
				this.stores = controller.Stores;
				RefreshData();

				BuildHeaderView();
			}

			private void BuildHeaderView()
			{
				headerView = new UIView();
				headerView.BackgroundColor = Utils.AppColors.TransparentWhite;


				// Total
				UILabel lblSelect = new UILabel()
				{
					Text = NSBundle.MainBundle.LocalizedString("ClickCollect_SelectStore", "Select store to collect from" + ":"),
					TextColor = AppColors.PrimaryColor,
					BackgroundColor = UIColor.Clear,
					TextAlignment = UITextAlignment.Center,
					Font = UIFont.SystemFontOfSize(16)
				};
				headerView.AddSubview (lblSelect);

				headerView.ConstrainLayout(() =>

					lblSelect.Frame.GetCenterY() == headerView.Frame.GetCenterY() &&
					lblSelect.Frame.Left == headerView.Frame.Left &&
					lblSelect.Frame.Right == headerView.Frame.Right &&
					lblSelect.Frame.Width == headerView.Frame.Width
				);
			}

			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return this.stores.Count;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				ClickAndCollectStoreScreenTableViewCell cell = tableView.DequeueReusableCell (ClickAndCollectStoreScreenTableViewCell.Key) as ClickAndCollectStoreScreenTableViewCell;
				if (cell == null)
					cell = new ClickAndCollectStoreScreenTableViewCell();

				Store store = this.stores [indexPath.Row];

				string title = store.Description;
				string distance = decimal.Round(store.Distance).ToString() + " " + NSBundle.MainBundle.LocalizedString("ClickCollect_StoreDistance", "km. away from here");
				string extraInfo = store.FormatAddress + "\n" + "\n" + distance;


				// Image
				Domain.Images.ImageView imageView = store.Images.FirstOrDefault();
				string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
				string imageId = (imageView != null ? imageView.Id : string.Empty);

				cell.SetValues(indexPath.Row, HandleInfoButtonPress, title, extraInfo, imageAvgColor, imageId);

				return cell;
			}

			public override UIView GetViewForHeader (UITableView tableView, nint section)
			{
				if (section == 0)
					return this.headerView;
				else
					return null;
			}

			public override nfloat GetHeightForHeader (UITableView tableView, nint section)
			{
				if (section == 0)
					return 44f;
				else
					return 0f;
			}

			public void HandleInfoButtonPress(int cellIndexPathRow)
			{
				Store store = this.stores[cellIndexPathRow];
				this.controller.StoreInfoButtonPressed(store);
			}

			public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
			{
				Store store = this.stores[indexPath.Row];
				this.controller.StoreSelected(store);

				tableView.DeselectRow(indexPath, true);
			}

			public override nfloat GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				Store store = this.stores [indexPath.Row];

				return ClickAndCollectStoreScreenTableViewCell.GetCellHeight (store.FormatAddress);
			}

			public void RefreshData()
			{
				this.stores = this.controller.Stores;
			}
		}


		private class ClickAndCollectStoreScreenTableViewCell : UITableViewCell
		{
			public static string Key = "ClickAndCollectStoreScreenTableViewCell";

			protected int id;
			private Action<int> onInfoButtonPressed;

			private const float titleLabelHeight = 20f;
			private const float priceLabelHeight = 20f;
			private const float margin = 5f;
			private const float buttonDimensions = 40f;
			private const float interCellSpacing = 10f;

			public ClickAndCollectStoreScreenTableViewCell() : base(UITableViewCellStyle.Default, Key)
			{
				this.BackgroundColor = UIColor.Clear;
				this.SelectionStyle = UITableViewCellSelectionStyle.None;

				SetLayout();
			}

			public virtual void SetLayout()
			{
				UIView customContentView = new UIView();
				customContentView.BackgroundColor = UIColor.White;
				customContentView.Tag = 1;
				this.ContentView.AddSubview(customContentView);

				UIImageView imageView = new UIImageView();
				imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
				imageView.ClipsToBounds = true;
				imageView.BackgroundColor = UIColor.Purple;
				imageView.Tag = 500;
				customContentView.AddSubview(imageView);

				UILabel lblTitle = new UILabel();
				lblTitle.BackgroundColor = UIColor.Clear;
				lblTitle.TextColor = Utils.AppColors.PrimaryColor;
				lblTitle.Tag = 100;
				customContentView.AddSubview(lblTitle);

				UILabel lblExtraInfo = new UILabel();
				lblExtraInfo.BackgroundColor = UIColor.Clear;
				lblExtraInfo.TextColor = UIColor.Gray;
				lblExtraInfo.Font = UIFont.SystemFontOfSize(12f);
				lblExtraInfo.Tag = 200;
				customContentView.AddSubview(lblExtraInfo);

				UIButton btnStoreInfo = new UIButton();
				btnStoreInfo.SetImage(Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile ("InfoIcon.png"), UIColor.Red), UIControlState.Normal);
				btnStoreInfo.ImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
				btnStoreInfo.ImageEdgeInsets = new UIEdgeInsets(5, 5, 5, 5);
				btnStoreInfo.BackgroundColor = UIColor.Clear;
				btnStoreInfo.TouchUpInside += (object sender, EventArgs e) => { 

					this.onInfoButtonPressed(this.id);
					btnStoreInfo.SetImage(GetInfoButtonIcon(), UIControlState.Normal);

				};
				btnStoreInfo.Tag = 300;
				customContentView.AddSubview(btnStoreInfo);


				this.ContentView.ConstrainLayout(() =>

					customContentView.Frame.Top ==  this.ContentView.Bounds.Top + interCellSpacing &&
					customContentView.Frame.Left == this.ContentView.Bounds.Left &&
					customContentView.Frame.Right == this.ContentView.Bounds.Right &&
					customContentView.Frame.Bottom == this.ContentView.Bounds.Bottom

				);

				customContentView.ConstrainLayout(() =>

					imageView.Frame.Top == customContentView.Frame.Top + 2 * margin &&
					imageView.Frame.Left == customContentView.Bounds.Left + 5 &&
					imageView.Frame.Height == 95f &&
					imageView.Frame.Width == 95f &&

					lblTitle.Frame.Top == imageView.Frame.Top &&
					lblTitle.Frame.Left == imageView.Frame.Right + 2 * margin &&
					lblTitle.Frame.Right == customContentView.Frame.Right - 2 * margin &&
					lblTitle.Frame.Height == titleLabelHeight &&

					lblExtraInfo.Frame.Top == lblTitle.Frame.Bottom + margin &&
					lblExtraInfo.Frame.Left == lblTitle.Frame.Left &&
					lblExtraInfo.Frame.Right == lblTitle.Frame.Right &&


					btnStoreInfo.Frame.Bottom == customContentView.Bounds.Bottom - 2 * margin &&
					btnStoreInfo.Frame.Right == lblExtraInfo.Frame.Right &&
					btnStoreInfo.Frame.Width == buttonDimensions &&
					btnStoreInfo.Frame.Height == buttonDimensions
				);
			}

			private UIImage GetInfoButtonIcon()
			{
				return Utils.UI.GetColoredImage (Presentation.Utils.Image.FromFile ("InfoIcon.png"), UIColor.Gray);
			}

			public void SetValues(int id, Action<int> onInfoButtonPressed, string title, string extraInfo, string imageAvgColorHex, string imageId)
			{
				this.id = id;
				this.onInfoButtonPressed = onInfoButtonPressed;

				UILabel lblTitle = (UILabel)this.ContentView.ViewWithTag(100);
				lblTitle.Text = title;

				UILabel lblExtraInfo = (UILabel)this.ContentView.ViewWithTag(200);
				lblExtraInfo.Lines = Utils.Util.GetStringLineCount(extraInfo);
				lblExtraInfo.Text = extraInfo;
				//lblExtraInfo.SizeToFit();

				UIButton btnInfo = (UIButton)this.ContentView.ViewWithTag(300);
				btnInfo.SetImage(GetInfoButtonIcon(), UIControlState.Normal);

				UIImageView imageView = (UIImageView)this.ContentView.ViewWithTag(500);
				if (String.IsNullOrEmpty(imageAvgColorHex))
					imageAvgColorHex = "E0E0E0"; // Default to light gray
				imageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(imageAvgColorHex);

				Utils.UI.LoadImageToImageView(imageId, false, imageView, new Domain.Images.ImageSize(200, 200), this.id.ToString());
			}

			private static nfloat GetExtraInfoLabelHeight(string extraInfoString)
			{
				// Let's get the height of the extra info label by creating a templabel that is exactly like the one used in the
				// actual cell instance and then apply SizeToFit().

				UILabel tempLabel = new UILabel();
				tempLabel.Text = extraInfoString;
				tempLabel.Font = UIFont.SystemFontOfSize(12f);
				tempLabel.Lines = Utils.Util.GetStringLineCount(extraInfoString);
				tempLabel.SizeToFit();
				return tempLabel.Frame.Height;
			}

			public static nfloat GetCellHeight(string extraInfoString)
			{
				nfloat minHeight = interCellSpacing + 2 * margin + titleLabelHeight + 2 * margin + Math.Max(priceLabelHeight, buttonDimensions) + margin;
				return minHeight + GetExtraInfoLabelHeight(extraInfoString);
			}
		}
	}
	*/
}


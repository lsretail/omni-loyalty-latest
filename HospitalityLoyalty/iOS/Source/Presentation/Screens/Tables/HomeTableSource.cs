using System;
using System.Collections.Generic;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using UIKit;

namespace Presentation.Screens
{
	public class HomeTableSource : UITableViewSource
	{
		private Action<ShortcutIds> onShortcutRowSelected;
		private List<CellTemplate> cellTemplates;

		public HomeTableSource(Action<ShortcutIds> onRowSelected, List<ShortcutIds> shortcutIds)
		{
			this.cellTemplates = new List<CellTemplate>();

			this.onShortcutRowSelected = onRowSelected;
			BuildCellTemplates(shortcutIds);
		}

		private void BuildCellTemplates(List<ShortcutIds> shortcutIds)
		{
			this.cellTemplates.Clear();

			foreach (ShortcutIds shortcutId in shortcutIds)
			{
				this.cellTemplates.Add(
					new CellTemplate()
					{
						Id = shortcutId,
						DisplayName = MapShortcutIdToDisplayName(shortcutId)
					}
				);
			}
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return this.cellTemplates.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			HomeScreenCell cell = tableView.DequeueReusableCell(HomeScreenCell.Key) as HomeScreenCell;
			if (cell == null)
				cell = new HomeScreenCell();

			string title = this.cellTemplates[indexPath.Row].DisplayName;
			UIImage image = null;

			if (indexPath.Row == 0)
				image = Utils.UI.GetColoredImage(Presentation.Utils.Image.FromFile("/Icons/IconMenu.png"), UIColor.White);
			else
				image = Utils.UI.GetColoredImage(UIImage.FromBundle("FavoriteOffIcon"), UIColor.White);

			cell.SetValues(title, image);

			return cell;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			this.onShortcutRowSelected(this.cellTemplates[indexPath.Row].Id);
			tableView.DeselectRow(indexPath, true);
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 54f;
		}

		private string MapShortcutIdToDisplayName(ShortcutIds sId)
		{
			switch (sId)
			{
				case ShortcutIds.Home:
					return LocalizationUtilities.LocalizedString("Home_Home", "Home");
				case ShortcutIds.Locations:
					return LocalizationUtilities.LocalizedString("Locations_Locations", "Restaurants");
				case ShortcutIds.Menu:
					return LocalizationUtilities.LocalizedString("Menu_Menu", "Menu");
				case ShortcutIds.OffersAndCoupons:
					return LocalizationUtilities.LocalizedString("OffersAndCoupons_OffersAndCoupons", "Offers & coupons");
				case ShortcutIds.History:
					return LocalizationUtilities.LocalizedString("History_History", "History");
				case ShortcutIds.Favorites:
					return LocalizationUtilities.LocalizedString("Favorites_Favorites", "Favorites");
				default:
					return string.Empty;
			}
		}

		private class CellTemplate
		{
			public ShortcutIds Id;
			public string DisplayName;
		}

		public enum ShortcutIds
		{
			Home,
			Menu,
			Locations,
			OffersAndCoupons,
			Favorites,
			History
		}
	}

	public class HomeScreenCell : UITableViewCell
	{
		public static string Key = "HomeScreenCell";

		private UIView customContentView;
		private UIImageView imageView;
		private UILabel lblTitle;

		public HomeScreenCell() : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.Default;

			SetLayout();
		}

		public virtual void SetLayout()
		{
			this.customContentView = new UIView();
			customContentView.BackgroundColor = Utils.AppColors.PrimaryColor;
			this.ContentView.AddSubview(customContentView);

			this.imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.Clear;
			this.customContentView.AddSubview(imageView);

			this.lblTitle = new UILabel();
			lblTitle.BackgroundColor = UIColor.Clear;
			lblTitle.TextColor = UIColor.White;
			this.customContentView.AddSubview(lblTitle);
		}

		public void SetValues(string title, UIImage image)
		{
			this.imageView.Image = image;
			this.lblTitle.Text = title;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			nfloat margin = 10f;
			nfloat imageDimension = 30f;

			this.customContentView.Frame = new CoreGraphics.CGRect(margin, margin, this.ContentView.Frame.Width - 2 * margin, this.ContentView.Frame.Height - margin);
			this.imageView.Frame = new CoreGraphics.CGRect(margin, 7f, imageDimension, imageDimension);
			this.lblTitle.Frame = new CoreGraphics.CGRect(this.imageView.Frame.Right + 2 * margin, 0f, this.ContentView.Frame.Width - imageDimension - 2 * margin, this.customContentView.Frame.Height);
		}
	}
}
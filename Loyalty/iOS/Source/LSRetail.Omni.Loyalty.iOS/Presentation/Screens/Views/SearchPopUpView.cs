using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
	public class SearchPopUpView : PopUpView
	{
		private UILabel lblChooseCategories;
		private UITableView tblCategories;
		private UIButton btnOk;

		private const int margin = 5;
		private const int btnOkHeight = 50;
		public SearchPopUpDto SearchPopUpDto { get; set; }

		public delegate void OkEventHandler(SearchPopUpDto searchPopUpDto);
		public event OkEventHandler Ok;

		public SearchPopUpView ( SearchPopUpDto searchPopUpDto) : base(true)
		{
			this.BackgroundColor = Utils.AppColors.BackgroundGray;

			this.SearchPopUpDto = searchPopUpDto;
			this.lblChooseCategories = new UILabel ();
			this.lblChooseCategories.Text = LocalizationUtilities.LocalizedString("SearchPopUp_Title", "Please choose categories you want to search in.");
			this.lblChooseCategories.Font = UIFont.SystemFontOfSize(16);
			this.lblChooseCategories.LineBreakMode = UILineBreakMode.WordWrap;
			this.lblChooseCategories.Lines = 0;

			this.tblCategories = new UITableView ();
			this.tblCategories.BackgroundColor = Utils.AppColors.BackgroundGray;
			this.tblCategories.SeparatorStyle = UITableViewCellSeparatorStyle.None;

			SearchPopUpTableSource searchPopUpTableSource = new SearchPopUpTableSource (SetSource ());
			searchPopUpTableSource.UpdateData += UpdateData;
			this.tblCategories.Source = searchPopUpTableSource;
			this.tblCategories.ReloadData ();

			this.btnOk = new UIButton ();
			this.btnOk.SetTitle (LocalizationUtilities.LocalizedString("General_OK", "OK"), UIControlState.Normal);
			this.btnOk.SetTitleColor (Utils.AppColors.SoftWhite, UIControlState.Normal);
			this.btnOk.BackgroundColor = Utils.AppColors.PrimaryColor;
			this.btnOk.TouchUpInside += (sender, e) => {
				if(Ok != null) {
					Ok(this.SearchPopUpDto);
				}
			};

			this.AddSubview (this.lblChooseCategories);
			this.AddSubview (this.tblCategories);
			this.AddSubview (this.btnOk);
		}

		public override void LayoutSubviews ()
		{
			this.lblChooseCategories.Frame = new CoreGraphics.CGRect (
				2 * margin,
				2 * margin, 
				this.Frame.Width - 2 * margin, 
				40f
			);
			this.tblCategories.Frame = new CoreGraphics.CGRect (
				0, 
				lblChooseCategories.Frame.Bottom + 2 * margin, 
				this.Frame.Width,
				this.Frame.Height -  (lblChooseCategories.Frame.Height + btnOkHeight + 2 * margin)
			);
			this.btnOk.Frame = new CoreGraphics.CGRect (
				0, 
				this.tblCategories.Frame.Bottom + 2 * margin,
				this.tblCategories.Frame.Width,
				btnOkHeight
			);
				
			base.LayoutSubviews ();
		}

		private List<Tuple<string,bool>> SetSource()
		{
			List<Tuple<string,bool>> Data = new List<Tuple<string,bool>> ();

			Data.Add(new Tuple<string, bool>(LocalizationUtilities.LocalizedString ("SearchPopUp_Item", "Item"), this.SearchPopUpDto.Item));
			Data.Add(new Tuple<string, bool>(LocalizationUtilities.LocalizedString ("SearchPopUp_Offer", "Offer"), this.SearchPopUpDto.Offer));
			Data.Add(new Tuple<string, bool>(LocalizationUtilities.LocalizedString ("SearchPopUp_Coupon", "Coupon"), this.SearchPopUpDto.Coupon));
			Data.Add(new Tuple<string, bool>(LocalizationUtilities.LocalizedString ("SearchPopUp_Notification", "Notification"), this.SearchPopUpDto.Notification));
			Data.Add(new Tuple<string, bool>(LocalizationUtilities.LocalizedString ("SearchPopUp_History", "History"), this.SearchPopUpDto.History));
			Data.Add(new Tuple<string, bool>(LocalizationUtilities.LocalizedString ("SearchPopUp_ShoppingList", "Shopping list"), this.SearchPopUpDto.ShoppingList));
			Data.Add(new Tuple<string, bool>(LocalizationUtilities.LocalizedString ("SearchPopUp_Store", "Store"), this.SearchPopUpDto.Store));

			return Data;
		}

		public void UpdateData(string text, bool value)
		{
			if (text == LocalizationUtilities.LocalizedString ("SearchPopUp_Item", "Item"))
				this.SearchPopUpDto.Item = value;
			if (text == LocalizationUtilities.LocalizedString ("SearchPopUp_Offer", "Offer"))
				this.SearchPopUpDto.Offer = value;
			if (text == LocalizationUtilities.LocalizedString ("SearchPopUp_Coupon", "Coupon"))
				this.SearchPopUpDto.Coupon = value;
			if (text == LocalizationUtilities.LocalizedString ("SearchPopUp_Notification", "Notification"))
				this.SearchPopUpDto.Notification = value;
			if (text == LocalizationUtilities.LocalizedString ("SearchPopUp_History", "History"))
				this.SearchPopUpDto.History = value;
			if (text == LocalizationUtilities.LocalizedString ("SearchPopUp_ShoppingList", "Shopping list"))
				this.SearchPopUpDto.ShoppingList = value;
			if (text == LocalizationUtilities.LocalizedString ("SearchPopUp_Store", "Store"))
				this.SearchPopUpDto.Store = value;
		}



		private class SearchPopUpTableSource : UITableViewSource
		{
			private List<Tuple<string,bool>> searchAvailabilitylist;

			public delegate void UpdateDataEventHandler(string title, bool value);
			public event UpdateDataEventHandler UpdateData;

			public SearchPopUpTableSource(List<Tuple<string,bool>> searchAvailability)
			{
				this.searchAvailabilitylist = searchAvailability;
			}
			public override nint NumberOfSections (UITableView tableView)
			{
				return 1;
			}


			public override nint RowsInSection (UITableView tableview, nint section)
			{
				return this.searchAvailabilitylist.Count;
			}

			public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
			{
				return 51f;
			}

			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell (SwitchCell.Key) as SwitchCell;
				if (cell == null) {
					cell = new SwitchCell ();
				}

				cell.Update = UpdateDataValue;
				cell.SetValues (searchAvailabilitylist[indexPath.Row].Item1, searchAvailabilitylist[indexPath.Row].Item2);
				return cell;
			}
				
			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				//this.controller.HideSearchBarKeyboard();

				tableView.DeselectRow (indexPath, true); // normal iOS behaviour is to remove the blue highlight
			}
				

			private void UpdateDataValue(string title, bool value)
			{
				if( UpdateData != null )
				{
					int index = 0;
					foreach (var searchAvailabilityItem in searchAvailabilitylist) {
						if (searchAvailabilityItem.Item1 == title)
							break;
						index++;
					}

					this.searchAvailabilitylist [index] = new Tuple<string, bool> (title, value);
					UpdateData (title, value);
				}
			}
		}
	}
}


using System;
using UIKit;
using Presentation.Utils;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation.Tables
{
	public class SearchCell : UITableViewCell
	{
		private UIImageView imgSearchRs;
		private UILabel lblCaption;
		private UIView customContentView;
		public static string Key = "SearchCell";
		protected int id;

		public SearchCell () : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;
			//this.Accessory = UITableViewCellAccessory.DetailButton;

			SetLayout();
		}

		private void SetLayout()
		{
			customContentView = new UIView();
			customContentView.BackgroundColor = UIColor.White;
			this.ContentView.AddSubview(customContentView);

			this.imgSearchRs = new UIImageView ()
			{
				ContentMode = UIViewContentMode.ScaleAspectFill,
				ClipsToBounds = true,
				Tag = 100
			};
			customContentView.AddSubview (this.imgSearchRs);


			this.lblCaption = new UILabel()
			{
				TextColor = Utils.AppColors.PrimaryColor,
				Font = UIFont.FromName ("Helvetica", 14),
				TextAlignment = UITextAlignment.Left,
				BackgroundColor = UIColor.Clear,
				Tag = 200
			};
			customContentView.AddSubview (this.lblCaption);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews ();
			const float margin = 5f;
			const float interCellSpacing = 10f;

			this.customContentView.Frame = new CGRect (
				this.ContentView.Bounds.Left + interCellSpacing,
				this.ContentView.Bounds.Top + interCellSpacing,
				this.ContentView.Frame.Width - 2 * interCellSpacing,
				this.ContentView.Frame.Height - interCellSpacing
			);
			this.imgSearchRs.Frame = new CGRect(
				0,
				0, 
				this.customContentView.Frame.Width / 4,
				this.customContentView.Frame.Height
			);
			this.lblCaption.Frame = new CGRect(
				this.imgSearchRs.Frame.Width + margin,
				0, 
				this.ContentView.Frame.Width - (margin + this.imgSearchRs.Frame.Width), 
				this.customContentView.Frame.Height
			);
		}


		public void UpdateCell(int id, object objSearch)
		{
			this.id = id;
			string imageId = string.Empty;

			if(objSearch is PublishedOffer)
			{
				PublishedOffer publishedOffer = objSearch as PublishedOffer;

				this.lblCaption.Text = publishedOffer.Description;

				if(publishedOffer.Images != null && publishedOffer.Images.Count > 0)
				{
					this.imgSearchRs.BackgroundColor = ColorUtilities.GetUIColorFromHexString(publishedOffer.Images[0].AvgColor);
					imageId = publishedOffer.Images[0].Id;
				}
				this.imgSearchRs.Image = null;
			}
			else if (objSearch is ItemCategory)
			{
				this.lblCaption.Text = ((ItemCategory) objSearch).Description;
				if(((ItemCategory) objSearch).Images != null){
					this.imgSearchRs.BackgroundColor = ColorUtilities.GetUIColorFromHexString (((ItemCategory) objSearch).Images[0].AvgColor);
					imageId = ((ItemCategory) objSearch).Images[0].Id;
				}
				this.imgSearchRs.Image = null;
			}
			else if (objSearch is LoyItem)
			{
				this.lblCaption.Text = ((LoyItem) objSearch).Description;
				if(((LoyItem) objSearch).Images != null && ((LoyItem) objSearch).Images.Count > 0){
					this.imgSearchRs.BackgroundColor = ColorUtilities.GetUIColorFromHexString (((LoyItem) objSearch).Images[0].AvgColor);
					imageId = ((LoyItem)objSearch).Images [0].Id;
				}
				this.imgSearchRs.Image = null;

			}
            else if (objSearch is ProductGroup)
			{
				this.lblCaption.Text = ((ProductGroup) objSearch).Description;
				if(((ProductGroup) objSearch).Images != null){
					this.imgSearchRs.BackgroundColor = ColorUtilities.GetUIColorFromHexString (((ProductGroup)  objSearch).Images[0].AvgColor);
					imageId = ((ProductGroup)objSearch).Images[0].Id;
				}
				this.imgSearchRs.Image = null;
			}
			else if (objSearch is Profile)
			{
				this.lblCaption.Text = ((Profile) objSearch).Description;
				this.imgSearchRs.BackgroundColor = Utils.AppColors.PrimaryColor;
				this.imgSearchRs.Image = ImageUtilities.FromFile("home-30.png");
			}
			else if (objSearch is Store)
			{
				this.lblCaption.Text = ((Store) objSearch).Description;
				if(((Store) objSearch).Images != null && ((Store) objSearch).Images.Count > 0){
					this.imgSearchRs.BackgroundColor = ColorUtilities.GetUIColorFromHexString (((Store) objSearch).Images[0].AvgColor);
					imageId = ((Store)objSearch).Images [0].Id;
				}
				this.imgSearchRs.Image = null;
			}
			Utils.UI.LoadImageToImageView(imageId, false, this.imgSearchRs, new ImageSize(700, 500), this.id.ToString());
		}
	}
}


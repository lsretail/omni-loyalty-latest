using System;
using CoreGraphics;
using Foundation;
using UIKit;
using CoreAnimation;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using Presentation.Utils;

namespace Presentation.UI
{
	public class MenuGroupCollectionRowCell : MenuBaseCollectionCell
	{
		internal static string CellIdentifier = "MenuGroupCollectionRowCell";
		private float fontSize = 14f;
		UIImageView imageView;
		UILabel lblText;
		UIView textContainerView;

		[Export("initWithFrame:")]
		public MenuGroupCollectionRowCell(CGRect frame) : base(frame)
		{
			// Image view
			imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.BackgroundColor = UIColor.White;
			imageView.Tag = 100;

			// Text container view
			textContainerView = new UIView();
			textContainerView.BackgroundColor = UIColor.White;
			textContainerView.Tag = 200;

			// Text label
			lblText = new UILabel();
			lblText.TextColor = AppColors.PrimaryColor;
			lblText.Font = UIFont.FromName("Helvetica", fontSize);
			lblText.TextAlignment = UITextAlignment.Left;
			lblText.Tag = 300;
			lblText.BackgroundColor = UIColor.Clear;

			ContentView.AddSubviews(imageView, textContainerView, lblText);
			//textContainerView.AddSubview(lblText);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			SetLayoutImageAndTextContainer();
		}

		public override void SetValue(MobileMenuNode menu)
		{
			base.SetValue(menu);

			imageView.BackgroundColor = Utils.UI.GetUIColorFromHexString(menu.Image.AvgColor);
			imageView.Layer.RemoveAllAnimations();
			imageView.Image = null;

			lblText.Text = menu.Description;

			LoadImageToImageView(menu.Image.Id, false, imageView);
		}

		private void SetLayoutImageAndTextContainer()
		{
			imageView.Frame = new CGRect(0, 0, this.ContentView.Frame.Width / 4, this.ContentView.Frame.Height);

			textContainerView.Frame = new CGRect(imageView.Frame.Right, 0, this.ContentView.Bounds.Width - imageView.Frame.Right, this.ContentView.Frame.Height);

			float margin = 10f;

			lblText.Frame = new CGRect(textContainerView.Frame.X + margin, textContainerView.Frame.Y, textContainerView.Frame.Width - margin, textContainerView.Frame.Height);
		}
	}
}
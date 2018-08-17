using System;
using UIKit;
using CoreGraphics;

namespace Presentation
{
	public class ImageCell : UITableViewCell
	{
		private UIImageView imageView;
		private UILabel lblCaption;
		//private UIView customContentView;
		public static string Key = "IMAGECELL";

		public ImageCell () : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.Default;
			//customContentView = new UIView();
			//customContentView.BackgroundColor = UIColor.White;
			//this.ContentView.AddSubview(customContentView);

			this.imageView = new UIImageView ()
			{
				ContentMode = UIViewContentMode.ScaleAspectFill,
				ClipsToBounds = true,
			};
			AddSubview (this.imageView);


			this.lblCaption = new UILabel()
			{
				Font = UIFont.FromName ("Helvetica", 18),
				TextAlignment = UITextAlignment.Left,
				BackgroundColor = UIColor.Clear,
			};
			AddSubview (this.lblCaption);
		}


		public override void LayoutSubviews()
		{
			base.LayoutSubviews ();
			const float margin = 5f;
			const float imageHeight = 30;

			this.imageView.Frame = new CGRect(
				3 * margin,
				(this.Frame.Height - imageHeight)/2, 
				imageHeight,
				imageHeight
			);

			this.lblCaption.Frame = new CGRect(
				this.imageView.Frame.Right + 5 * margin,
				0, 
				this.Frame.Width - ( 5 * margin + this.imageView.Frame.Right), 
				this.Frame.Height
			);
		}

		public void UpdateCell (string text, UIImage image)
		{
			this.lblCaption.Text = text;
			this.imageView.Image = image;
		}
	}
}


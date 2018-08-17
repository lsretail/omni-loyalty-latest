using System;
using UIKit;
using CoreGraphics;

namespace LSRetail.Omni.Hospitality.Loyalty.iOS
{
	public class ScrollItemView : UIView
	{
		public UITextView TextView {
			get;
			set;
		}

		public UIImageView ImageView {
			get;
			set;
		}

		public string id;

		public delegate void ClickedEventHandler (string id);
		public ClickedEventHandler viewClicked;

		public ScrollItemView (string id)
		{
			this.id = id;
			this.BackgroundColor = UIColor.Clear;
			// Text view
			this.TextView = new UITextView();
			this.TextView.Editable = false;
			this.TextView.BackgroundColor = UIColor.Clear;
			this.TextView.TextColor = UIColor.Gray;
			this.TextView.Font = UIFont.SystemFontOfSize (12);
			this.TextView.TextAlignment = UITextAlignment.Left;

			// Image view
			this.ImageView = new UIImageView();
			this.ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			this.ImageView.ClipsToBounds = true;
			this.ImageView.BackgroundColor = UIColor.Clear;
			this.ImageView.Layer.CornerRadius = 10.0f;

			this.AddGestureRecognizer (new UITapGestureRecognizer ( () => {
				if(this.viewClicked  != null){
					this.viewClicked (this.id);
				}
			}));

			this.AddSubview (this.TextView);
			this.AddSubview (this.ImageView);
		}

		public void SetFrame (nfloat x, nfloat y, nfloat height, nfloat width) 
		{
			nfloat xMargin = 10f;

			this.Frame = new CGRect(
				x,
				y,
				height,
				width
			);

			this.ImageView.Frame = new CGRect (
				0,
				0,
				width, 
				width
			);

			this.TextView.Frame = new CGRect (
				0,
				this.ImageView.Frame.Bottom,
				this.ImageView.Frame.Width,
				(height - this.ImageView.Frame.Bottom) + xMargin
			);
		}
	}
}


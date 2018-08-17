using System;
using UIKit;

namespace Presentation
{
	public class SwitchCell : UITableViewCell
	{
		private UISwitch swcOnOrOff; 
		private UILabel lblCaption;

		public static string Key = "SearchPopUpCell";

		public delegate void SwitchValueChangedDelegate(string text, bool value);
		public SwitchValueChangedDelegate Update;

		public SwitchCell () : base(UITableViewCellStyle.Default, Key)
		{
			this.BackgroundColor = UIColor.Clear;
			this.SelectionStyle = UITableViewCellSelectionStyle.None;

			this.swcOnOrOff = new UISwitch();
			this.swcOnOrOff.OnTintColor =  Utils.AppColors.PrimaryColor;
			this.swcOnOrOff.VerticalAlignment = UIControlContentVerticalAlignment.Center;
			this.swcOnOrOff.ValueChanged += (object sender, EventArgs e) => 
			{
				if(Update != null)
				{
					Update(this.lblCaption.Text, this.swcOnOrOff.On);
				}	
			};
			this.lblCaption = new UILabel();
			this.lblCaption.Font = UIFont.FromName ("Helvetica", 16);
			this.lblCaption.BackgroundColor = UIColor.Clear;

			this.ContentView.AddSubview (this.swcOnOrOff);
			this.ContentView.AddSubview (this.lblCaption);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews ();

			this.lblCaption.Frame = new CoreGraphics.CGRect (
				20,
				0,
				this.ContentView.Frame.Width - this.swcOnOrOff.Frame.Width,
				this.ContentView.Frame.Height
			);
				
			this.swcOnOrOff.Frame = new CoreGraphics.CGRect (
				this.ContentView.Frame.Width - 20 - this.swcOnOrOff.Frame.Width,
				10,
				this.ContentView.Frame.Width / 3,
				this.ContentView.Frame.Height - 10
			);
		}

		public void SetValues( string text, bool onOrOff)
		{
			this.swcOnOrOff.SetState (onOrOff, true);
			this.lblCaption.Text = text;
		}
	}
}


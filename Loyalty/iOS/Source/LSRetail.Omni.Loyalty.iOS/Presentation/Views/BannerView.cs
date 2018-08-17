using System;
using UIKit;
using Foundation;
using CoreGraphics;
using ObjCRuntime;
using System.Timers;
using Presentation.Utils;

namespace Presentation
{
	public class BannerView : UIView
	{
		private UITextView textView;
		private UIImageView imageView;
		private UIView clickableView;
		nfloat frameWidth = Utils.Util.AppDelegate.Window.Frame.Width;

		public Timer timer;

		public delegate void BannerViewClickedEventHandler();
		public event BannerViewClickedEventHandler BannerViewClicked;

		public BannerView()
		{
			this.BackgroundColor = AppColors.TransparentBlack3;

			// Text view
			this.textView = new UITextView();
			this.textView.Editable = false;
			this.textView.BackgroundColor = UIColor.Clear;
			this.textView.TextColor = UIColor.White;
			this.textView.Font = UIFont.SystemFontOfSize(16);
			this.textView.TextAlignment = UITextAlignment.Center;

			// Image view
			this.imageView = new UIImageView();
			this.imageView.ContentMode = UIViewContentMode.ScaleAspectFill;

			//Clickable View
			clickableView  = new UIView();
			clickableView.BackgroundColor = UIColor.Clear;

			this.timer = new Timer(4000);
			this.timer.Elapsed += (object sender, ElapsedEventArgs e) => {

				Hide(false);
			};

			this.AddSubview(imageView);
			this.AddSubview(textView);
			this.AddSubview(clickableView);
		}

		private void Main()
		{
			// Intialize the frame for animation
			this.SetFrame ();

			// Set animation
			UIView.BeginAnimations("slideAnimation");
			UIView.SetAnimationDuration (1);

			// Set new frame position
			this.Frame = new CGRect (0, 0, frameWidth, Utils.Util.AppDelegate.StatusbarPlusNavbarHeight);

			// Commit animation
			UIView.CommitAnimations ();

			// event handler if clicked
			clickableView.AddGestureRecognizer (new UITapGestureRecognizer ( () => 
				{
					timer.Stop();
					this.Hide(true);

					if(this.BannerViewClicked != null)
					{
						this.BannerViewClicked();
					}
				}));
		}

		private void SetFrame()
		{
			// Set new frame position
			this.Frame = new CGRect (0, 0, frameWidth, Utils.Util.AppDelegate.StatusbarPlusNavbarHeight);
			this.clickableView.Frame = this.Frame;
			this.imageView.Frame = new CGRect (15f, 24f, 34f, 34f);
			this.textView.Frame = new CGRect (0f, 24f, frameWidth, 44f);
			this.Frame = new CGRect (0, -this.Frame.Height, this.Frame.Width, this.Frame.Height);
		}

		private void SetText(String text)
		{
			this.textView.Text = text;
		}

		private void ClearText ()
		{
			this.textView.Text = string.Empty;
		}

		private void SetImage(UIImage image)
		{
			this.imageView.Image = image;
		}

		public void Show (string textToDisplay, UIImage image)
		{
			this.timer.Start();
			SetText (textToDisplay);
			SetImage(image);
			Main ();
		}

		public void Hide (bool clicked)
		{
			new NSObject ().InvokeOnMainThread (() => 
				{
					this.timer.Stop();
					if(!clicked)
					{
						// Animation to hide notification banner
						UIView.BeginAnimations("slideAnimation");
						UIView.SetAnimationDuration (2);

						// Intialize the frame for animation
						this.Frame = new CGRect (0, -this.Frame.Height, this.Frame.Width, this.Frame.Height);

						//Set up the animation delegate
						UIView.SetAnimationDelegate (this);
						UIView.SetAnimationDidStopSelector (new Selector ("slideAnimationFinished"));

						// Commit animation
						UIView.CommitAnimations ();
					}
					else
					{
						this.RemoveFromSuperview();
					}
				}
			);
		}

		[Export("slideAnimationFinished")]
		void SlideStopped ()
		{
			this.RemoveFromSuperview ();
		}
	}
}


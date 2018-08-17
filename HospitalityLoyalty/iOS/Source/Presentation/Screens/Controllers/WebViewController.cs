using System;
using Foundation;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class WebViewController : UIViewController
	{
		private UIWebView rootView;
		private string URL;

		public WebViewController(string URLToLoad)
		{
			this.URL = URLToLoad;

			this.Title = new Uri(this.URL).Host;
			this.rootView = new UIWebView();
			this.View.AddSubview(this.rootView);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			System.Diagnostics.Debug.WriteLine("LOADING URL: " + this.URL);
			this.rootView.LoadRequest(new NSUrlRequest(new NSUrl(this.URL)));
		}

		public override void ViewDidLayoutSubviews()
		{
			base.ViewDidLayoutSubviews();

			this.rootView.Frame = this.View.Frame;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
		}

		private string FormatUrl(string url)
		{
			if (!(url.StartsWith("http://") || url.StartsWith("https://")))
				url = "http://" + url;

			return url;
		}
	}
}


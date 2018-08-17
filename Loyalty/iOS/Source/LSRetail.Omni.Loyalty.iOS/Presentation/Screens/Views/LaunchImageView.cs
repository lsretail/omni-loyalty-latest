using System;
using UIKit;

namespace Presentation
{
	public class LaunchImageView : UIImageView
	{
		public LaunchImageView ()
		{
			this.Image = Utils.UI.GetLaunchImage();
		}
	}
}


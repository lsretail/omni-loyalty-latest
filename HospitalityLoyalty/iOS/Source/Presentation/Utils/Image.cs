using System;
using CoreGraphics;
using UIKit;
using Foundation;

namespace Presentation.Utils
{
	public class Image
	{
		public static UIImage FromFile(string filename)
		{
			return UIImage.FromFile(@"Images/"+ filename);
		}

		public static UIImage FromBase64(string base64String)
		{
			NSData data = NSData.FromArray(Convert.FromBase64String(base64String));
			return UIImage.LoadFromData(data);
		}
	}
}


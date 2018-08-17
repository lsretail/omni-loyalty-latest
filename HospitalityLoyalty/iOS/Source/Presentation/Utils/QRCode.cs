using System;
using CoreGraphics;
using Foundation;
using UIKit;
using ZXing.QrCode;

namespace Presentation.Utils.QRCode
{
	public static class QRCode
	{
		public static UIImage GenerateQRCode(string xml)
		{
			try
			{
				int height = 280;
				int width = 280;
				var writer = new QRCodeWriter();
				var matrix = writer.encode(xml, ZXing.BarcodeFormat.QR_CODE, width, height);

				return Render(matrix);			 
			}
			catch(Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(ex.ToString());
				return Image.FromFile("/Images/noImage.jpg");
			}
		}

		public static UIImage Render(ZXing.Common.BitMatrix matrix)
		{
			UIGraphics.BeginImageContext(new CGSize(matrix.Width, matrix.Height));
			var context = UIGraphics.GetCurrentContext();

			var black = new CGColor(0f, 0f, 0f);
			var white = new CGColor(1.0f, 1.0f, 1.0f);

			for (int x = 0; x < matrix.Width; x++)
			{
				for (int y = 0; y < matrix.Height; y++)
				{
					context.SetFillColor(matrix[x, y] ? black : white);
					context.FillRect(new CGRect(x, y, 1, 1));
				}
			}

			var img = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return img;
		}
	}
}


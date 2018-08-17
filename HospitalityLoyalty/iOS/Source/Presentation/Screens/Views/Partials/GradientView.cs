using System;
using System.Drawing;
using Foundation;
using UIKit;
using CoreAnimation;
using CoreGraphics;
using ObjCRuntime;

namespace Presentation
{
	//Gradient background. Used in ItemDetailsView.
	//Overrites the UIView Layer to use CAGradientLayer
	//Instead of CALayer to receive gradient effect
	public class GradientView : UIView
	{
		private CGColor color;
		private CGColor gradient;
		private CAGradientLayer gradientLayer
		{
			get { return (CAGradientLayer)this.Layer; }
		}

		public GradientView(CGColor clr, CGColor grditClr, int divide)
		{
			color = clr;
			gradient = grditClr;

			SetColors(divide);
		}

		//Enables us to use GradientLayer instead of Layer
		[Export("layerClass")]
		public static Class LayerClass()
		{
			return new Class(typeof(CAGradientLayer));
		}

		//creates an array of colors the first one being the gradient part.
		//Then it's added to the gradient Layer 
		public void SetColors(int divide)
		{
			CGColor[] tmp = new CGColor[divide];
			tmp[0] = gradient;

			for (var i = 1; i < divide; i++)
			{
				tmp[i] = color;
			}

			gradientLayer.Colors = tmp;
		}
	}
}


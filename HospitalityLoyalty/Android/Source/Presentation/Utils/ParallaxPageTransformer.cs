using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace Presentation.Utils
{
    class ParallaxPageTransformer : Java.Lang.Object, ViewPager.IPageTransformer
    {
	    float parallaxCoefficient;
	    float distanceCoefficient;
	    IEnumerable<int>[] viewLayers;

        public ParallaxPageTransformer(float parallaxCoefficient, float distanceCoefficient, params IEnumerable<int>[] viewLayers)
	    {
		    this.parallaxCoefficient = parallaxCoefficient;
		    this.distanceCoefficient = distanceCoefficient;
		    this.viewLayers = viewLayers;
	    }

	    public void TransformPage (View page, float position)
	    {
		    float coefficient = page.Width * parallaxCoefficient;
		    foreach (var layer in viewLayers) {
			    foreach (var viewID in layer) {
				    var v = page.FindViewById (viewID);
				    if (v != null)
					    v.TranslationX = coefficient * position;
			    }
			    coefficient *= distanceCoefficient;
		    }
	    }
    }
}
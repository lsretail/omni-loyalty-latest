using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Utils;

namespace Presentation.Activities.Image
{
    public class FullScreenImagePagerFragment : BaseFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.FullScreenImagePagerScreen, null);

            var startingPos = Arguments.GetInt(BundleUtils.StartingPos);
            var imageIds = Arguments.GetStringArray(BundleUtils.ImageIds);

            var viewpager = view.FindViewById<ViewPager>(Resource.Id.FullScreenImagePager);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                viewpager.SetPageTransformer(true, new ZoomOutPageTransformer());
            }

            viewpager.Adapter = new FullScreenImagePagerAdapter(ChildFragmentManager, imageIds);

            var indicator = view.FindViewById<Xamarin.ViewPagerIndicator.LinePageIndicator>(Resource.Id.FullScreenImagePagerIndicator);

            if (imageIds.Length > 1)
            {
                indicator.SetViewPager(viewpager);
            }
            else
            {
                indicator.Visibility = ViewStates.Gone;
            }

            viewpager.SetCurrentItem(startingPos, false);

            return view;
        }
    }
}
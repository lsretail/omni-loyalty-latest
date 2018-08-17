using System;

using Android.OS;
using Android.Support.V4.View;
using Android.Views;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using Presentation.Activities.Base;
using Presentation.Util;

namespace Presentation.Activities.Image
{
    public class FullScreenImagePagerFragment : LoyaltyFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.FullScreenImagePagerScreen, null);

            var startingPos = Arguments.GetInt(BundleConstants.CurrentPage);
            var imageIds = Arguments.GetStringArray(BundleConstants.ItemListId);

            var viewpager = view.FindViewById<ViewPager>(Resource.Id.FullScreenImagePager);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                //viewpager.SetPageTransformer(true, new ZoomOutPageTransformer());
            }

            viewpager.Adapter = new FullScreenImagePagerAdapter(ChildFragmentManager, imageIds);

            var indicator = view.FindViewById<LinePageIndicator>(Resource.Id.FullScreenImagePagerIndicator);

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
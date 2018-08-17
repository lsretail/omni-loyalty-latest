using System.Collections.Generic;

using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using DK.Ostebaronen.Droid.ViewPagerIndicator;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Activities.Image
{
    public class DetailImagePager
    {
        private List<ImageView> imageViews;
        private ViewPager pager;

        public DetailImagePager(View container, FragmentManager fragmentManager, List<ImageView> imageViews)
        {
            this.imageViews = imageViews;

            if (imageViews.Count > 0)
            {
                var multipleImages = false;
                if (imageViews.Count > 1)
                {
                    multipleImages = true;
                }

                pager = container.FindViewById<ViewPager>(Resource.Id.DetailImagePager);
                var pagerIndicator = container.FindViewById<LinePageIndicator>(Resource.Id.DetailImagePagerIndicator);

                pager.PageMargin = (int)container.Resources.GetDimension(Resource.Dimension.BasePadding);

                pager.Adapter = new DetailImagePagerAdapter(fragmentManager, imageViews, pager.MeasuredWidth, pager.MeasuredHeight, multipleImages);
                
                pager.SetPageTransformer(false, new Utils.ViewPagerUtils.ZoomOutPageTransformer());

                pagerIndicator.SetViewPager(pager);

                if (multipleImages)
                {
                    pagerIndicator.Visibility = ViewStates.Visible;
                }
            }
        }

        public Android.Widget.ImageView GetFirstImage()
        {
            if (pager.ChildCount > 0)
            {
                var view = pager.GetChildAt(0);

                return view.FindViewById<Android.Widget.ImageView>(Resource.Id.DetailImageViewImage);
            }

            return null;
        }
    }
}
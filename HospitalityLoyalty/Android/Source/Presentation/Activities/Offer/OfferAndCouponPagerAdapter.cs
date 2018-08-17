using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Presentation.Activities.Offer
{
    class OfferAndCouponPagerAdapter: FragmentStatePagerAdapter 
    {
        private readonly Context context;

        public OfferAndCouponPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public OfferAndCouponPagerAdapter(FragmentManager fm, Context context)
            : base(fm)
        {
            this.context = context;
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Fragment GetItem(int position)
        {
            if(position == 0)
                return new CouponFragment();
            return new OfferFragment();
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            if (position == 0)
                return new Java.Lang.String(context.Resources.GetString(Resource.String.Coupons));
            return new Java.Lang.String(context.Resources.GetString(Resource.String.Offers));
        }
    }
}
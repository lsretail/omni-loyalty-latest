using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Coupons
{
    [Activity(Label = "", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden, Theme = "@style/ApplicationThemeFullyTransparent")]
    public class CouponDetailActivity : LoyaltyFragmentActivity
    {
        private const string CouponFragmentTag = "CouponFragmentTag";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.BaseSinglePanelLayout);

            if (bundle == null)
            {
                var details = CouponDetailFragment.NewInstance();
                details.Arguments = Intent.Extras;

                var fragmentCoupon = SupportFragmentManager.BeginTransaction();
                fragmentCoupon.Replace(Resource.Id.BasePanelLayoutContentFrameOne, details, CouponFragmentTag);
                fragmentCoupon.Commit();
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_clear_white_24dp);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        #region MENU

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.OfferDetailMenu, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case ResourceConstants.Home:
                    Finish();

                    return true;

                case Resource.Id.MenuViewGenerateQR:
                    var intent = new Intent();
                    intent.SetClass(this, typeof(QRCode.QRCodeActivity));

                    StartActivity(intent);

                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}
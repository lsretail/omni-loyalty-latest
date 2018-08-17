using System;

using Android.App;
using Android.OS;
using Android.Views;
using Presentation.Activities.Base;
using Presentation.Util;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.QRCode
{
    [Activity(Label = "@string/ActionbarQrCode")]
    public class QRCodeActivity : LoyaltyFragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (bundle == null)
            {
                var qrCode = new QrCodeFragment();

                var fragmentNotification = SupportFragmentManager.BeginTransaction();
                fragmentNotification.Add(ContentId, qrCode);
                fragmentNotification.Commit();
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_clear_white_24dp);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case ResourceConstants.Home:
                    Finish();

                    break;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}
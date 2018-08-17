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
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;
using Receiver = Presentation.Util.Receiver;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Offers
{
    [Activity(Label = "", ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class OfferDetailActivity : LoyaltyFragmentActivity, IBroadcastObserver
    {
        private const string OfferFragmentTag = "OfferFragmentTag";

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.BaseSinglePanelLayout);

            if (bundle == null)
            {
                var details = OfferDetailFragment.NewInstance();
                details.Arguments = Intent.Extras;

                var fragmentOffer = SupportFragmentManager.BeginTransaction();
                fragmentOffer.Add(Resource.Id.BasePanelLayoutContentFrameOne, details, OfferFragmentTag);
                fragmentOffer.Commit();
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_clear_white_24dp);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.OffersUpdated:
                case Utils.BroadcastUtils.DomainModelUpdated:
                    var couponFragment = SupportFragmentManager.FindFragmentByTag(OfferFragmentTag);

                    if (couponFragment is OfferDetailFragment)
                    {
                        (couponFragment as OfferDetailFragment).RefreshOffer();
                    }
                    break;
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            AddObserver(this);
        }

        protected override void OnPause()
        {
            RemoveObserver(this);
            base.OnPause();
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
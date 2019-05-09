using System;

using Android.OS;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Util;
using IBroadcastObserver = Presentation.Util.IBroadcastObserver;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Utils = Presentation.Util.Utils;

namespace Presentation.Activities.AccountTier
{
    public class AccountTierFragment : LoyaltyFragment, IRefreshableActivity, IBroadcastObserver
    {
        private GeneralSearchModel model;
        private TextView nextTier;
        private View nextTierHeader;

        private bool isBigScreen;
        private TextView currentpoints;

        public static AccountTierFragment NewInstance()
        {
            var accountTierDetail = new AccountTierFragment() { Arguments = new Bundle() };
            return accountTierDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            HasOptionsMenu = true;

            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);
            model = new GeneralSearchModel(Activity, this);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.AccountTier);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.AccountTierScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            currentpoints = view.FindViewById<TextView>(Resource.Id.AccountTierViewPointsBalance);
            var currentTier = view.FindViewById<TextView>(Resource.Id.AccountTierViewCurrentTier);
            nextTier = view.FindViewById<TextView>(Resource.Id.AccountTierViewNextTier);
            var nextTierPerks = view.FindViewById<TextView>(Resource.Id.AccountTierViewNextTierPerks);
            nextTierHeader = view.FindViewById(Resource.Id.AccountTierNextTierHeader);

            var qrCode = view.FindViewById<ImageView>(Resource.Id.AccountTierQrCode);
            qrCode.SetImageBitmap(Utils.QrCodeUtils.GenerateQRCode(Activity, false));

            var user = AppData.Device.UserLoggedOnToDevice;
            var userLoggedOn = user.Account;

            currentpoints.Text = String.Format(GetString(Resource.String.AccountTierViewPointsBalance), userLoggedOn.PointBalance.ToString("N0"));
            currentTier.Text = string.Format(GetString(Resource.String.AccountTierViewCurrentTier), userLoggedOn.Scheme.Description);
            nextTierPerks.Text = userLoggedOn.Scheme.Perks;

            decimal points = userLoggedOn.Scheme.PointsNeeded - userLoggedOn.PointBalance;
            if (points < 0)
                points = 0;

            nextTier.Text = string.Format(GetString(Resource.String.AccountTierViewNextTier),
                                          userLoggedOn.Scheme.PointsNeeded.ToString("N0"),
                                          points.ToString("N0"),
                                          userLoggedOn.Scheme.NextScheme.Description);
            return view;
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).AddObserver(this);
            }
            UpdatePointBalance();
        }

        public override void OnPause()
        {
            if (Activity is LoyaltyFragmentActivity)
            {
                (Activity as LoyaltyFragmentActivity).RemoveObserver(this);
            }
            base.OnPause();
        }

        public void ShowIndicator(bool show)
        {
        }

        private async void UpdatePointBalance()
        {
            await new MemberContactModel(Activity).MemberContactGetPointBalance(AppData.Device.UserLoggedOnToDevice.Card.Id);
        }

        public void BroadcastReceived(string action)
        {
            switch (action)
            {
                case Utils.BroadcastUtils.DomainModelUpdated:
                case Utils.BroadcastUtils.PointsUpdated:
                    currentpoints.Text = String.Format(GetString(Resource.String.AccountTierViewPointsBalance), AppData.Device.UserLoggedOnToDevice.Account.PointBalance.ToString("N0"));
                    break;
            }
        }
    }
}

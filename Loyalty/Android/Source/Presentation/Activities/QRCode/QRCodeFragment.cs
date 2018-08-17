using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.QRCode
{
    public class QrCodeFragment : LoyaltyFragment
    {
        private bool isBigScreen;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            View view;

            if (isBigScreen)
            {
                view = Inflate(inflater, Resource.Layout.QRCodeTabletScreen);
            }
            else
            {
                view = Inflate(inflater, Resource.Layout.QRCodeScreen);
            }

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.QrCodeScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            if (savedInstanceState == null)
            {
                var qrCodeHeader = QRCodeCodeFragment.NewInstance();
                var qrCodeList = QRCodeItemsFragment.NewInstance();

                var fragmentNotification = ChildFragmentManager.BeginTransaction();
                fragmentNotification.Add(Resource.Id.QrCodeScreenContentFrameOne, qrCodeHeader);
                fragmentNotification.Add(Resource.Id.QrCodeScreenContentFrameTwo, qrCodeList);
                fragmentNotification.Commit();
            }

            return view;
        }
    }
}
using System;

using Android.OS;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Util;

namespace Presentation.Activities.QRCode
{
    public class QRCodeCodeFragment : LoyaltyFragment
    {
        public static QRCodeCodeFragment NewInstance()
        {
            var welcomeDetail = new QRCodeCodeFragment() { Arguments = new Bundle() };
            return welcomeDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.QRCode);

            view.FindViewById<ImageView>(Resource.Id.QRCodeHeaderViewQRCode).SetImageBitmap(Utils.QrCodeUtils.GenerateQRCode(Activity));

            return view;
        }
    }
}
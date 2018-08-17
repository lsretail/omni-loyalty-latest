using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Models;
using Presentation.Utils;
using Presentation.Views;
using ZXing;
using ZXing.QrCode;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Checkout
{
    public class CheckoutFragment : BaseFragment, View.IOnClickListener, IRefreshableActivity
    {
        private BasketModel basketModel;
        private TransactionModel transactionModel;
        private OrderModel orderModel;

        private ImageView qrCodeImageView;
        private View qrCodeProgress;
        private ColoredButton retryButton;
        private ColoredButton doneButton;

        private string transactionId;

        private int userBrightness;
        private int userBrightnessMode;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.CheckoutScreen, null);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.CheckoutScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            basketModel = new BasketModel(Activity);
            transactionModel = new TransactionModel(Activity);
            orderModel = new OrderModel(Activity, this);

            retryButton = view.FindViewById<ColoredButton>(Resource.Id.CheckoutScreenRetry);
            doneButton = view.FindViewById<ColoredButton>(Resource.Id.CheckoutScreenDone);

            //view.FindViewById<ColoredButton>(Resource.Id.CheckoutScreenPayNow).SetOnClickListener(this);
            retryButton.SetOnClickListener(this);
            doneButton.SetOnClickListener(this);

            qrCodeImageView = view.FindViewById<ImageView>(Resource.Id.CheckoutScreenQrCode);
            qrCodeProgress = view.FindViewById<View>(Resource.Id.CheckoutScreenQrCodeLoading);

            if (Arguments != null && Arguments.ContainsKey(BundleUtils.TransactionId))
            {
                transactionId = Arguments.GetString(BundleUtils.TransactionId);

                qrCodeImageView.SetImageBitmap(GenerateQRCode(transactionId));

                ShowIndicator(false);
            }
            else
            {
                Order();
            }

            if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.WriteSettings) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(Activity, new[] { Manifest.Permission.WriteSettings }, 0);
            }

            return view;
        }

        private async void Order()
        {
            retryButton.Visibility = ViewStates.Gone;

           var guid = await  orderModel.OrderSave();

            if (string.IsNullOrEmpty(guid))
            {
                retryButton.Visibility = ViewStates.Visible;
            }
            else
            {
                retryButton.Visibility = ViewStates.Gone;
                doneButton.Visibility = ViewStates.Visible;
                qrCodeImageView.SetImageBitmap(GenerateQRCode(guid));
                AppData.Basket.Id = guid;
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.WriteSettings) == Permission.Granted)
            {
                SetBrightnessMax();
            }
        }

        public override void OnPause()
        {
            if (Android.Support.V4.App.ActivityCompat.CheckSelfPermission(Activity, Manifest.Permission.WriteSettings) == Permission.Granted)
            {
                ResetBrightness();
            }

            base.OnPause();
        }

        private void SetBrightnessMax()
        {
            //Get the content resolver
            var cResolver = Activity.ContentResolver;

            //Get the current window
            var window = Activity.Window;

            try
            {
                //Get the current system brightness
                userBrightnessMode = Settings.System.GetInt(cResolver, Settings.System.ScreenBrightnessMode);
                userBrightness = Settings.System.GetInt(cResolver, Settings.System.ScreenBrightness);
            }
            catch (Settings.SettingNotFoundException)
            {
                //Throw an error case it couldn't be retrieved
                LogUtils.Log("Cannot access system brightness");
            }


            // To handle the auto
            Settings.System.PutInt(cResolver, Settings.System.ScreenBrightnessMode, (int)Android.Provider.ScreenBrightness.ModeManual);

            //Set the system brightness using the brightness variable value
            Settings.System.PutInt(cResolver, Settings.System.ScreenBrightness, 255);
            //Get the current window attributes
            var layoutpars = window.Attributes;
            //Set the brightness of this window
            layoutpars.ScreenBrightness = 1;
            //Apply attribute changes to this window
            window.Attributes = layoutpars;
        }

        private void ResetBrightness()
        {
            //Get the content resolver
            var cResolver = Activity.ContentResolver;

            Settings.System.PutInt(cResolver, Settings.System.ScreenBrightnessMode, userBrightnessMode);

            //Set the system brightness using the brightness variable value
            Settings.System.PutInt(cResolver, Settings.System.ScreenBrightness, userBrightness);
        }

        private Bitmap GenerateQRCode(string id)
        {
            var code = string.Format(@"<mobilehosploy><id>{0}</id></mobilehosploy>", id);

            var height = 500;
            var width = 500;
            var writer = new QRCodeWriter();
            var matrix = writer.encode(code, BarcodeFormat.QR_CODE, width, height);

            int[] pixels = new int[width * height];
            // All are 0, or black, by default
            for (int y = 0; y < height; y++)
            {
                int offset = y * width;
                for (int x = 0; x < width; x++)
                {
                    pixels[offset + x] = matrix[x, y] ? Color.Black : Color.Transparent;            //WHITE ??
                }
            }

            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            bitmap.SetPixels(pixels, 0, width, 0, 0, width, height);

            return bitmap;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.CheckoutScreenDone:
                    if (string.IsNullOrEmpty(transactionId))
                    {
                        var transaction = transactionModel.CreateTransaction();

                        AppData.Transactions.Add(transaction);

                        doneButton.Enabled = false;
                        doneButton.SetText(Resource.String.Saving);

                        SyncTransactions();
                    }
                    else
                    {
                        GoHome();   
                    }
                    break;


                case Resource.Id.CheckoutScreenRetry:
                    Order();
                    break;
            }
        }

        private async void SyncTransactions()
        {
            var success = await transactionModel.SyncTransactions();

            if (success)
            {
                basketModel.ClearBasket(false);

                GoHome();
            }
            else
            {
                doneButton.Enabled = true;
                doneButton.SetText(Resource.String.Done);
            }
        }

        private void GoHome()
        {
            var upIntent = new Intent();
            upIntent.SetClass(Activity, typeof(HomeActivity));
            upIntent.AddFlags(ActivityFlags.ClearTop);
            upIntent.AddFlags(ActivityFlags.SingleTop);
            upIntent.PutExtra(BundleUtils.ChosenMenuBundleName, (int)BundleUtils.ChosenMenu.DefaultMenu);

            StartActivity(upIntent);

            Activity.Finish();
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                qrCodeImageView.Visibility = ViewStates.Gone;
                qrCodeProgress.Visibility = ViewStates.Visible;
            }
            else
            {
                qrCodeProgress.Visibility = ViewStates.Gone;
                qrCodeImageView.Visibility = ViewStates.Visible;
            }
        }
    }
}
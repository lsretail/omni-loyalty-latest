using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Activities.Base;
using Presentation.Activities.Home;
using Presentation.Activities.Offer;
using Presentation.Models;
using Presentation.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Contact
{
    public class ContactFragment : BaseFragment, View.IOnClickListener, SwipeRefreshLayout.IOnRefreshListener, IRefreshableActivity, IBroadcastObserver
    {
        private SwipeRefreshLayout contactRefreshContainer;
        private TextView points;
        private ImageView qrCode;

        private TextView name;
        private TextView email;
        private TextView alternateId;

        private ContactModel contactModel;
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Inflate(inflater, Resource.Layout.ContactScreen, null);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.ContactScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            contactModel = new ContactModel(Activity, this);

            name = view.FindViewById<TextView>(Resource.Id.ContactScreenName);
            email = view.FindViewById<TextView>(Resource.Id.ContactScreenEmail);

            alternateId = view.FindViewById<TextView>(Resource.Id.ContactScreenAlternateId);
            
            contactRefreshContainer = view.FindViewById<SwipeRefreshLayout>(Resource.Id.ContactScreenRefreshContainer);

            contactRefreshContainer.SetColorSchemeResources(Resource.Color.accent);
            contactRefreshContainer.SetOnRefreshListener(this);

            if (!AppData.HasPoints)
            {
                contactRefreshContainer.Enabled = false;
            }
            
            points = view.FindViewById<TextView>(Resource.Id.ContactScreenPoints);

            var couponButton = view.FindViewById<View>(Resource.Id.ContactScreenCoupons);
            var updateButton = view.FindViewById<View>(Resource.Id.ContactScreenUpdate);
            var changePasswordButton = view.FindViewById<View>(Resource.Id.ContactScreenChangePassword);

            couponButton.SetOnClickListener(this);
            updateButton.SetOnClickListener(this);
            changePasswordButton.SetOnClickListener(this);

            if (!AppData.HasOffersAndCoupons)
            {
                couponButton.Visibility = ViewStates.Gone;
            }

            qrCode = view.FindViewById<ImageView>(Resource.Id.ContactScreenQrCode);

            qrCode.Visibility = ViewStates.Gone;

            return view;
        }

        private void SetPoints()
        {
            if (AppData.HasPoints && AppData.Contact != null && AppData.Contact.Account != null)
            {
                points.Text = string.Format(Resources.GetString(Resource.String.ContactYouHavePoints), AppData.Contact.Account.PointBalance.ToString("n0"));
            }
            else
            {
                points.Visibility = ViewStates.Gone;
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            SetPoints();

            name.Text = AppData.Contact.FirstName;
            email.Text = AppData.Contact.Email;

            if (string.IsNullOrEmpty(AppData.Contact.AlternateId))
            {
                alternateId.Visibility = ViewStates.Gone;
            }
            else
            {
                alternateId.Text = AppData.Contact.AlternateId;
            }

            BroadcastUtils.SendBroadcast(Activity, BroadcastUtils.ContactUpdated);

            var basketQrCode = new BasketQrCode(AppData.MobileMenu)
            {
                Contact = AppData.Contact,
            };

            if (Activity is HospActivity)
            {
                (Activity as HospActivity).AddObserver(this);
            }

            qrCode.SetImageBitmap(Utils.Utils.GenerateQrCode(basketQrCode.Serialize()));
        }

        public override void OnPause()
        {
            if (Activity is HospActivity)
            {
                (Activity as HospActivity).RemoveObserver(this);
            }

            base.OnPause();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.ContactScreenCoupons:
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(OfferAndCouponActivity));

                    ActivityUtils.StartActivityWithAnimation(Activity, intent, v);
                    break;

                case Resource.Id.ContactScreenUpdate:
                    var updateIntent = new Intent();
                    updateIntent.SetClass(Activity, typeof(UpdateContactActivity));

                    ActivityUtils.StartActivityWithAnimation(Activity, updateIntent, v);
                    break;

                case Resource.Id.ContactScreenChangePassword:
                    var changePasswordIntent = new Intent();
                    changePasswordIntent.SetClass(Activity, typeof(ChangePasswordActivity));

                    ActivityUtils.StartActivityWithAnimation(Activity, changePasswordIntent, v);
                    break;
            }
        }

        public void OnRefresh()
        {
            contactModel.ContactGetPointBalance(AppData.Contact.Id);
        }

        public void ShowIndicator(bool show)
        {
            contactRefreshContainer.Refreshing = show;

            if (Activity is HospActivity)
            {
                if (show)
                {
                    (Activity as HospActivity).ShowLoadingMessage(Resources.GetString(Resource.String.ContactRefreshingPoints));
                }
                else
                {
                    (Activity as HospActivity).RemoveLoadingMessage();
                }
            }
        }

        public void BroadcastReceived(string action)
        {
            if (action == BroadcastUtils.ContactUpdated || action == BroadcastUtils.ContactPointsUpdated)
            {
                SetPoints();
            }
        }
    }
}
using System;

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using Presentation.Activities.Base;
using Presentation.Adapters;
using Presentation.Dialogs;
using Presentation.Models;
using Presentation.Util;
using LSRetail.Omni.Domain.DataModel.Base;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Activities.Contact
{
    public class ContactUsFragment : LoyaltyFragment, IRefreshableActivity
    {
        private GeneralSearchModel searchModel;
        private ViewSwitcher switcher;
        private ListView headers;
        private View progressView;
        private ContactUs contactUs;

        public static ContactUsFragment NewInstance()
        {
            var detail = new ContactUsFragment() { Arguments = new Bundle() };
            return detail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Utils.ViewUtils.Inflate(inflater, Resource.Layout.ContactUs);
            
            var toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.ContactUsScreenToolbar);
            (Activity as LoyaltyFragmentActivity).SetSupportActionBar(toolbar);

            switcher = view.FindViewById<ViewSwitcher>(Resource.Id.ContactUsViewSwitcher);
            headers = view.FindViewById<ListView>(Resource.Id.ContactUsViewHeaders);
            progressView = view.FindViewById<View>(Resource.Id.ContactUsViewLoadingSpinner);

            searchModel = new GeneralSearchModel(Activity, this);

            LoadAppsettings();

            return view;
        }

        private async void LoadAppsettings()
        {
            var results = await searchModel.AppSettings(ConfigKey.Password_Policy);
            AppSettingsSuccess(results);
        }

        private void AppSettingsSuccess(string contactUsHtml)
        {
            contactUs = new ContactUs(contactUsHtml);


            var header = string.Empty;
            foreach (var headerLine in contactUs.HeaderLines)
            {
                header += headerLine + System.Environment.NewLine;
            }

            var footer = string.Empty;
            foreach (var footerLine in contactUs.FooterLines)
            {
                footer += footerLine + System.Environment.NewLine;
            }

            if (!string.IsNullOrEmpty(header))
            {
                var headerView = Util.Utils.ViewUtils.Inflate(Activity.LayoutInflater, Resource.Layout.ContactUsListHeaderItems);
                headerView.FindViewById<TextView>(Resource.Id.ContactUsViewSubTitle).Text = header.TrimEnd(System.Environment.NewLine.ToCharArray());
                headers.AddHeaderView(headerView, null, false);
            }

            if (!string.IsNullOrEmpty(footer))
            {
                var footerView = Util.Utils.ViewUtils.Inflate(Activity.LayoutInflater, Resource.Layout.ContactUsListFooterItems);
                footerView.FindViewById<TextView>(Resource.Id.ContactUsViewSubTitle).Text = footer.TrimEnd(System.Environment.NewLine.ToCharArray());
                headers.AddHeaderView(footerView, null, false);
            }

            headers.Adapter = new ContactUsAdapter(Activity, contactUs.ContactLines, OnActionItemPressed);
        }

        private void OnActionItemPressed(ContactUs.ContactLine contactLine)
        {
            try
            {
                if (contactLine.Type == ContactUs.ContactLine.ContactType.Phone)
                {
                    Intent callIntent = new Intent(Intent.ActionDial);
                    callIntent.SetData(Android.Net.Uri.Parse("tel:" + contactLine.Value));
                    Activity.StartActivity(callIntent);
                }
                else if (contactLine.Type == ContactUs.ContactLine.ContactType.Email)
                {
                    Intent intent = new Intent(Intent.ActionSend);
                    intent.SetType("message/rfc822");
                    intent.PutExtra(Intent.ExtraEmail  , new String[]{contactLine.Value});
                    Activity.StartActivity(Intent.CreateChooser(intent, "Send mail..."));
                }
                else if (contactLine.Type == ContactUs.ContactLine.ContactType.Web)
                {
                    if (!contactLine.Value.StartsWith("http://") && !contactLine.Value.StartsWith("https://"))
                        contactLine.Value = "http://" + contactLine.Value;
                    Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(contactLine.Value));
                    Activity.StartActivity(browserIntent);
                }
            }
            catch (Exception)
            {
                var warningDialog = new WarningDialog(Activity, "");
                warningDialog.Message = GetString(Resource.String.ContactUsViewOpenAppError);
                warningDialog.Show();
            }
        }

        public void ShowIndicator(bool show)
        {
            if (show)
            {
                if (switcher.CurrentView != progressView)
                {
                    switcher.ShowPrevious();
                }
            }
            else
            {
                if (switcher.CurrentView != headers)
                {
                    switcher.ShowNext();
                }
            }
        }
    }
}
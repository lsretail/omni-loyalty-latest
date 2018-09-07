using System;
using System.Collections.Generic;

using Android.OS;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Models;
using Presentation.Util;
using ColoredButton = Presentation.Views.ColoredButton;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Activities.Account
{
    public class AccountProfilesFragment : LoyaltyFragment, IRefreshableActivity, View.IOnClickListener
    {
        private ListView headers;
        private View loadingView;
        private View contentView;
        private ViewSwitcher switcher;

        private ProfileModel model;
        private List<Profile> profiles;
        private bool isBigScreen;

        public bool LoadAccountProfiles { get; set; }
        public Action OnCancelPressed { get; set; }
        public Action OnCreatePressed { get; set; }

        public static AccountProfilesFragment NewInstance()
        {
            var accountProfilesDetail = new AccountProfilesFragment() { Arguments = new Bundle() };
            return accountProfilesDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            model = new ProfileModel(Activity, this);

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.AccountProfile);

            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            switcher = view.FindViewById<ViewSwitcher>(Resource.Id.AccountProfileViewSwitcher);
            loadingView = view.FindViewById(Resource.Id.AccountProfileViewLoadingSpinner);
            contentView = view.FindViewById(Resource.Id.AccountProfileViewContentView);
            headers = view.FindViewById<ListView>(Resource.Id.AccountProfileViewList);

            var cancel = view.FindViewById<ColoredButton>(Resource.Id.AccountProfilesCancel);
            var create = view.FindViewById<ColoredButton>(Resource.Id.AccountProfilesCreate);

            if (isBigScreen)
            {
                cancel.Visibility = ViewStates.Gone;
            }
            else
            {
                cancel.SetOnClickListener(this);
            }

            if (LoadAccountProfiles)
            {
                create.SetText(Resource.String.AccountViewUpdate);
            }
            else
            {
                create.SetText(Resource.String.AccountViewCreate);
            }

            create.SetOnClickListener(this);

            headers.EmptyView = view.FindViewById(Resource.Id.AccountProfileViewEmptyView);
            headers.ChoiceMode = ChoiceMode.Multiple;

            view.FindViewById<Button>(Resource.Id.AccountProfileViewEmptyViewRetry).SetOnClickListener(this);

            return view;
        }

        public override void OnDestroy()
        {
            model.Stop();

            base.OnDestroy();
        }

        public override void OnActivityCreated(Bundle p0)
        {
            base.OnActivityCreated(p0);

            LoadProfilesFromWs();
        }

        private async void LoadProfilesFromWs()
        {
            if (profiles == null || profiles.Count == 0)
            {
                if (LoadAccountProfiles)
                {
                    profiles = await model.ProfilesGetByContactId(AppData.Device.UserLoggedOnToDevice.Id);
                }
                else
                {
                    profiles = await model.ProfilesGetAll();
                }

                LoadProfiles();
            }
            else
            {
                LoadProfiles();
            }
        }

        private void LoadProfiles()
        {
            if (profiles == null)
            {
                profiles = new List<Profile>();
            }

            var profileDescriptions = new List<string>();
            profiles.ForEach(x => profileDescriptions.Add(x.Description));

            var headerArrayAdapter = new ArrayAdapter<String>(Activity, Resource.Layout.CustomListMultipleChoice, profileDescriptions);
            headers.Adapter = headerArrayAdapter;

            if (LoadAccountProfiles)
            {
                AppData.Device.UserLoggedOnToDevice.Profiles = profiles;

                for (int i = 0; i < profiles.Count; i++)
                {
                    headers.SetItemChecked(i, profiles[i].ContactValue);
                }
                LoadAccountProfiles = false;
            }

            ShowIndicator(false);
        }

        public List<Profile> Profiles
        {
            get
            {
                var selectedList = headers.CheckedItemPositions;
                for (int i = 0; i < profiles.Count; i++)
                {
                    profiles[i].ContactValue = (selectedList.Get(i)) ? true : false;
                }
                return profiles;
            }
        }

        private void ShowLoading()
        {
            if (switcher.CurrentView != loadingView)
                switcher.ShowPrevious();
        }

        private void ShowContent()
        {
            if (switcher.CurrentView != contentView)
                switcher.ShowNext();
        }

        public void ShowIndicator(bool show)
        {
            if (show)
                ShowLoading();
            else
                ShowContent();
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.AccountProfileViewEmptyViewRetry:
                    LoadProfilesFromWs();
                    break;

                case Resource.Id.AccountProfilesCreate:
                    if (OnCreatePressed != null)
                    {
                        OnCreatePressed();
                    }
                    break;

                case Resource.Id.AccountProfilesCancel:
                    if (OnCancelPressed != null)
                    {
                        OnCancelPressed();
                    }
                    break;
            }
        }
    }
}
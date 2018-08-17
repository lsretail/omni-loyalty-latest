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
using Presentation.Activities.Home;
using Presentation.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Welcome
{
    public class WelcomeFragment : BaseFragment, View.IOnClickListener
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.WelcomeScreen);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.WelcomeScreenToolbar);
            (Activity as HospActivity).SetSupportActionBar(toolbar);

            var nextButton = view.FindViewById(Resource.Id.WelcomeViewNext);
            nextButton.SetOnClickListener(this);

            return view;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.WelcomeViewNext:
                    PreferenceUtils.SetBool(Activity, PreferenceUtils.WelcomeHasBeenShown, true);
                    
                    var intent = new Intent();
                    intent.SetClass(Activity, typeof(HomeActivity));

                    StartActivity(intent);

                    Activity.Finish();
                    break;
            }
        }
    }
}
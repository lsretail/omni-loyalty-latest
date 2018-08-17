using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Presentation.Activities.Login
{
    public class LoginPagerAdapter : FragmentStatePagerAdapter
    {
        private Context context;

        public LoginPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LoginPagerAdapter(FragmentManager fm, Context context) : base(fm)
        {
            this.context = context;
        }

        public override int Count
        {
            get { return 2; }
        }

        public override Fragment GetItem(int position)
        {
            if(position == 0)
                return new SignInFragment();
            return new SignUpFragment();
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            if (position == 0)
                return new Java.Lang.String(context.Resources.GetString(Resource.String.ActionBarLogin));
            return new Java.Lang.String(context.Resources.GetString(Resource.String.LoginSignUp));
        }
    }
}
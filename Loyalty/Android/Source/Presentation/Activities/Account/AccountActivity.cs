
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;

using Presentation.Activities.Base;

using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;

namespace Presentation.Activities.Account
{
    [Activity(ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden)]
    public class AccountActivity : LoyaltyFragmentActivity//, View.IOnClickListener, Android.Support.V7.App.ActionBar.ITabListener
    {
        private const string AccountFragmentTag = "AccountFragmentTag";

        protected override void OnCreate(Bundle bundle)
        {
            HasMenu = false;
            HasRightDrawer = false;
            HasLeftDrawer = false;
            
            base.OnCreate(bundle);
            
            if (bundle == null)
            {
                var details = new AccountFragment();
                details.Arguments = Intent.Extras;

                var fragmentCoupon = SupportFragmentManager.BeginTransaction();
                fragmentCoupon.Add(ContentId, details, AccountFragmentTag);
                fragmentCoupon.Commit();
            }
        }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            base.SetSupportActionBar(toolbar);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
        }

        public override void OnBackPressed()
        {
            var fragment = SupportFragmentManager.FindFragmentByTag(AccountFragmentTag);

            if (fragment is AccountFragment)
            {
                var accountFragment = fragment as AccountFragment;

                if (accountFragment.CurrentPage == AccountFragment.ShownPage.Profile)
                {
                    accountFragment.Display();
                }
                else
                {
                    SetResult(Result.Canceled);
                    Finish();
                }
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    SetResult(Result.Canceled);
                    Finish();
                    return true;
            }

            return false;
        }
    }
}
using System;

using Android.App;
using Android.OS;
using Android.Views;

namespace Presentation.Activities.Base
{
    public abstract class CardListFragment : LoyaltyFragment
    {
        protected bool IsBigScreen { get; set; }
        protected bool ShowAsList { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            HasOptionsMenu = true;

            IsBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            if (IsBigScreen)
            {
                ShowAsList = false;
            }
            else
            {
                ShowAsList = Util.Utils.PreferenceUtils.GetBool(Activity, Util.Utils.PreferenceUtils.ShowListAsList);
            }

            var view = CreateView(inflater);

            return view;
        }

        public abstract View CreateView(LayoutInflater inflater);

        #region MENU

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            if (!IsBigScreen)
            {
                inflater.Inflate(Resource.Menu.ShowListAsMenu, menu);

                var showAsListMenuItem = menu.FindItem(Resource.Id.MenuViewShowListAs);

                if (ShowAsList)
                {
                    showAsListMenuItem.SetTitle(Resource.String.MenuViewShowAsGrid);
                    showAsListMenuItem.SetIcon(Resource.Drawable.ic_view_module_white_24dp);
                }
                else
                {
                    showAsListMenuItem.SetTitle(Resource.String.MenuViewShowAsList);
                    showAsListMenuItem.SetIcon(Resource.Drawable.ic_view_list_white_24dp);
                }
            }

            base.OnCreateOptionsMenu(menu, inflater);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.MenuViewShowListAs:
                    ShowAsList = !ShowAsList;
                    Util.Utils.PreferenceUtils.SetBool(Activity, Util.Utils.PreferenceUtils.ShowListAsList, ShowAsList);

                    Activity.InvalidateOptionsMenu();

                    FragmentManager.BeginTransaction().Detach(this).Attach(this).Commit();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}
using System;

using Android.App;
using Android.OS;
using Android.Views;

namespace Presentation.Activities.Base
{
    public abstract class MasterDetailCardListFragment : MasterDetailFragment
    {
        private bool canChangeDisplayMode = true;

        protected bool ShowAsList { get; set; }
        protected bool CanChangeDisplayMode
        {
            get { return canChangeDisplayMode; }
            set { canChangeDisplayMode = value; }
        }


        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            ShowAsList = Util.Utils.PreferenceUtils.GetBool(Activity, Util.Utils.PreferenceUtils.ShowListAsList);

            return base.OnCreateView(inflater, container, savedInstanceState);
        }
        
        public override View CreateView(LayoutInflater inflater)
        {
            View view;

            if (isBigScreen)
            {
                if (ShowAsList)
                {
                    view = Inflate(inflater, Resource.Layout.BaseDoublePanelLayout);
                }
                else
                {
                    view = Inflate(inflater, Resource.Layout.BaseBigScreenCardListLayout);
                }
            }
            else
            {
                view = Inflate(inflater, Resource.Layout.BaseSinglePanelLayout);
            }

            return view;
        }

        #region MENU

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            if (canChangeDisplayMode)
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

                    Activity.SupportInvalidateOptionsMenu();

                    var headers = ChildFragmentManager.FindFragmentByTag(MasterLayoutFragmentTag);
                    var detail = ChildFragmentManager.FindFragmentByTag(DetailLayoutFragmentTag);

                    var ft = ChildFragmentManager.BeginTransaction();
                    if (headers != null)
                    {
                        ft.Remove(headers);
                    }
                    if (detail != null)
                    {
                        ft.Remove(detail);
                    }
                    ft.Commit();

                    FragmentManager.BeginTransaction().Detach(this).Attach(this).Commit();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
    }
}
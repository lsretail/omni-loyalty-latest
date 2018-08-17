using System;

using Android.OS;
using Android.Views;
using Presentation.Util;

namespace Presentation.Activities.Base
{
    public abstract class MasterDetailFragment : LoyaltyFragment, IItemClickListener
    {
        protected const string MasterLayoutFragmentTag = "MasterLayoutFragmentTag";
        protected const string DetailLayoutFragmentTag = "DetailLayoutFragmentTag";

        protected const int MasterLayoutId = Resource.Id.BasePanelLayoutContentFrameOne;
        protected const int DetailLayoutId = Resource.Id.BasePanelLayoutContentFrameTwo;

        protected bool isBigScreen;
        
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);

            var view = CreateView(inflater);

            LoadMasterScreen(savedInstanceState);

            return view;
        }

        public virtual View CreateView(LayoutInflater inflater)
        {
            View view = null;
            if (isBigScreen)
            {
                view = Inflate(inflater, Resource.Layout.BaseDoublePanelLayout);
            }
            else
            {
                view = Inflate(inflater, Resource.Layout.BaseSinglePanelLayout);
            }

            return view;
        }

        public abstract void LoadMasterScreen(Bundle savedInstanceState);

        public abstract void ItemClicked(int type, string id, string id2, View view);
    }
}
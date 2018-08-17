using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace Presentation.Activities.Favorite
{
    public class FavoritePagerAdapter : FragmentStatePagerAdapter
    {
        private readonly Context context;

        public FavoritePagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public FavoritePagerAdapter(FragmentManager fm, Context context)
            : base(fm)
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
                return new FavoriteItemFragment();
            return new FavoriteTransactionFragment();
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            if (position == 0)
                return new Java.Lang.String(context.Resources.GetString(Resource.String.FavoriteItems));
            return new Java.Lang.String(context.Resources.GetString(Resource.String.FavoriteTransactions));
        }
    }
}
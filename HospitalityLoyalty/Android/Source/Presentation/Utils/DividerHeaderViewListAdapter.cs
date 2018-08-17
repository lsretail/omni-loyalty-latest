using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Presentation.Utils
{
    public class DividerHeaderViewListAdapter : HeaderViewListAdapter
    {
        protected DividerHeaderViewListAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public DividerHeaderViewListAdapter(IList<ListView.FixedViewInfo> headerViewInfos, IList<ListView.FixedViewInfo> footerViewInfos, IListAdapter adapter) : base(headerViewInfos, footerViewInfos, adapter)
        {
        }

        public override bool AreAllItemsEnabled()
        {
            return true;
        }
    }
}
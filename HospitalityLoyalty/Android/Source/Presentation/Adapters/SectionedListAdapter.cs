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

namespace Presentation.Adapters
{
    public abstract class SectionedListAdapter : BaseAdapter<SectionedListAdapter.SectionedListItem>
    {
        protected Context Context;
        protected List<SectionedListItem> Items = new List<SectionedListItem>();

        public SectionedListAdapter(Context context)
        {
            this.Context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return Items.Count; }
        }

        public override SectionedListItem this[int position]
        {
            get { return Items[position]; }
        }

        public override int GetItemViewType(int position)
        {
            if (this[position] is SectionedListHeaderItem)
            {
                return 0;
            }

            return 1;
        }

        public override bool AreAllItemsEnabled()
        {
            return true;
        }

        public override bool IsEnabled(int position)
        {
            if (this[position] is SectionedListHeaderItem)
            {
                return false;
            }

            return true;
        }

        public override int ViewTypeCount
        {
            get { return 2; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this[position];
            
            if (item is SectionedListHeaderItem)
            {
                return GetHeaderView(position, convertView, item);
            }

            return GetLineView(position, convertView, item);
        }

        public virtual View GetHeaderView(int position, View convertView, SectionedListItem item)
        {
            var header = item as SectionedListHeaderItem;

            if (convertView == null)
            {
                var inflater = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService));
                convertView = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.ListViewSectionHeader);
            }

            convertView.FindViewById<TextView>(Resource.Id.ListViewSectionHeaderDescription).Text = header.Description.ToUpper();

            return convertView;
        }

        public abstract View GetLineView(int position, View convertView, SectionedListItem item);

        public class SectionedListItem
        {
            public SectionedListItem()
            {
            }
        }

        public class SectionedListHeaderItem : SectionedListItem
        {
            public string Description { get; set; }

            public SectionedListHeaderItem()
            {
                Description = string.Empty;
            }
        }

        public class SectionListLineItem : SectionedListItem
        {
            public SectionListLineItem()
            {
            }
        }
    }
}
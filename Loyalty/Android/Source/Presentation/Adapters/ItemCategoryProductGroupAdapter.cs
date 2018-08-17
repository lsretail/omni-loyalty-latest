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
using Presentation.Util;

namespace Presentation.Adapters
{
    public class ItemCategoryProductGroupAdapter : BaseExpandableListAdapter, View.IOnTouchListener
    {
        private Context context;
        private readonly List<ExpandableListUtil.JavaGroup> objects;
        private ImageLoader loader = new ImageLoader();

        public ItemCategoryProductGroupAdapter(Context context, List<ExpandableListUtil.JavaGroup> objects)
        {
            this.context = context;
            this.objects = objects;
        }

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
        {
            return objects[groupPosition].Items[childPosition];
        }

        public override long GetChildId(int groupPosition, int childPosition)
        {
            return ExpandableListView.GetPackedPositionForChild(groupPosition, childPosition);
        }

        public override int GetChildrenCount(int groupPosition)
        {
            return objects[groupPosition].Items.Count;
        }

        public override Java.Lang.Object GetGroup(int groupPosition)
        {
            return objects[groupPosition];
        }

        public override long GetGroupId(int groupPosition)
        {
            return ExpandableListView.GetPackedPositionForGroup(groupPosition);
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition)
        {
            return false;
        }

        public override int GroupCount
        {
            get { return objects.Count; }
        }

        public override bool HasStableIds
        {
            get { return true; }
        }

        public override int GetChildType(int groupPosition, int childPosition)
        {
            return objects[groupPosition].GroupType == ExpandableListUtil.GroupType.Offer ? 0 : 1;
        }

        public override int ChildTypeCount
        {
            get { return 2; }
        }

        public override bool AreAllItemsEnabled()
        {
            return true;
        }

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                LayoutInflater layoutInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = layoutInflater.Inflate(Resource.Layout.ExpandableListGroupHeader, null);
            }

            convertView.FindViewById<TextView>(Resource.Id.ExpandableListGroupHeaderHeader).Text = objects[groupPosition].Name;

            convertView.SetOnTouchListener(this);

            return convertView;
        }

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.SimpleListItem, null);
            }

            (convertView as TextView).Text = objects[groupPosition].Items[childPosition].Value as string;
            
            return convertView;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            return false;
        }
    }
}
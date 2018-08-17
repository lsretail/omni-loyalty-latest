using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Presentation.Utils;

namespace Presentation.Adapters
{
    public class DrawerMenuItemAdapter : BaseRecyclerAdapter
    {
        private readonly List<DrawerMenuItem> drawerMenuItems;
        private Action<DrawerMenuItem> onItemClicked;

        public DrawerMenuItemAdapter(List<DrawerMenuItem> drawerMenuItems, Action<DrawerMenuItem> onItemClicked)
        {
            this.drawerMenuItems = drawerMenuItems;
            this.onItemClicked = onItemClicked;
        }

        public override int ItemCount
        {
            get { return drawerMenuItems.Count; }
        }

        public override int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var drawerItemHolder = holder as DrawerItemViewHolder;
            var drawerItem = drawerMenuItems[position];

            if (drawerItemHolder == null || drawerItem == null)
            {
                return;
            }

            drawerItemHolder.Title.Text = drawerItem.Title;

            if (string.IsNullOrEmpty(drawerItem.SubTitle))
            {
                drawerItemHolder.Subtitle.Visibility = ViewStates.Gone;
            }
            else
            {
                drawerItemHolder.Subtitle.Text = drawerItem.SubTitle;
                drawerItemHolder.Subtitle.Visibility = ViewStates.Visible;
            }

            if (drawerItem.Color)
                drawerItemHolder.Title.SetTextColor(new Color(ContextCompat.GetColor(drawerItemHolder.Title.Context, Resource.Color.accent)));
            else
                drawerItemHolder.Title.SetTextColor(new Color(ContextCompat.GetColor(drawerItemHolder.Title.Context, Resource.Color.black87)));

            drawerItemHolder.Image.SetImageResource(drawerItem.Image);
            drawerItemHolder.Image.SetColorFilter(Utils.Utils.GetColorFilter(new Color(ContextCompat.GetColor(drawerItemHolder.Image.Context, Resource.Color.accent))));

            if (drawerItem.Background.HasValue)
            {
                drawerItemHolder.ItemView.SetBackgroundColor(drawerItem.Background.Value);
            }
            else
            {
                drawerItemHolder.ItemView.SetBackgroundResource(Resource.Color.transparent);
            }

            if (drawerItem.IsLoading)
            {
                drawerItemHolder.Progress.Visibility = ViewStates.Visible;
            }
            else
            {
                drawerItemHolder.Progress.Visibility = ViewStates.Gone;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = Utils.Utils.ViewUtils.Inflate(LayoutInflater.From(parent.Context), Resource.Layout.DrawerMenuListItem, parent);

            var vh = new DrawerItemViewHolder(
                view,
                (type, pos) =>
                {
                    var item = drawerMenuItems[pos];

                    onItemClicked(item);
                });

            return vh;
        }

        private enum ItemClickedType
        {
            Item
        }

        private class DrawerItemViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private readonly Action<ItemClickedType, int> itemClicked;

            public TextView Title { get; set; }
            public TextView Subtitle { get; set; }
            public ProgressBar Progress { get; set; }
            public ImageView Image { get; set; }

            public DrawerItemViewHolder(IntPtr javaReference, JniHandleOwnership transfer)
                : base(javaReference, transfer)
            {
            }

            public DrawerItemViewHolder(View view, Action<ItemClickedType, int> itemClicked)
                : base(view)
            {
                this.itemClicked = itemClicked;
            
                Title = view.FindViewById<TextView>(Resource.Id.DrawerMenuListItemTitle);
                Subtitle = view.FindViewById<TextView>(Resource.Id.DrawerMenuListItemSubTitle);
                Progress = view.FindViewById<ProgressBar>(Resource.Id.DrawerMenuListItemLoadingSpinner);
                Image = view.FindViewById<ImageView>(Resource.Id.DrawerMenuListItemImage);

                view.SetOnClickListener(this);
            }

            public void OnClick(View v)
            {
                itemClicked(ItemClickedType.Item, AdapterPosition);
            }
        }
    }
}
using System;
using System.Collections.Generic;

using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Presentation.Util;

namespace Presentation.Adapters
{
    public class DrawerMenuItemAdapter : BaseAdapter<DrawerMenuItem>, View.IOnClickListener
    {
        private readonly Context context;
        private readonly List<DrawerMenuItem> drawerMenuItems;

        public DrawerMenuItemAdapter(Context context, List<DrawerMenuItem> drawerMenuItems)
        {
            this.context = context;
            this.drawerMenuItems = drawerMenuItems;
        }

        public DrawerMenuItemAdapter(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return drawerMenuItems.Count; }
        }

        public override int ViewTypeCount
        {
            get { return 3; }
        }

        public override int GetItemViewType(int position)
        {
            var item = this[position];

            if (item is SecondaryActionDrawerMenuItem)
            {
                return 0;
            }
            if (item is SecondaryTextDrawerMenuItem)
            {
                return 1;
            }
            return 2;
        }

        public override DrawerMenuItem this[int position]
        {
            get { return drawerMenuItems[position]; }
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        public override bool IsEnabled(int position)
        {
            return this[position].Enabled;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = this[position];

            if (convertView == null) // no view to re-use, create new
            {
                if (item is SecondaryActionDrawerMenuItem)
                {
                    var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                    convertView = inflater.Inflate(Resource.Layout.SecondaryActionDrawerMenuListItem, null);
                }
                else if (item is SecondaryTextDrawerMenuItem)
                {
                    var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                    convertView = inflater.Inflate(Resource.Layout.SecondaryTextDrawerMenuListItem, null);
                }
                else
                {
                    var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                    convertView = inflater.Inflate(Resource.Layout.DrawerMenuListItem, null);
                }
            }

            var title = convertView.FindViewById<TextView>(Resource.Id.DrawerMenuListItemTitle);
            var subTitle = convertView.FindViewById<TextView>(Resource.Id.DrawerMenuListItemSubTitle);
            //var progressIndicator = convertView.FindViewById<ProgressBar>(Resource.Id.DrawerMenuListItemLoadingSpinner);

            title.Text = item.Title;

            if (string.IsNullOrEmpty(item.SubTitle))
            {
                subTitle.Visibility = ViewStates.Gone;
            }
            else
            {
                subTitle.Text = item.SubTitle;
                subTitle.Visibility = ViewStates.Visible;
            }

            convertView.FindViewById<ImageView>(Resource.Id.DrawerMenuListItemImage).SetImageResource(item.Image);

            if (item.Accent.HasValue)
            {
                if (item.Color)
                    title.SetTextColor(item.Accent.Value);
                else
                    title.SetTextColor(new Color(ContextCompat.GetColor(context, Resource.Color.black87)));

                convertView.FindViewById<ImageView>(Resource.Id.DrawerMenuListItemImage).SetColorFilter(Utils.ImageUtils.GetColorFilter(item.Accent.Value));
            }

            if (item.Background.HasValue)
            {
                convertView.SetBackgroundColor(item.Background.Value);
            }
            else
            {
                convertView.SetBackgroundResource(Resource.Color.transparent);
            }

            if (item is SecondaryTextDrawerMenuItem)
            {
                var secondaryText = convertView.FindViewById<TextView>(Resource.Id.DrawerMenuListItemSecondaryText);

                var background = secondaryText.Background as GradientDrawable;

                if (item.Accent.HasValue)
                {
                    background.SetColor(item.Accent.Value);
                }
                else
                {
                    background.SetColor(new Color(ContextCompat.GetColor(context, Resource.Color.transparent)));
                }

                if (string.IsNullOrEmpty((item as SecondaryTextDrawerMenuItem).SecondaryText))
                {
                    secondaryText.Visibility = ViewStates.Gone;
                }
                else
                {
                    secondaryText.Text = (item as SecondaryTextDrawerMenuItem).SecondaryText;
                    secondaryText.Visibility = ViewStates.Visible;
                }
            }
            
            if (item is SecondaryActionDrawerMenuItem)
            {
                var secondaryActionItem = item as SecondaryActionDrawerMenuItem;

                var secondaryAction = convertView.FindViewById<ImageButton>(Resource.Id.DrawerMenuListItemSecondaryAction);

                secondaryAction.SetImageResource(secondaryActionItem.SecondaryActionResource);

                secondaryAction.Tag = position;
                secondaryAction.SetOnClickListener(this);

                if (item.Accent.HasValue)
                {
                    secondaryAction.SetColorFilter(Utils.ImageUtils.GetColorFilter(item.Accent.Value));
                }
            }

            return convertView;
        }

        public void OnClick(View v)
        {
            var pos = (int) v.Tag;
            var drawerItem = this[pos];

            if (drawerItem is SecondaryActionDrawerMenuItem)
            {
                var secondaryActionItem = drawerItem as SecondaryActionDrawerMenuItem;
                secondaryActionItem.SecondaryAction();
            }
        }
    }
}
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
using Presentation.Utils;
using Menu = LSRetail.Omni.Domain.DataModel.Base.Menu.Menu;

namespace Presentation.Adapters
{
    public class MenuSpinnerAdapter : BaseAdapter<Menu>, ISpinnerAdapter
    {
        private readonly Context context;

        public MenuSpinnerAdapter(Context context)
        {
            this.context = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return AppData.MobileMenu.MenuNodes.Count; }
        }

        public override Menu this[int position]
        {
            get { return AppData.MobileMenu.MenuNodes[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                convertView = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.MenuSpinner);
            }

            convertView.FindViewById<TextView>(Resource.Id.MenuSpinnerTitle).Text = this[position].Description;

            return convertView;
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            var menu = this[position];

            if (convertView == null)
            {
                var inflater = ((LayoutInflater)context.GetSystemService(Context.LayoutInflaterService));
                convertView = Utils.Utils.ViewUtils.Inflate(inflater, Resource.Layout.MenuSpinnerListItem);
            }

            convertView.FindViewById<TextView>(Resource.Id.MenuSpinnerListItemTitle).Text = menu.Description;

            var subTitle = convertView.FindViewById<TextView>(Resource.Id.MenuSpinnerListItemSubtitle);

            if (!string.IsNullOrEmpty(menu.ValidDescription))
            {
                subTitle.Text = "Tuesdays";
                subTitle.Visibility = ViewStates.Visible; 
            }
            else
            {
                subTitle.Visibility = ViewStates.Gone; 
            }

            return convertView;
        }
    }
}
using System;
using System.Collections.Generic;

using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;

namespace Presentation.Adapters
{
    class SimpleSpinnerAdapter : ArrayAdapter<string>
    {
        private int resource;
        private int multiLineResource;
        private int selectedPosition;

        private Color normalColor;
        private Color accentColor;

        public SimpleSpinnerAdapter(Context context, int resource, int multiLineResource, IList<string> objects) : base(context, 0, 0, objects)
        {
            this.resource = resource;
            this.multiLineResource = multiLineResource;

            normalColor = new Color(ContextCompat.GetColor(context, Resource.Color.black87));
            accentColor = new Color(ContextCompat.GetColor(context, Resource.Color.accent));
        }


        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = Util.Utils.ViewUtils.Inflate(inflater, multiLineResource);
            }

            ((CheckedTextView)view).Text = GetItem(position);

            if (position == selectedPosition)
            {
                ((CheckedTextView) view).SetTextColor(accentColor);
            }
            else
            {
                ((CheckedTextView)view).SetTextColor(normalColor);
            }

            return view;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = Util.Utils.ViewUtils.Inflate(inflater, resource);
            }

            ((TextView)view).Text = GetItem(position);

            return view;
        }

		public void setSelectedPosition(int position)
		{
			this.selectedPosition = position;
		}
    }
}
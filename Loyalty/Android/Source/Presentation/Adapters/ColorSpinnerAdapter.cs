using System.Collections.Generic;
using System.Linq;

using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Adapters
{
    class ColorSpinnerAdapter : ArrayAdapter<string>
    {
        private int resource;
        private int multiLineResource;
        private int selPosition = -1;
        //private List<int> colorPositions;
        private readonly List<DimValue> values;

        private Color normalColor;
        private Color accentColor;
        private Color posColor;

        public ColorSpinnerAdapter(Context context, int resource, int multiLineResource, List<DimValue> values) : base(context, 0, 0, values.Select(x => x.Value).ToList())
        {
            this.resource = resource;
            this.multiLineResource = multiLineResource;
            this.values = values;

            normalColor = new Color(ContextCompat.GetColor(context, Resource.Color.black87));
            accentColor = new Color(ContextCompat.GetColor(context, Resource.Color.accent));
            posColor = new Color(ContextCompat.GetColor(context, Resource.Color.gray));
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;

            if (view == null)
            {
                LayoutInflater inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
                view = Util.Utils.ViewUtils.Inflate(inflater, multiLineResource);
            }

            var value = values[position];

            ((CheckedTextView)view).Text = value.Value;

            /*
            if (!value.IsAvailable)
            {
                ((CheckedTextView)view).SetTextColor(posColor);
            }
            */
            if (value.IsSelected)
            {
                ((CheckedTextView)view).SetTextColor(accentColor);
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

        public void SetColorItems(List<int> tocolor)
        {
            //colorPositions = tocolor;
        }

        public void SetSel(int sel)
        {
            selPosition = sel;
        }
    }
}
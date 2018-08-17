using System;
using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;

namespace Presentation.Adapters
{
    public class ContactUsAdapter : BaseAdapter<ContactUs.ContactLine>, View.IOnClickListener
    {
        private Activity context;
        private List<ContactUs.ContactLine> contactUsList;
        private readonly Action<ContactUs.ContactLine> onActionItemPressed;

        public ContactUsAdapter(Activity context, List<ContactUs.ContactLine> contactUsList, Action<ContactUs.ContactLine> onActionItemPressed)
            : base()
        {
            this.context = context;
            this.contactUsList = contactUsList;
            this.onActionItemPressed = onActionItemPressed;
        }

        public override long GetItemId(int position)
        {
            return position;
        }


        public override int Count
        {
            get { return contactUsList.Count; }
        }

        public override ContactUs.ContactLine this[int position]
        {
            get { return contactUsList[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var contactLine = contactUsList[position];

            View view = convertView;
            
            if (view == null)
                view = Util.Utils.ViewUtils.Inflate(context.LayoutInflater, Resource.Layout.ContactUsListItem);

            view.FindViewById<TextView>(Resource.Id.ContactUsListItemDescription).Text = contactLine.Description;
            view.FindViewById<TextView>(Resource.Id.ContactUsListItemValue).Text = contactLine.Value;

            var action = view.FindViewById<ImageButton>(Resource.Id.ContactUsListItemAction);

            if (contactLine.Type == ContactUs.ContactLine.ContactType.Phone)
            {
                action.SetImageResource(Resource.Drawable.ic_action_call);
            }
            else if (contactLine.Type == ContactUs.ContactLine.ContactType.Email)
            {
                action.SetImageResource(Resource.Drawable.ic_action_email);
            }
            else if (contactLine.Type == ContactUs.ContactLine.ContactType.Web)
            {
                action.SetImageResource(Resource.Drawable.ic_action_web_site);
            }

            action.Tag = position;
            action.SetOnClickListener(this);

            return view;
        }

        public void OnClick(View v)
        {
            int pos = (int) v.Tag;
            onActionItemPressed(contactUsList[pos]);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace Presentation.Dialogs
{
    public class MultiChoiceAlertDialog : BaseAlertDialog
    {
        private View view;

        public MultiChoiceAlertDialog(Context context, string [] items, IList<bool> checkedItems, string title, string message = "") : base(context)
        {
            Message = message;
            Title = title;

            view = Util.Utils.ViewUtils.Inflate(LayoutInflater, Resource.Layout.AlertDialogListView);

            var listview = view.FindViewById<ListView>(Resource.Id.AlertDialogListViewList);
            listview.Adapter = new ArrayAdapter(context, Resource.Layout.CustomListMultipleChoice, items);
            listview.ChoiceMode = ChoiceMode.Multiple;

            for (int i = 0; i < checkedItems.Count(); i++)
            {
                listview.SetItemChecked(i, checkedItems[i]);
            }

            listview.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
                                      {
                                          checkedItems[args.Position] = !checkedItems[args.Position];
                                          listview.SetItemChecked(args.Position, checkedItems[args.Position]);
                                      };
        }

        public override void Show()
        {
            CreateDialog(view);
        }
    }
}
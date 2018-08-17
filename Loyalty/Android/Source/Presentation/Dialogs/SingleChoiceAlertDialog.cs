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
    public class SingleChoiceAlertDialog : BaseAlertDialog
    {
        private View view;
        private ListView listview;
        
        public int SelectedItem { get { return listview.CheckedItemPosition; } }

        public SingleChoiceAlertDialog(Context context, string[] items, string title, string message = "", int selectedItem = 0) : base(context)
        {
            Message = message;
            Title = title;

            var layoutInflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            view = Util.Utils.ViewUtils.Inflate(layoutInflater, Resource.Layout.AlertDialogListView);

            listview = view.FindViewById<ListView>(Resource.Id.AlertDialogListViewList);
            listview.Adapter = new ArrayAdapter(context, Resource.Layout.CustomListSingleChoiceMedium, items);
            listview.ChoiceMode = ChoiceMode.Single;

            listview.SetItemChecked(selectedItem, true);

            listview.ItemClick += delegate(object sender, AdapterView.ItemClickEventArgs args)
                                      {
                                          listview.SetItemChecked(args.Position, true);
                                      };
        }

        public override void Show()
        {
            CreateDialog(view);
        }
    }
}
using System;

using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using Presentation.Activities.Base;
using Presentation.Adapters;

namespace Presentation.Activities.QRCode
{
    public class QRCodeItemsFragment : LoyaltyFragment
    {
        private RecyclerView qrRecyclerView;
        private QRCodeAdapter adapter;

        public static QRCodeItemsFragment NewInstance()
        {
            var welcomeDetail = new QRCodeItemsFragment() { Arguments = new Bundle() };
            return welcomeDetail;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            var view = Util.Utils.ViewUtils.Inflate(inflater, Resource.Layout.QRCodeItemList);

            qrRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.QRCodeViewQRCodeList);
            qrRecyclerView.SetLayoutManager(new LinearLayoutManager(Activity));
            qrRecyclerView.HasFixedSize = true;
            qrRecyclerView.AddItemDecoration(new BaseRecyclerAdapter.DefaultItemDecoration(Activity));

            adapter = new QRCodeAdapter(Activity);

            qrRecyclerView.SetAdapter(adapter);
            adapter.SetItems(Activity);

            return view;
        }
    }
}
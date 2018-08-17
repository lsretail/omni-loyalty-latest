using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Presentation.Views
{
    public class LoyaltyRecyclerView : RecyclerView
    {
        private View emptyView;
        private Observer observer;

        protected LoyaltyRecyclerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LoyaltyRecyclerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        public LoyaltyRecyclerView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public LoyaltyRecyclerView(Context context) : base(context)
        {
        }

        public void CheckIfEmpty()
        {
            if (emptyView != null && GetAdapter() != null)
            {
                bool emptyViewVisible = GetAdapter().ItemCount == 0;
                emptyView.Visibility = emptyViewVisible ? ViewStates.Visible : ViewStates.Gone;
                Visibility = emptyViewVisible ? ViewStates.Gone : ViewStates.Visible;
            }
        }

        public override void SetAdapter(Adapter adapter)
        {
            if (observer == null)
            {
                observer = new Observer(CheckIfEmpty);
            }

            Adapter oldAdapter = GetAdapter();
            if (oldAdapter != null)
            {
                oldAdapter.UnregisterAdapterDataObserver(observer);
            }
            base.SetAdapter(adapter);
            if (adapter != null)
            {
                adapter.RegisterAdapterDataObserver(observer);
            }

            CheckIfEmpty();
        }

        public void SetEmptyView(View emptyView)
        {
            this.emptyView = emptyView;
            CheckIfEmpty();
        }

        private class Observer : AdapterDataObserver
        {
            public Action checkIfEmpty;

            public Observer(Action checkIfEmpty)
            {
                this.checkIfEmpty = checkIfEmpty;
            }

            public override void OnChanged()
            {
                base.OnChanged();
                checkIfEmpty();
            }

            public override void OnItemRangeInserted(int positionStart, int itemCount)
            {
                base.OnItemRangeInserted(positionStart, itemCount);
                checkIfEmpty();
            }

            public override void OnItemRangeRemoved(int positionStart, int itemCount)
            {
                base.OnItemRangeRemoved(positionStart, itemCount);
                checkIfEmpty();
            }
        }
    }
}
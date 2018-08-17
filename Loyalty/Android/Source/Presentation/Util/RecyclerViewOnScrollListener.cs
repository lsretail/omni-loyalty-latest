using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Presentation.Util
{
    public class RecyclerViewOnScrollListener : RecyclerView.OnScrollListener
    {
        public interface IRecyclerViewOnScrollListener
        {
            void OnScrollStateChanged(RecyclerView recyclerView, int newState);
            void OnScrolled(RecyclerView recyclerView, int dx, int dy);
        }

        private IRecyclerViewOnScrollListener onScrollListener;

        public RecyclerViewOnScrollListener(IRecyclerViewOnScrollListener onScrollListener)
        {
            this.onScrollListener = onScrollListener;
        }

        public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
        {
            base.OnScrollStateChanged(recyclerView, newState);
            onScrollListener.OnScrollStateChanged(recyclerView, newState);
        }

        public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
        {
            base.OnScrolled(recyclerView, dx, dy);
            onScrollListener.OnScrolled(recyclerView, dx, dy);
        }
    }
}
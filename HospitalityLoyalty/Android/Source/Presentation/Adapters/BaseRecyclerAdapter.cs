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

namespace Presentation.Adapters
{
    public abstract class BaseRecyclerAdapter : RecyclerView.Adapter
    {
        public abstract int GetColumnSpan(int position, int maxColumns);
    }

    public class GridSpanSizeLookup : GridLayoutManager.SpanSizeLookup
    {
        private BaseRecyclerAdapter baseRecyclerAdapter;
        private readonly int maxColumns;

        public GridSpanSizeLookup(BaseRecyclerAdapter baseRecyclerAdapter, int maxColumns)
        {
            this.baseRecyclerAdapter = baseRecyclerAdapter;
            this.maxColumns = maxColumns;
        }

        public override int GetSpanSize(int position)
        {
            return baseRecyclerAdapter.GetColumnSpan(position, maxColumns);
        }
    }

    public class RecyclerOnScrollListener : RecyclerView.OnScrollListener
    {
        public interface IOnScrollChangedListener
        {
            void OnScrolled(RecyclerView view, int dx, int dy);
        }

        private IOnScrollChangedListener onScrollChangedListener;

        public RecyclerOnScrollListener(IOnScrollChangedListener onScrollChangedListener)
        {
            this.onScrollChangedListener = onScrollChangedListener;
        }

        public override void OnScrollStateChanged(RecyclerView view, int scrollState)
        {
            base.OnScrollStateChanged(view, scrollState);
        }

        public override void OnScrolled(RecyclerView view, int dx, int dy)
        {
            base.OnScrolled(view, dx, dy);

            onScrollChangedListener.OnScrolled(view, dx, dy);
        }
    }
}
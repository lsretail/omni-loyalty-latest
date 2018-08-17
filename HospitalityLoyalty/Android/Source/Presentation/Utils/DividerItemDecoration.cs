using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace Presentation.Utils
{
    public class DividerItemDecoration : RecyclerView.ItemDecoration
    {
        private int listSpacing;
        private int gridSpacing;

        public DividerItemDecoration(Context context)
        {
            listSpacing = context.Resources.GetDimensionPixelSize(Resource.Dimension.OneDP);
            gridSpacing = context.Resources.GetDimensionPixelSize(Resource.Dimension.FourDP);
        }

        public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
        {
            if (parent.GetLayoutManager() is GridLayoutManager)
            {
                var gridLayoutManager = parent.GetLayoutManager() as GridLayoutManager;

                var columnCount = gridLayoutManager.SpanCount;

                var spacing = gridSpacing / 2;

                outRect.Left = spacing;
                outRect.Right = spacing;
                outRect.Top = spacing;
                outRect.Bottom = spacing;
            }
            else if (parent.GetLayoutManager() is StaggeredGridLayoutManager)
            {
                var gridLayoutManager = parent.GetLayoutManager() as StaggeredGridLayoutManager;

                var columnCount = gridLayoutManager.SpanCount;

                var spacing = gridSpacing / 2;

                outRect.Left = spacing;
                outRect.Right = spacing;
                outRect.Top = spacing;
                outRect.Bottom = spacing;
            }
            else
            {
                if (parent.GetChildAdapterPosition(view) != 0)
                {
                    outRect.Top = listSpacing;
                }
            }
        }
    }
}
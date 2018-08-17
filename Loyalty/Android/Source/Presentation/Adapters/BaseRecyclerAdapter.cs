
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;

namespace Presentation.Adapters
{
    public abstract class BaseRecyclerAdapter : RecyclerView.Adapter
    {
        public enum ListItemSize
        {
            Normal = 0,
            SmallCard = 1
        }

        public virtual int GetColumnSpan(int position, int maxColumns)
        {
            return 1;
        }

        public class GridSpanSizeLookup : GridLayoutManager.SpanSizeLookup
        {
            private BaseRecyclerAdapter baseRecyclerAdapter;
            private readonly int columns;

            public GridSpanSizeLookup(Context context, BaseRecyclerAdapter baseRecyclerAdapter)
            {
                this.baseRecyclerAdapter = baseRecyclerAdapter;
                columns = context.Resources.GetInteger(Resource.Integer.CardColumns);
            }

            public override int GetSpanSize(int position)
            {
                return baseRecyclerAdapter.GetColumnSpan(position, columns);
            }
        }
        
        public class DefaultItemDecoration : RecyclerView.ItemDecoration
        {
            private int listSpacing;
            private int gridSpacing;

            public DefaultItemDecoration(Context context)
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

                    var spacing = gridSpacing/2;

                    outRect.Left = spacing;
                    outRect.Right = spacing;
                    outRect.Top = spacing;
                    outRect.Bottom = spacing;
                }
                else if (parent.GetLayoutManager() is StaggeredGridLayoutManager)
                {
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

        public class HorizontalItemDecoration : RecyclerView.ItemDecoration
        {
            private int spacing;

            public HorizontalItemDecoration(Context context)
            {
                spacing = context.Resources.GetDimensionPixelSize(Resource.Dimension.FourDP);
            }

            public override void GetItemOffsets(Rect outRect, View view, RecyclerView parent, RecyclerView.State state)
            {
                if (parent.GetChildAdapterPosition(view) != 0)
                {
                    outRect.Left = spacing;
                }
            }
        }
    }
}
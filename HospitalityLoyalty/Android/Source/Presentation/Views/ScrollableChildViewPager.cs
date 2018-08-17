using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Presentation.Views
{
    public class ScrollableChildViewPager : ViewPager
    {
        public int ChildId { get; set; }
        protected ScrollableChildViewPager(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ScrollableChildViewPager(Context p0, IAttributeSet p1)
            : base(p0, p1)
        {
        }

        public ScrollableChildViewPager(Context p0)
            : base(p0)
        {
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (ChildId > 0)
            {
                View scroll = FindViewById(ChildId);
                if (scroll != null)
                {
                    Rect rect = new Rect();
                    scroll.GetHitRect(rect);
                    if (rect.Contains((int)ev.GetX(), (int)ev.GetY()))
                    {
                        return false;
                    }
                }
            }

            return base.OnInterceptTouchEvent(ev);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Presentation.Views
{
    public class InterceptingLinearLayout : LinearLayout
    {
        protected InterceptingLinearLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public InterceptingLinearLayout(Context context) : base(context)
        {
        }

        public InterceptingLinearLayout(Context context, IAttributeSet attrs) : base(context, attrs)
        {
        }

        public InterceptingLinearLayout(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return true;
        }
    }
}
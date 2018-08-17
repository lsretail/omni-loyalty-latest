using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;

namespace Presentation.Views
{
    //To fix a bug causing the hint not to appear until focused on api > 20 - https://gist.github.com/ljubisa987/e33cd5597da07172c55d
    public class CustomTextInputLayout : TextInputLayout, ViewTreeObserver.IOnGlobalLayoutListener
    {
        private View child;

        protected CustomTextInputLayout(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public CustomTextInputLayout(Context p0) : base(p0)
        {
        }

        public CustomTextInputLayout(Context p0, IAttributeSet p1) : base(p0, p1)
        {
        }

        public override void AddView(View child, int index, ViewGroup.LayoutParams @params)
        {
            this.child = child;
            Utils.Utils.ViewUtils.AddOnGlobalLayoutListener(child, this);

            base.AddView(child, index, @params);
        }

        public void OnGlobalLayout()
        {
            Utils.Utils.ViewUtils.RemoveOnGlobalLayoutListener(child, this);
            OnLayout(false, Left, Top, Right, Bottom);
        }
    }
}
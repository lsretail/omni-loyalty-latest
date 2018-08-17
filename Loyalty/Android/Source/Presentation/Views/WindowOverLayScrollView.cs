using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Print;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;
using Presentation.Util;

namespace Presentation.Views
{
    public class WindowOverLayScrollView : ScrollView
    {
        private bool marginSet = false;
        private bool translateView = false;

        public View TranslatingTopImageView { get; set; }
        public View TopAnchorView { get; set; }
        public View BelowAnchorView { get; set; }

        public bool HasFab
        {
            set
            {
                if (value)
                {
                    halfFabHeightWithPadding = Resources.GetDimensionPixelSize(Resource.Dimension.HalfMiniFabPadding);
                }
                else
                {
                    halfFabHeightWithPadding = 0;
                }
            }
        }
        

        public int ActionBarAlpha { get; private set; }

        private int belowAnchorViewOriginalLeftMargin = 0;
        private int belowAnchorViewOriginalTopMargin = 0;
        private int belowAnchorViewOriginalRightMargin = 0;
        private int belowAnchorViewOriginalBottomMargin = 0;

        private int halfFabHeightWithPadding = 0;
        private int topAnchorMeasuredHeight = 0;

        public interface IOnScrollChangedListener
        {
            void OnScrollChanged(ScrollView who, int l, int t, int oldl, int oldt);
        }

        public IOnScrollChangedListener OnScrollChangedListener { private get; set; }
        public bool IsOverScrollEnabled { get; set; }

        public WindowOverLayScrollView(Context context) : base(context)
        {
            Initialize();
        }

        public WindowOverLayScrollView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize();
        }

        public WindowOverLayScrollView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Initialize();
        }

        private void Initialize()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
            {
                translateView = true;
            }

            HasFab = true;
            //halfFabHeightWithPadding = Resources.GetDimensionPixelSize(Resource.Dimension.HalfMiniFabPadding);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            
            if (TopAnchorView.MeasuredHeight > 0 && BelowAnchorView.LayoutParameters is ViewGroup.MarginLayoutParams)
            {
                if (translateView)
                {
                    var layoutParams = (ViewGroup.MarginLayoutParams)BelowAnchorView.LayoutParameters;

                    if (!marginSet)
                    {
                        belowAnchorViewOriginalLeftMargin = layoutParams.LeftMargin;
                        belowAnchorViewOriginalTopMargin = layoutParams.TopMargin;
                        belowAnchorViewOriginalRightMargin = layoutParams.RightMargin;
                        belowAnchorViewOriginalBottomMargin = layoutParams.BottomMargin;
                        marginSet = true;
                    }

                    layoutParams.SetMargins(belowAnchorViewOriginalLeftMargin, belowAnchorViewOriginalTopMargin + TopAnchorView.MeasuredHeight - halfFabHeightWithPadding, belowAnchorViewOriginalRightMargin, belowAnchorViewOriginalBottomMargin);
                }
                else
                {
                    if (!marginSet)
                    {
                        belowAnchorViewOriginalLeftMargin = BelowAnchorView.PaddingLeft;
                        belowAnchorViewOriginalTopMargin = BelowAnchorView.PaddingTop;
                        belowAnchorViewOriginalRightMargin = BelowAnchorView.PaddingRight;
                        belowAnchorViewOriginalBottomMargin = BelowAnchorView.PaddingBottom;
                        marginSet = true;
                    }

                    BelowAnchorView.SetPadding(belowAnchorViewOriginalLeftMargin, belowAnchorViewOriginalTopMargin + TopAnchorView.MeasuredHeight - halfFabHeightWithPadding, belowAnchorViewOriginalRightMargin, belowAnchorViewOriginalBottomMargin);
                }

                if (TopAnchorView.MeasuredHeight != topAnchorMeasuredHeight)
                {
                    topAnchorMeasuredHeight = TopAnchorView.MeasuredHeight;
                    base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
                }
            }
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
        }

        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {
            base.OnScrollChanged(l, t, oldl, oldt);

            if (OnScrollChangedListener != null)
            {
                OnScrollChangedListener.OnScrollChanged(this, l, t, oldl, oldt);
            }

            var loyaltyActivity = Context as LoyaltyFragmentActivity;

            if (loyaltyActivity == null)
                return;

            int headerHeight = TranslatingTopImageView.Height - loyaltyActivity.SupportActionBar.Height;
            float ratio = (float)Math.Min(Math.Max(t, 0), headerHeight) / headerHeight;

            if (translateView && TranslatingTopImageView != null)
            {
                int translationY = (int)((ratio * TranslatingTopImageView.Height) / 2);
                TranslatingTopImageView.TranslationY = translationY;
                
                var anchorLocation = new int[2];
                var bottomContentLocation = new int[2];
                TopAnchorView.GetLocationOnScreen(anchorLocation);
                BelowAnchorView.GetLocationOnScreen(bottomContentLocation);

                //if (t + windowOverlayActivity.SupportActionBar.Height > TranslatingTopImageView.Height)
                if (anchorLocation[1] < loyaltyActivity.StatusBarHeight + loyaltyActivity.SupportActionBar.Height || anchorLocation[1] + TopAnchorView.Height - halfFabHeightWithPadding > bottomContentLocation[1])
                {
                    var translation = TopAnchorView.TranslationY + (loyaltyActivity.StatusBarHeight + loyaltyActivity.SupportActionBar.Height) - anchorLocation[1];

                    Utils.LogUtils.Log(translation.ToString());

                    TopAnchorView.TranslationY = translation;
                    //TopAnchorView.TranslationY = -(TranslatingTopImageView.Height - (t + windowOverlayActivity.SupportActionBar.Height));
                }
                else
                {
                    TopAnchorView.TranslationY = 0;
                }
            }
            
            var windowOverlayActivity = Context as WindowOverlayActivity;

            if (windowOverlayActivity == null)
                return;
            
            ActionBarAlpha = (int)(ratio * 255);

            if (Context is WindowOverlayActivity)
                (Context as WindowOverlayActivity).SetActionBarAlpha(ActionBarAlpha);
        }



        protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX, int scrollRangeY,
                                             int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
        {
            return base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, IsOverScrollEnabled ? maxOverScrollX : 0, IsOverScrollEnabled ? maxOverScrollY : 0, isTouchEvent);
        }


        private float xDistance, yDistance, lastX, lastY;
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            switch (ev.Action) {
                case MotionEventActions.Down:
                    xDistance = yDistance = 0f;
                    lastX = ev.GetX();
                    lastY = ev.GetY();
                    break;

                case MotionEventActions.Move:
                    float curX = ev.GetX();
                    float curY = ev.GetY();
                    xDistance += Math.Abs(curX - lastX);
                    yDistance += Math.Abs(curY - lastY);
                    lastX = curX;
                    lastY = curY;
                    if(xDistance > yDistance)
                        return false;
                    break;
            }

            return base.OnInterceptTouchEvent(ev);
        }
    }
}
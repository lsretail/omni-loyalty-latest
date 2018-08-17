using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Presentation.Views
{
    public class ProgressButton : FrameLayout
    {
        public enum ProgressButtonState
        {
            Normal = 0,
            Loading = 1,
            Done = 2
        }

        private TextView textView;
        private ProgressBar progressBar;

        private ProgressButtonState state;

        private string normalText;
        private string doneText;

        private Drawable normalDrawable;
        private Drawable loadingDrawable;
        private Drawable doneDrawable;

        public ProgressButton(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            Initialize(attrs);
        }

        public ProgressButton(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize(attrs);
        }

        public ProgressButtonState State
        {
            get { return state; }
            set
            {
                state = value;

                switch (state)
                {
                    case ProgressButtonState.Normal:
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                            Foreground = ContextCompat.GetDrawable(Context, Resource.Drawable.SelectableBackgroundWhiteInside);

                        Background = normalDrawable;

                        textView.Text = normalText;

                        textView.Visibility = ViewStates.Visible;
                        progressBar.Visibility = ViewStates.Gone;

                        textView.SetTextColor(new Color(ContextCompat.GetColor(Context, Resource.Color.white87)));
                        break;

                    case ProgressButtonState.Loading:
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                            Foreground = null;

                        Background = loadingDrawable;

                        textView.Visibility = ViewStates.Gone;
                        progressBar.Visibility = ViewStates.Visible;
                        break;

                    case ProgressButtonState.Done:
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                            Foreground = ContextCompat.GetDrawable(Context, Resource.Drawable.selecteble_background);

                        Background = doneDrawable;

                        textView.Text = doneText;
                        textView.Visibility = ViewStates.Visible;
                        progressBar.Visibility = ViewStates.Gone;

                        textView.SetTextColor(new Color(ContextCompat.GetColor(Context, Resource.Color.accent)));
                        break;
                }
            }
        }

        private void Initialize(IAttributeSet attrs)
        {
            var progressButtonAttributes = Context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.ProgressButton, 0, 0);

            var insetLeft = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetLeft, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            var insetRight = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetRight, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            var insetTop = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetTop, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            var insetBottom = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetBottom, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            var roundedCorners = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_roundCorners, false);

            normalText = progressButtonAttributes.GetString(Resource.Styleable.ProgressButton_normalText);
            doneText = progressButtonAttributes.GetString(Resource.Styleable.ProgressButton_doneText);

            #region Drawables

            normalDrawable = new StateListDrawable();

            var colorPressed = new Color(ContextCompat.GetColor(Context, Resource.Color.accentpressed));

            var colorNormal = new Color(ContextCompat.GetColor(Context, Resource.Color.accentnormal));

            var pressedShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
            pressedShapeDrawable.Paint.Color = colorPressed;

            var normalShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
            normalShapeDrawable.Paint.Color = colorNormal;


            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                (normalDrawable as StateListDrawable).AddState(new int[] { Android.Resource.Attribute.StatePressed }, new InsetDrawable(pressedShapeDrawable, insetLeft, insetTop, insetRight, insetBottom));
            
            (normalDrawable as StateListDrawable).AddState(new int[] { }, new InsetDrawable(normalShapeDrawable, insetLeft, insetTop, insetRight, insetBottom));

            var whiteShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
            whiteShapeDrawable.Paint.Color = new Color(ContextCompat.GetColor(Context, Resource.Color.white));

            loadingDrawable = new LayerDrawable(new Drawable[]
                {
                    new InsetDrawable(normalShapeDrawable, insetLeft, insetTop, insetRight, insetBottom),
                    new InsetDrawable(whiteShapeDrawable, Resources.GetDimensionPixelSize(Resource.Dimension.OneDP))
                });

            var donePressedStateDrawable = new ShapeDrawable(CreateRect(roundedCorners));
            donePressedStateDrawable.Paint.Color = new Color(ContextCompat.GetColor(Context, Resource.Color.background_pressed));

            var donePressedDrawable = new LayerDrawable(new Drawable[]
                {
                    new InsetDrawable(normalShapeDrawable, insetLeft, insetTop, insetRight, insetBottom),
                    new InsetDrawable(new LayerDrawable(new Drawable[] {whiteShapeDrawable, donePressedStateDrawable}), Resources.GetDimensionPixelSize(Resource.Dimension.OneDP)), 
                });

            doneDrawable = new StateListDrawable();

            if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                (doneDrawable as StateListDrawable).AddState(new int[] { Android.Resource.Attribute.StatePressed }, donePressedDrawable);

            (doneDrawable as StateListDrawable).AddState(new int[] { }, loadingDrawable);

            #endregion

            textView = new TextView(Context, null, Resource.Style.Subhead);
            textView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            textView.SetTextColor(new Color(ContextCompat.GetColor(Context, Resource.Color.white)));
            textView.Gravity = GravityFlags.Center;
            textView.SetTypeface(null, TypefaceStyle.Bold);

            progressBar = new ProgressBar(Context, null, Android.Resource.Attribute.ProgressBarStyle);
            progressBar.LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            (progressBar.LayoutParameters as FrameLayout.LayoutParams).Gravity = GravityFlags.Center;
            progressBar.Visibility = ViewStates.Gone;
            progressBar.IndeterminateDrawable.SetColorFilter(Util.Utils.ImageUtils.GetColorFilter(new Color(ContextCompat.GetColor(Context, Resource.Color.accent)), PorterDuff.Mode.Multiply));

            AddView(textView);
            AddView(progressBar);

            State = ProgressButtonState.Normal;
        }

        private Shape CreateRect(bool roundedCorners)
        {
            if (roundedCorners)
            {
                var roundValue = Resources.GetDimensionPixelSize(Resource.Dimension.TwoDP);
                return new RoundRectShape(new float[] { roundValue, roundValue, roundValue, roundValue, roundValue, roundValue, roundValue, roundValue }, null, null);
            }
            else
            {
                return new RectShape();
            }
        }
    }
}
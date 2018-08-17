using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Animation;
using Android.App;
using Android.Content;
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
    public class ColoredButton : FrameLayout
    {
        private TextView textView;
        private ImageView imageView;

        private Drawable drawableLeft;

        private int insetLeft;
        private int insetRight;
        private int insetTop;
        private int insetBottom;
        private bool roundedCorners;

        private Color? oldNormalColor;
        private Color? oldPressedColor;

        protected ColoredButton(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public ColoredButton(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            Initialize(attrs);
        }

        public ColoredButton(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            Initialize(attrs);
        }

        private void Initialize(IAttributeSet attrs)
        {
            var progressButtonAttributes = Context.Theme.ObtainStyledAttributes(attrs, Resource.Styleable.ProgressButton, 0, 0);

            var buttonColorResource = progressButtonAttributes.GetResourceId(Resource.Styleable.ProgressButton_buttonColor, 0);
            var buttonColorPressedResource = progressButtonAttributes.GetResourceId(Resource.Styleable.ProgressButton_buttonColorPressed, 0);

            var drawableLeftResourceId = progressButtonAttributes.GetResourceId(Resource.Styleable.ProgressButton_drawableLeft, 0);

            insetLeft = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetLeft, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            insetRight = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetRight, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            insetTop = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetTop, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            insetBottom = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetBottom, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;

            roundedCorners = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_roundCorners, false);

            var text = progressButtonAttributes.GetString(Resource.Styleable.ProgressButton_normalText);

            #region Drawables

            if (buttonColorResource != 0 && buttonColorPressedResource != 0)
            {
                var colorPressed = new Color(ContextCompat.GetColor(Context, buttonColorPressedResource));

                var colorNormal = new Color(ContextCompat.GetColor(Context, buttonColorResource));

                SetBackgroundDrawable(colorNormal, colorPressed);
            }

            if (drawableLeftResourceId != 0)
            {
                drawableLeft = ContextCompat.GetDrawable(Context, drawableLeftResourceId);
            }

            #endregion

            var imageViewHeight = Resources.GetDimensionPixelSize(Resource.Dimension.ListIconPaddingRight);
            var TextViewLeftPadding = 0;

            imageView = new ImageView(Context);
            imageView.LayoutParameters = new FrameLayout.LayoutParams(imageViewHeight, imageViewHeight);
            imageView.SetPadding(Context.Resources.GetDimensionPixelSize(Resource.Dimension.HalfBasePadding), 0, 0, 0);
            
            (imageView.LayoutParameters as FrameLayout.LayoutParams).Gravity = GravityFlags.CenterVertical;

            if (drawableLeft != null)
            {
                TextViewLeftPadding = Context.Resources.GetDimensionPixelSize(Resource.Dimension.HalfBasePadding) + imageViewHeight;
                imageView.SetImageDrawable(drawableLeft);
            }
            else
            {
                TextViewLeftPadding = Context.Resources.GetDimensionPixelSize(Resource.Dimension.BasePadding);
            }

            textView = new TextView(Context, null, Resource.Style.SubheadLight);
            textView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            textView.SetPadding(TextViewLeftPadding, 0, Context.Resources.GetDimensionPixelSize(Resource.Dimension.BasePadding), 0);
            textView.Gravity = GravityFlags.Center;
            textView.SetTypeface(null, TypefaceStyle.Bold);
            textView.SetTextColor(new Color(ContextCompat.GetColor(Context, Resource.Color.white87)));
            textView.Text = text;

            AddView(imageView);
            AddView(textView);
        }

        public void SetText(string text)
        {
            textView.Text = text;
        }

        public string Text
        {
            get { return textView.Text; }
            set { textView.Text = value; }
        }

        public void SetText(int resourceId)
        {
            textView.SetText(resourceId);
        }

        public void SetBackgroundDrawable(Color normalColor, Color pressedColor)
        {
            TransitionDrawable transitionDrawable;

            if (oldNormalColor != null && oldPressedColor != null)
            {
                var newBackground = new StateListDrawable();

                var newPressedShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
                newPressedShapeDrawable.Paint.Color = pressedColor;

                var newNormalShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
                newNormalShapeDrawable.Paint.Color = normalColor;

                (newBackground as StateListDrawable).AddState(new int[] { Android.Resource.Attribute.StatePressed }, newPressedShapeDrawable);
                (newBackground as StateListDrawable).AddState(new int[] { }, newNormalShapeDrawable);

                var oldBackground = new StateListDrawable();

                var oldPressedShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
                oldPressedShapeDrawable.Paint.Color = oldPressedColor.Value;

                var oldNormalShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
                oldNormalShapeDrawable.Paint.Color = oldNormalColor.Value;

                (oldBackground as StateListDrawable).AddState(new int[] { Android.Resource.Attribute.StatePressed }, new InsetDrawable(oldPressedShapeDrawable, insetLeft, insetTop, insetRight, insetBottom));
                (oldBackground as StateListDrawable).AddState(new int[] { }, new InsetDrawable(oldNormalShapeDrawable, insetLeft, insetTop, insetRight, insetBottom));

                transitionDrawable = new TransitionDrawable(new Drawable[] { oldBackground, newBackground });

                Background = transitionDrawable;
                transitionDrawable.StartTransition(300);
            }
            else
            {
                var newBackground = new StateListDrawable();

                var newPressedShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
                newPressedShapeDrawable.Paint.Color = pressedColor;

                var newNormalShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
                newNormalShapeDrawable.Paint.Color = normalColor;

                (newBackground as StateListDrawable).AddState(new int[] { Android.Resource.Attribute.StatePressed }, new InsetDrawable(newPressedShapeDrawable, insetLeft, insetTop, insetRight, insetBottom));
                (newBackground as StateListDrawable).AddState(new int[] { }, new InsetDrawable(newNormalShapeDrawable, insetLeft, insetTop, insetRight, insetBottom));

                Background = newBackground;
            }

            oldNormalColor = normalColor;
            oldPressedColor = pressedColor;
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

        public override bool Pressed 
        {
            get { return base.Pressed; } 
            set
            {
                if(Pressed && (Parent as View).Pressed)
                    return;
                base.Pressed = value;
            } 
        }
    }
}
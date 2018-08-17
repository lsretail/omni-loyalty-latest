using System;
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
        private int insetLeft;
        private int insetRight;
        private int insetTop;
        private int insetBottom;
        private bool roundedCorners;

        private Color? oldNormalColor;
        private Color? oldPressedColor;

        protected ColoredButton(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ColoredButton(Context context, IAttributeSet attrs)
            : base(context, attrs)
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

            insetLeft = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetLeft, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            insetRight = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetRight, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            insetTop = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetTop, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;
            insetBottom = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_insetBottom, false) ? Resources.GetDimensionPixelSize(Resource.Dimension.ViewInset) : 0;

            roundedCorners = progressButtonAttributes.GetBoolean(Resource.Styleable.ProgressButton_roundCorners, false);

            var text = progressButtonAttributes.GetString(Resource.Styleable.ProgressButton_normalText);

            #region Drawables


            if (buttonColorResource != 0)
            {
                Color colorPressed, colorNormal;
                
                colorNormal = new Color(ContextCompat.GetColor(Context, buttonColorResource));

                if (buttonColorPressedResource == 0)
                {
                    colorPressed = new Color(colorNormal.R, colorNormal.G, colorNormal.B, (int)(colorNormal.A * 0.8));
                }
                else
                {
                    colorPressed = new Color(ContextCompat.GetColor(Context, buttonColorPressedResource));
                }

                SetBackgroundDrawable(colorNormal, colorPressed);
                
                if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                {
                    Foreground = ContextCompat.GetDrawable(Context, Resource.Drawable.SelectableBackgroundWhiteInside);
                }
            }

            #endregion

            textView = new TextView(Context, null, Resource.Style.ButtonLight);
            textView.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            textView.SetTextColor(new Color(ContextCompat.GetColor(Context, Resource.Color.white)));
            textView.Gravity = GravityFlags.Center;
            textView.SetTypeface(null, TypefaceStyle.Bold);
            textView.Text = text;

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

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    (newBackground as StateListDrawable).AddState(new int[] {Android.Resource.Attribute.StatePressed}, newPressedShapeDrawable);
                }

                (newBackground as StateListDrawable).AddState(new int[] { }, newNormalShapeDrawable);

                var oldBackground = new StateListDrawable();

                var oldPressedShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
                oldPressedShapeDrawable.Paint.Color = oldPressedColor.Value;

                var oldNormalShapeDrawable = new ShapeDrawable(CreateRect(roundedCorners));
                oldNormalShapeDrawable.Paint.Color = oldNormalColor.Value;

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    (oldBackground as StateListDrawable).AddState(new int[] { Android.Resource.Attribute.StatePressed }, new InsetDrawable(oldPressedShapeDrawable, insetLeft, insetTop, insetRight, insetBottom));
                }  
             
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

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                {
                    (newBackground as StateListDrawable).AddState(new int[] { Android.Resource.Attribute.StatePressed }, new InsetDrawable(newPressedShapeDrawable, insetLeft, insetTop, insetRight, insetBottom));
                }
                    
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
                if (Pressed && (Parent as View).Pressed)
                    return;
                base.Pressed = value;
            }
        }
    }
}
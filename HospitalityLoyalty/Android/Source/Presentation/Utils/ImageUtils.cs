using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace Presentation.Utils
{
    public class ImageUtils
    {
        public static Bitmap DecodeImage(string decodeString)
        {
            try
            {
                if (!string.IsNullOrEmpty(decodeString))
                {
                    byte[] decodedString = Base64.Decode(decodeString, Base64Flags.Default);

                    BitmapFactory.Options options = new BitmapFactory.Options();

                    options.InSampleSize = 1;

                    var bitmap = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length, options);

                    options.Dispose();

                    return bitmap;
                }
            }
            catch (Exception)
            {
                System.GC.Collect();
                return DecodeImage(decodeString);
            }
            
            return null;
        }

        public static void CrossfadeImage(ImageView imageView, Drawable image) {

          //imageView <-- The View which displays the images
          //images[] <-- Holds R references to the images to display
          //imageIndex <-- index of the first image to show in images[] 
          //forever <-- If equals true then after the last image it starts all over again with the first image resulting in an infinite loop. You have been warned.

            int fadeInDuration = 500; // Configure time values here

            imageView.Visibility = ViewStates.Invisible;    //Visible or invisible by default - this will apply when the animation ends

            imageView.SetImageDrawable(image);

            Animation fadeIn = new AlphaAnimation(0, 1);
            fadeIn.Interpolator = new DecelerateInterpolator(); // add this
            fadeIn.Duration = fadeInDuration;

            AnimationSet animation = new AnimationSet(false); // change to false
            animation.AddAnimation(fadeIn);
            animation.RepeatCount = 1;

            animation.AnimationEnd += (sender, args) =>
                {
                    imageView.Visibility = ViewStates.Visible;
                };

            imageView.Animation = animation;
        }

        public static void DisplayImage(ImageView imageView, Bitmap image, View imageColorView = null)
        {
            imageView.SetImageBitmap(image);

            if (imageColorView != null) imageColorView.SetBackgroundColor(Color.Transparent);
        }

        public static void CrossfadeImage(ImageView imageView, Bitmap image, View imageColorView = null)
        {

            //imageView <-- The View which displays the images
            //images[] <-- Holds R references to the images to display
            //imageIndex <-- index of the first image to show in images[] 
            //forever <-- If equals true then after the last image it starts all over again with the first image resulting in an infinite loop. You have been warned.

            var imageId = (string)imageView.Tag;
            int fadeInDuration; // Configure time values here

            fadeInDuration = 500;

            imageView.Visibility = ViewStates.Invisible;    //Visible or invisible by default - this will apply when the animation ends

            imageView.SetImageBitmap(image);

            Animation fadeIn = new AlphaAnimation(0, 1);
            fadeIn.Interpolator = new DecelerateInterpolator(); // add this
            fadeIn.Duration = fadeInDuration;

            AnimationSet animation = new AnimationSet(false); // change to false
            animation.AddAnimation(fadeIn);
            animation.RepeatCount = 1;

            animation.AnimationEnd += (sender, args) =>
            {
                imageView.Visibility = ViewStates.Visible;

                if (imageId == (string) imageView.Tag)
                {
                    if (imageColorView != null) imageColorView.SetBackgroundColor(Color.Transparent);
                }
            };

            imageView.Animation = animation;
        }

        public static void CrossfadeImage(ImageView imageView, Drawable drawable, View imageColorView = null, bool crossfade = true)
        {
            if (crossfade)
            {
                var imageId = (string)imageView.Tag;
                int fadeInDuration; // Configure time values here

                fadeInDuration = 500;

                imageView.Visibility = ViewStates.Invisible;
                //Visible or invisible by default - this will apply when the animation ends

                imageView.SetImageDrawable(drawable);

                Animation fadeIn = new AlphaAnimation(0, 1);
                fadeIn.Interpolator = new DecelerateInterpolator(); // add this
                fadeIn.Duration = fadeInDuration;

                AnimationSet animation = new AnimationSet(false); // change to false
                animation.AddAnimation(fadeIn);
                animation.RepeatCount = 1;

                animation.AnimationEnd += (sender, args) =>
                {
                    if (imageColorView != null) imageColorView.SetBackgroundColor(Color.Transparent);

                    imageView.Visibility = ViewStates.Visible;
                };

                imageView.Animation = animation;
            }
            else
            {
                imageView.SetImageDrawable(drawable);

                if (imageColorView != null) imageColorView.SetBackgroundColor(Color.Transparent);
            }
        }
        
        public static void CrossfadeImage(ImageView imageView, Bitmap bitmap, View imageColorView = null, bool crossfade = true)
        {
            if (crossfade)
            {
                var imageId = (string)imageView.Tag;
                int fadeInDuration; // Configure time values here

                fadeInDuration = 500;

                imageView.Visibility = ViewStates.Invisible;
                //Visible or invisible by default - this will apply when the animation ends

                imageView.SetImageBitmap(bitmap);

                Animation fadeIn = new AlphaAnimation(0, 1);
                fadeIn.Interpolator = new DecelerateInterpolator(); // add this
                fadeIn.Duration = fadeInDuration;

                AnimationSet animation = new AnimationSet(false); // change to false
                animation.AddAnimation(fadeIn);
                animation.RepeatCount = 1;

                animation.AnimationEnd += (sender, args) =>
                {
                    if (imageColorView != null) imageColorView.SetBackgroundColor(Color.Transparent);

                    imageView.Visibility = ViewStates.Visible;
                };

                imageView.Animation = animation;
            }
            else
            {
                imageView.SetImageBitmap(bitmap);

                if (imageColorView != null) imageColorView.SetBackgroundColor(Color.Transparent);

                imageView.Visibility = ViewStates.Visible;
            }
        }

        public static void ClearImageView(ImageView image)
        {
            var prevBitmap = image.Drawable as BitmapDrawable;

            image.DestroyDrawingCache();

            image.Visibility = ViewStates.Visible;

            if (prevBitmap != null)
            {
                if (prevBitmap.Bitmap != null)
                {
                    prevBitmap.Bitmap.Recycle();
                    prevBitmap.Bitmap.Dispose();
                    image.SetImageBitmap(null);
                }

                prevBitmap.SetCallback(null);

                prevBitmap.InvalidateSelf();

                image.InvalidateDrawable(prevBitmap);
                
                prevBitmap.Dispose();

                image.SetImageDrawable(null);

                image.Invalidate();

                if (image.Animation != null)
                {
                    (image.Animation as AnimationSet).Duration = 0;
                }
            }

            image.SetImageResource(Resource.Color.transparent);
        }

        public static int GetThumbnailImageSize(int size)
        {
            //size = size/2;

            return size;
        }

        public class CircleDrawable : Drawable
        {
            Bitmap bmp;
            BitmapShader bmpShader;
            Paint paint;
            RectF oval;

            public CircleDrawable(Bitmap bmp)
            {
                this.bmp = bmp;
                this.bmpShader = new BitmapShader(bmp, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
                this.paint = new Paint() { AntiAlias = true };
                this.paint.SetShader(bmpShader);
                this.oval = new RectF();
            }

            public override void Draw(Canvas canvas)
            {
                canvas.DrawOval(oval, paint);
            }

            protected override void OnBoundsChange(Rect bounds)
            {
                base.OnBoundsChange(bounds);

                var left = 0;
                var top = 0;

                if (bounds.Width() > bounds.Height())
                {
                    left = (bounds.Width() - bounds.Height()) / 2;
                }
                else
                {
                    top = (bounds.Height() - bounds.Width()) / 2;
                }

                oval.Set(left, top, bounds.Width() - left, bounds.Height() - top);
            }

            public override int IntrinsicWidth
            {
                get
                {
                    return bmp.Width;
                }
            }

            public override int IntrinsicHeight
            {
                get
                {
                    return bmp.Height;
                }
            }

            public override void SetAlpha(int alpha)
            {

            }

            public override int Opacity
            {
                get
                {
                    return (int)Format.Opaque;
                }
            }

            public override void SetColorFilter(ColorFilter cf)
            {

            }
        }
    }
}
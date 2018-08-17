using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Util;
using Android.Views;
using Presentation.Activities.Base;

namespace Presentation.Utils
{
    public class ActivityUtils
    {
        public static void StartActivityWithAnimation(Context context, Intent intent, View view)
        {
            if (view != null && Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                var animationBundle = CreateAnimationBundle(view);
                context.StartActivity(intent, animationBundle);
            }
            else
            {
                context.StartActivity(intent);
            }
        }

        public static void StartActivityForResultWithAnimation(Activity activity, int requestCode, Intent intent, View view)
        {
            if (view != null && Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
            {
                var animationBundle = CreateAnimationBundle(view);
                activity.StartActivityForResult(intent, requestCode, animationBundle);
            }
            else
            {
                activity.StartActivityForResult(intent, requestCode);
            }
        }

        public static Bundle CreateAnimationBundle(View view)
        {
            return ActivityOptions.MakeScaleUpAnimation(view, 0, 0, view.Width, view.Height).ToBundle();
        }

        public static void StartActivityWithAnimation(Activity activity, Intent intent, params Pair[] pairs)
        {
            if (pairs != null && pairs.Length > 0 && Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                var options = ActivityOptionsCompat.MakeSceneTransitionAnimation(activity, pairs);
                activity.StartActivity(intent, options.ToBundle());
            }
            else if (pairs != null && pairs.Length > 0)
            {
                StartActivityWithAnimation(activity, intent, (View) pairs[0].First);
            }
            else
            {
                StartActivityWithAnimation(activity, intent, null);
            }
        }
    }
}

using System.Collections.Generic;

using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Java.Lang;
using Presentation.Util;
using Math = System.Math;

namespace Presentation.Activities.Base
{
    [Activity(Theme = "@style/ApplicationThemeFullyTransparent")]
    public class WindowOverlayActivity : LoyaltyFragmentActivity, Drawable.ICallback
    {
        private int alpha;
        private int oldActionBarAlpha = 255;
        private bool actionBarWasHidden = false;
        private bool isBigScreen;

        private float leftDrawerOffset = 0;
        private float rightDrawerOffset = 0;

        protected override void OnCreate(Bundle bundle)
        {
            //TransparentActionBar = true;

            //dont want effect on tablets
            isBigScreen = Resources.GetBoolean(Resource.Boolean.BigScreen);
            
            base.OnCreate(bundle);

            if (!isBigScreen)
            {
                FindViewById(ContentId).SetPadding(0, 0, 0, 0);

                if (bundle == null)
                {
                    SetActionBarAlpha(0);
                }
                else
                {
                    SetActionBarAlpha(bundle.GetInt(BundleConstants.Alpha));
                }

                if (Build.VERSION.SdkInt < BuildVersionCodes.JellyBeanMr1)
                {
                    ActionBarBackgroundDrawable.Callback = this;
                }
            }
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutInt(BundleConstants.Alpha, alpha);

            base.OnSaveInstanceState(outState);
        }

        public override void OnDrawerClosed(View drawerView)
        {
            base.OnDrawerClosed(drawerView);

            if (!isBigScreen)
            {
                oldActionBarAlpha = 255;

                if (actionBarWasHidden)
                {
                    SupportActionBar.Hide();
                    actionBarWasHidden = false;
                }
            }
        }

        public override void OnDrawerOpened(View drawerView)
        {
            base.OnDrawerOpened(drawerView);
        }

        public override void OnDrawerSlide(View drawerView, float slideOffset)
        {
            base.OnDrawerSlide(drawerView, slideOffset);

            if (!isBigScreen)
            {
                if (drawerView.Id == LeftDrawerId)
                {
                    leftDrawerOffset = slideOffset;
                }
                else
                {
                    rightDrawerOffset = slideOffset;
                }

                if (!actionBarWasHidden && !SupportActionBar.IsShowing)
                {
                    actionBarWasHidden = true;
                    SupportActionBar.Show();
                }

                var newAlpha = Math.Max(Math.Max(leftDrawerOffset, rightDrawerOffset)*(float) 255, GetActionBarAlpha());

                Utils.LogUtils.Log(newAlpha.ToString());

                SetActionBarAlpha();
            }
        }

        public override void OnDrawerStateChanged(int newState)
        {
            base.OnDrawerStateChanged(newState);

            if (!isBigScreen)
            {
                if (newState == Android.Support.V4.Widget.DrawerLayout.StateDragging && oldActionBarAlpha == 255)
                {
                    oldActionBarAlpha = GetActionBarAlpha();
                }
            }
        }


        public void InvalidateDrawable(Drawable who)
        {
            SupportActionBar.SetBackgroundDrawable(who);
        }

        public void ScheduleDrawable(Drawable who, IRunnable what, long when)
        {
        }

        public void UnscheduleDrawable(Drawable who, IRunnable what)
        {
        }

        public void SetActionBarAlpha(int newAlpha)
        {
            if (!isBigScreen)
            {
                alpha = newAlpha;
                SetActionBarAlpha();
            }
        }

        private void SetActionBarAlpha()
        {
            if (!isBigScreen)
            {
                var newAlpha = Math.Max(Math.Max(leftDrawerOffset, rightDrawerOffset)*(float) 255, GetActionBarAlpha());

                //SetActionBarColor(Utils.ImageUtils.BlendColors(new Color(0, 0, 0, 50), ActionBarColor, newAlpha / 255), StatusbarColor);
                ActionBarBackgroundDrawable.SetAlpha((int) newAlpha);
            }
        }

        public int GetActionBarAlpha()
        {
            return alpha;
        }
    }
}
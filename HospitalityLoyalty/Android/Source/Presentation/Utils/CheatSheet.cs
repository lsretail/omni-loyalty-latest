using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Presentation.Utils
{
    public class CheatSheet
    {
        /**
         * The estimated height of a toast, in dips (density-independent pixels). This is used to
         * determine whether or not the toast should appear above or below the UI element.
         */
        private const int EstimatedToastHeightDips = 48;

        public static bool ShowCheatSheet(View view, string text) {
            if (string.IsNullOrEmpty(text)) {
                return false;
            }
 
            int[] screenPos = new int[2]; // origin is device display
            Rect displayFrame = new Rect(); // includes decorations (e.g. status bar)
            view.GetLocationOnScreen(screenPos);
            view.GetWindowVisibleDisplayFrame(displayFrame);
 
            Context context = view.Context;
            int viewWidth = view.Width;
            int viewHeight = view.Height;
            int viewCenterX = screenPos[0] + viewWidth / 2;
            int screenWidth = context.Resources.DisplayMetrics.WidthPixels;
            int estimatedToastHeight = (int) (EstimatedToastHeightDips * context.Resources.DisplayMetrics.Density);
 
            Toast cheatSheet = Toast.MakeText(context, text, ToastLength.Short);
            bool showBelow = screenPos[1] < estimatedToastHeight;
            if (showBelow) {
                // Show below
                // Offsets are after decorations (e.g. status bar) are factored in
                cheatSheet.SetGravity(GravityFlags.Top | GravityFlags.CenterHorizontal, viewCenterX - screenWidth / 2, screenPos[1] - displayFrame.Top + viewHeight);
            } else {
                // Show above
                // Offsets are after decorations (e.g. status bar) are factored in
                // NOTE: We can't use Gravity.BOTTOM because when the keyboard is up
                // its height isn't factored in.
                cheatSheet.SetGravity(GravityFlags.Top | GravityFlags.CenterHorizontal,
                        viewCenterX - screenWidth / 2,
                        screenPos[1] - displayFrame.Top - estimatedToastHeight);
            }
 
            cheatSheet.Show();
            return true;
        }
    }
}
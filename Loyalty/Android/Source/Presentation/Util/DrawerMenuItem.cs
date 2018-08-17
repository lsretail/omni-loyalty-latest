using System;
using Android.Graphics;
using Presentation.Activities.Base;

namespace Presentation.Util
{
    public class DrawerMenuItem
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Image { get; set; }
        public bool Color { get; set; }
        public bool Enabled { get; set; }
        public bool IsLoading { get; set; }
        public Color? Accent { get; set; }
        public Color? Background { get; set; }
        public int ActivityType { get; set; }

        public DrawerMenuItem()
        {
            Title = string.Empty;
            SubTitle = string.Empty;
            Enabled = true;
            ActivityType = LoyaltyFragmentActivity.ActivityTypes.None;
        }
    }

    public class SecondaryActionDrawerMenuItem : DrawerMenuItem
    {
        public Action SecondaryAction { get; set; }
        public int SecondaryActionResource { get; set; }
    }

    public class SecondaryTextDrawerMenuItem : DrawerMenuItem
    {
        public string SecondaryText { get; set; }
    }
}
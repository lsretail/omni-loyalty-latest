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
using Presentation.Activities.Base;

namespace Presentation.Utils
{
    public class DrawerMenuItem
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public int Image { get; set; }
        public bool Color { get; set; }
        public bool Enabled { get; set; }
        public bool IsLoading { get; set; }
        public Color? Background { get; set; }
        public HospActivity.ActivityTypes ActivityType { get; set; }

        public DrawerMenuItem()
        {
            Title = string.Empty;
            SubTitle = string.Empty;
            Enabled = true;
            ActivityType = HospActivity.ActivityTypes.None;
        }
    }
}
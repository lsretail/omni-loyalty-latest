using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Activities.Base;

namespace Presentation.Activities.SlideControl
{
    public abstract class SlideControlBaseFragment : BaseFragment
    {
        public abstract void SetSupportActionBar();
    }
}
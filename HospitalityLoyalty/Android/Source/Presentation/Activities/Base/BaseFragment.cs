using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Presentation.Utils;

namespace Presentation.Activities.Base
{
    public class BaseFragment : Fragment
    {

        protected View Inflate(LayoutInflater inflater, int resourceId, ViewGroup root = null, bool tryAgain = true)
        {
            return Utils.Utils.ViewUtils.Inflate(inflater, resourceId, root, tryAgain);
        }

        public override void OnPause()
        {
            //System.GC.Collect();

            base.OnPause();
        }
    }
}
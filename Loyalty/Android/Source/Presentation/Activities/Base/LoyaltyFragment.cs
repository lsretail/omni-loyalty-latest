using System;

using Android.Views;
using Fragment = Android.Support.V4.App.Fragment;

namespace Presentation.Activities.Base
{
    public class LoyaltyFragment : Fragment
    {
        protected bool active;

        protected View Inflate(LayoutInflater inflater, int resourceId, ViewGroup root = null, bool tryAgain = true)
        {
            try
            {
                return inflater.Inflate(resourceId, root);
            }
            //catch (OutOfMemoryException oome)
            catch (Exception)
            {
                if (tryAgain)
                {
                    GC.Collect();
                    return Inflate(inflater, resourceId, root, false);
                }
                throw;
            }
        }

        public override void OnStart()
        {
            base.OnStart();

            active = true;
        }

        public override void OnStop()
        {
            active = false;

            base.OnStop();
        }
    }
}
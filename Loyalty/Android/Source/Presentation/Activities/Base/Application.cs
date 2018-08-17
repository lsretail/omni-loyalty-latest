using System;

using Android.App;
using Android.Runtime;

namespace Presentation.Activities.Base
{
    [Application()]
    public class Application : Android.App.Application
    {
        public Application()
        {
        }

        protected Application(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }


        public override void OnTerminate()
        {
            base.OnTerminate();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace Presentation.Activities.Base
{
    public abstract class HospActivityNoStatusBar : HospActivity
    {
        public Toolbar Toolbar { get; private set; }

        public override void SetSupportActionBar(Toolbar toolbar)
        {
            if(toolbar == null)
                return;

            base.SetSupportActionBar(toolbar);

            this.Toolbar = toolbar;

            //toolbar.Title = Title;
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            //SetSupportActionBar(Toolbar);

            return base.OnCreateOptionsMenu(menu);
        }
    }
}
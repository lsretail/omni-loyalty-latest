using System;
using System.Collections.Generic;

using Android.Content;
using Android.Widget;

namespace Presentation.Adapters
{
    class EmptyAdapter : ArrayAdapter
    {
        public EmptyAdapter(Context context, string message)
            : base(context, Android.Resource.Layout.SimpleListItem1, new List<string>(){message})
        {
        }
    }
}
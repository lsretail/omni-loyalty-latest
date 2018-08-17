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

namespace Presentation.Utils
{
    public class JavaWrapper<T> : Java.Lang.Object
    {
        public JavaWrapper(T managedObject)
        {
            this.Value = managedObject;
        }

        public T Value { get; private set; }
    }
}
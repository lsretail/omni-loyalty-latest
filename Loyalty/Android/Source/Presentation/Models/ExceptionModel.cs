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
using Presentation.Util;

namespace Presentation.Models
{
    public class ExceptionModel : BaseModel
    {
        public ExceptionModel(Context context) : base(context, null)
        {
        }

        protected override void CreateService()
        {
        }
    }
}
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
    public interface IItemClickListener
    {
        void ItemClicked(ItemType type, string id, string id2, int itemType, View view, string animationImageId = "");
    }
}
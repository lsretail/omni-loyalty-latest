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
    public enum SectionTypes
    {
        Ingredient = 0,
        ProductGroup = 1,
        DealGroup = 2,
        MenuItem = 3
    }

    public enum LineTypes
    {
        Header = 0,
        Normal = 1,
        CheckBox = 2,
        Counter = 3,
        Radio = 4
    }

    public class SectionedListItem
    {
        public SectionTypes SectionType { get; set; }

        public SectionedListItem()
        {
        }
    }

    public class SectionedListHeaderItem : SectionedListItem
    {
        public string Description { get; set; }

        public SectionedListHeaderItem()
        {
            Description = string.Empty;
        }
    }

    public class SectionListLineItem : SectionedListItem
    {
        public Object Item { get; set; }
        public Object ItemParent { get; set; }
        public bool Selected { get; set; }

        public SectionListLineItem()
        {
        }
    }
}
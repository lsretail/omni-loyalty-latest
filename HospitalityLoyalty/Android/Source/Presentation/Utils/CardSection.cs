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
    public class CardSection<T>
    {
        public CardSection()
        {
            Items = new List<CardItem<T>>();
        }

        public bool HasHeader { get; set; }
        public CardHeader<T> Header { get; set; }
        public List<CardItem<T>> Items { get; set; }
    }
}
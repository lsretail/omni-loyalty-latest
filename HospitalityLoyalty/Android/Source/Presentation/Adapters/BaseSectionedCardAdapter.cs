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
using Presentation.Utils;

namespace Presentation.Adapters
{
    public abstract class BaseSectionedCardAdapter<T> : BaseAdapter<Card<T>>, View.IOnClickListener
    {
        public enum LineType
        {
            LoadingIndicator = 0,
            Header = 1,
            SmallExtraSmallTwoCards = 2,
            SmallCards = 3,
            MediumSmallCards = 4,
            MediumWrappedCards = 5,
            MediumSmallCardAndSmallCards = 6,
            MediumCardAndSmallCards = 7,
            SingleWrappedCard = 8
        }

        private int lineCount;
        private readonly Dictionary<LineType, int> cardInRowType;
        private readonly Dictionary<int, int> rowStartingItem;
        private int[] cardIds;
        private bool isLoading = false;

        protected IItemClickListener Listener { get; private set; }
        protected Context Context { get; private set; }
        protected List<LineType> LineTypes { get; private set; }
        protected int Width { get; private set; }
        protected List<CardSection<T>> Items { get; set; }
        protected bool HasBackground { get; set; }
        protected bool HasSelectableBackground { get; set; }

        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                isLoading = value;
                NotifyDataSetChanged();
            }
        }

        protected BaseSectionedCardAdapter(IItemClickListener listener, Context context)
        {
            HasSelectableBackground = true;
            
            this.Listener = listener;
            this.Context = context;
            this.LineTypes = new List<LineType>();
            cardInRowType = new Dictionary<LineType, int>();
            rowStartingItem = new Dictionary<int, int>();

            cardIds = new int[]
                {
                    Resource.Id.CardOne,
                    Resource.Id.CardTwo,
                    Resource.Id.CardThree,
                    Resource.Id.CardFour,
                    Resource.Id.CardFive,
                    Resource.Id.CardSix,
                    Resource.Id.CardSeven,
                    Resource.Id.CardEight,
                    Resource.Id.CardNine,
                    Resource.Id.CardTen,
                    Resource.Id.CardEleven
                };

            rowStartingItem[0] = 0;

            var windowManager = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            Display display = windowManager.DefaultDisplay;

            Width = display.Width;

            LineTypes.Add(LineType.LoadingIndicator);
            LineTypes.Add(LineType.Header);
        }

        protected int NumberOfCardsInRow(LineType type)
        {
            LogUtils.Log("checking cards for row: " + type);
            if (!cardInRowType.ContainsKey(type))
            {
                var padding = Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardMargin);
                var smallCardWidth = Context.Resources.GetDimensionPixelSize(Resource.Dimension.SmallSmallCardWidth);
                var mediumCardWidth = Context.Resources.GetDimensionPixelSize(Resource.Dimension.MediumCardWidth);
                var numberOfCards = 0;
                var minCards = 1;

                var currentWidth = padding;

                if (type == LineType.SmallExtraSmallTwoCards)
                {
                    numberOfCards = 2;
                }
                else if (type == LineType.SmallCards)
                {
                    minCards = 2;

                    while (currentWidth <= Width)
                    {
                        numberOfCards++;
                        currentWidth += smallCardWidth;
                        currentWidth += padding;
                    }

                    numberOfCards--;
                }
                else if (type == LineType.MediumSmallCards)
                {
                    while (currentWidth <= Width)
                    {
                        numberOfCards++;
                        currentWidth += mediumCardWidth;
                        currentWidth += padding;
                    }

                    numberOfCards--;
                }
                else if (type == LineType.MediumWrappedCards)
                {
                    //return 1 if tablet
                    while (currentWidth <= Width)
                    {
                        numberOfCards++;
                        currentWidth += mediumCardWidth;
                        currentWidth += padding;
                    }

                    numberOfCards--;
                }
                else if (type == LineType.MediumSmallCardAndSmallCards)
                {
                    currentWidth += mediumCardWidth;
                    currentWidth += padding;

                    while (currentWidth <= Width)
                    {
                        numberOfCards++;
                        currentWidth += smallCardWidth;
                        currentWidth += padding;
                    }
                }
                else if (type == LineType.MediumCardAndSmallCards)
                {
                    currentWidth += mediumCardWidth;
                    currentWidth += padding;

                    while (currentWidth <= Width)
                    {
                        numberOfCards++;
                        numberOfCards++;
                        currentWidth += smallCardWidth;
                        currentWidth += padding;
                    }

                    numberOfCards = numberOfCards - 1;
                }

                cardInRowType[type] = Math.Max(minCards, numberOfCards);
            }

            return cardInRowType[type];
        }

        public override int Count
        {
            get
            {
                if (lineCount == 0)
                {
                    var count = 0;

                    if (Items != null)
                    {
                        foreach (var cardSection in Items)
                        {
                            if (cardSection.HasHeader)
                            {
                                rowStartingItem[count + 1] = rowStartingItem[count] + 1;

                                count++;
                            }

                            var sectionCount = 0;

                            while (cardSection.Items != null && sectionCount < cardSection.Items.Count)
                            {
                                var currentLineCount = NumberOfCardsInRow(LineTypes[GetItemViewType(count)]);
                                sectionCount += currentLineCount;

                                if (sectionCount > cardSection.Items.Count)
                                {
                                    rowStartingItem[count + 1] = rowStartingItem[count] + sectionCount - cardSection.Items.Count;
                                }
                                else
                                {
                                    rowStartingItem[count + 1] = rowStartingItem[count] + currentLineCount;
                                }

                                count++;
                            }
                        }
                    }

                    lineCount = count;
                }

                return isLoading ? lineCount + 1 : lineCount;
            }
        }

        public override bool AreAllItemsEnabled()
        {
            return false;
        }

        public override bool IsEnabled(int position)
        {
            return false;
        }

        protected Card<T> GetItem(string tag)
        {
            var row = GetRowFromTag(tag);
            var col = GetColFromTag(tag);

            return GetItem(row, col) as CardItem<T>;
        }

        protected Card<T> GetItem(int row, int col)
        {
            var pos = rowStartingItem[row] + col;

            if (rowStartingItem.ContainsKey(row + 1))
            {
                if (pos >= rowStartingItem[row + 1])
                {
                    return null;
                }
            }

            return this[pos];
        }

        public override Card<T> this[int position]
        {
            get
            {
                /*if (Items != null && position < Items.Count)
                    return Items[position];*/
                if (Items != null)
                {
                    var count = 0;

                    foreach (var cardSection in Items)
                    {
                        if (cardSection.HasHeader && count == position)
                        {
                            return cardSection.Header;
                        }
                        else
                        {
                            if (cardSection.HasHeader)
                            {
                                count++;
                            }

                            foreach (var cardItem in cardSection.Items)
                            {
                                if (position == count)
                                {
                                    return cardItem;
                                }

                                count++;
                            }
                        }
                    }
                }
                return null;
            }
        }

        private void InflateAndFillChildView(int row, int col, LayoutInflater inflater, View cardView, Card<T>.CardType cardType)
        {
            var card = GetItem(row, col);

            LinearLayout parent = null;

            if (cardView != null)
            {
                if (cardType == Card<T>.CardType.Small)
                {
                    parent = cardView.FindViewById<LinearLayout>(Resource.Id.CardSmallBaseContainer);
                }
                else if (cardType == Card<T>.CardType.MediumSmall)
                {
                    parent = cardView.FindViewById<LinearLayout>(Resource.Id.CardMediumSmallBaseContainer);
                }
                else if (cardType == Card<T>.CardType.Medium)
                {
                    parent = cardView.FindViewById<LinearLayout>(Resource.Id.CardMediumBaseContainer);
                }
                else if (cardType == Card<T>.CardType.MediumWrapped)
                {
                    parent = cardView.FindViewById<LinearLayout>(Resource.Id.CardMediumWrappedBaseContainer);
                }
                else if (cardType == Card<T>.CardType.Single)
                {
                    parent = cardView.FindViewById<LinearLayout>(Resource.Id.CardSingleWrappedBaseContainer);
                }

                if (parent != null)
                {
                    if (!HasSelectableBackground)
                    {
                        if (parent.Parent is FrameLayout)
                        {
                            (parent.Parent as FrameLayout).Foreground = Context.Resources.GetDrawable(Resource.Color.transparent);
                        }
                    }
                    /*if (card == null)
                    {
                        parent.Tag = (int)Card<T>.CardContentType.None;
                    }*/
                    //else
                    if(card != null)
                    {
                        if ((int)parent.Tag != (int)card.ContentType)
                        {
                            parent.RemoveAllViews();
                            parent.Tag = (int)card.ContentType;

                            switch (card.ContentType)
                            {
                                case Card<T>.CardContentType.MenuItem:
                                    if (cardType == Card<T>.CardType.Small)
                                    {
                                        inflater.Inflate(Resource.Layout.CardMenuItemSmall, parent);
                                    }
                                    else
                                    {
                                        inflater.Inflate(Resource.Layout.CardMenuItem, parent);
                                    }
                                    break;

                                case Card<T>.CardContentType.MenuGroup:
                                    inflater.Inflate(Resource.Layout.CardMenuGroup, parent);
                                    break;

                                case Card<T>.CardContentType.Map:
                                    inflater.Inflate(Resource.Layout.CardMap, parent);
                                    break;

                                case Card<T>.CardContentType.Store:
                                    inflater.Inflate(Resource.Layout.CardStore, parent);
                                    break;

                                case Card<T>.CardContentType.Transaction:
                                    inflater.Inflate(Resource.Layout.CardTransaction, parent);
                                    break;

                                case Card<T>.CardContentType.TransactionDetail:
                                    inflater.Inflate(Resource.Layout.CardTransactionDetail, parent);
                                    break;

                                case Card<T>.CardContentType.Coupon:
                                    if (cardType == Card<T>.CardType.Single)
                                    {
                                        inflater.Inflate(Resource.Layout.CardCouponWrapped, parent);
                                    }
                                    else
                                    {
                                        inflater.Inflate(Resource.Layout.CardCoupon, parent);
                                    }
                                    break;

                                case Card<T>.CardContentType.Offer:
                                    if (cardType == Card<T>.CardType.Single)
                                    {
                                        inflater.Inflate(Resource.Layout.CardOfferWrapped, parent);
                                    }
                                    else
                                    {
                                        inflater.Inflate(Resource.Layout.CardOffer, parent);
                                    }
                                    break;

                                case Card<T>.CardContentType.BasketItem:
                                    inflater.Inflate(Resource.Layout.CardBasketItem, parent);
                                    break;

                                case Card<T>.CardContentType.Hint:
                                    inflater.Inflate(Resource.Layout.CardHint, parent);
                                    break;
                            }
                        }
                    }
                }
            }

            FillCard(row, col, cardType, cardView);
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LogUtils.Log("inflating row: " + position);

            var inflater = ((LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService));
            var type = LineTypes[GetItemViewType(position)];

            if (type == LineType.LoadingIndicator)
            {
                if (convertView == null)
                {
                    convertView = inflater.Inflate(Resource.Layout.CardLoading, null);
                }
            }
            else if (type == LineType.Header)
            {
                if (convertView == null)
                {
                    convertView = inflater.Inflate(Resource.Layout.CardDisabledSectionHeader, null);
                }

                var header = GetItem(position, 0);

                if (header is CardHeader<T>)
                {
                    convertView.FindViewById<TextView>(Resource.Id.CardDisabledSectionHeaderDescription).Text = (header as CardHeader<T>).Description.ToUpper();
                }
            }
            else
            {
                var count = NumberOfCardsInRow(type);

                LogUtils.Log("GetView Start");

                switch (type)
                {
                    case LineType.SmallExtraSmallTwoCards:
                        if (convertView == null)
                        {
                            convertView = inflater.Inflate(Resource.Layout.CardLayoutExtraSmallTwoCard, null);
                        }
                        FillCard(position, 0, Card<T>.CardType.SmallExtraSmall, convertView.FindViewById(Resource.Id.CardLayoutExtraSmallCardOne));
                        FillCard(position, 1, Card<T>.CardType.SmallExtraSmall, convertView.FindViewById(Resource.Id.CardLayoutExtraSmallCardTwo));
                        break;
                    case LineType.SmallCards:
                        if (convertView == null)
                        {
                            convertView = new LinearLayout(Context);

                            convertView.LayoutParameters = new AbsListView.LayoutParams(AbsListView.LayoutParams.FillParent, AbsListView.LayoutParams.WrapContent);
                            convertView.SetPadding(Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0, Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0);
                            (convertView as LinearLayout).Orientation = Orientation.Horizontal;

                            for (int i = 0; i < count; i++)
                            {
                                var smallView = inflater.Inflate(Resource.Layout.CardSmallBase, convertView as LinearLayout, false);
                                smallView.Id = cardIds[i];

                                (convertView as LinearLayout).AddView(smallView);
                            }
                        }


                        InflateAndFillChildView(position, 0, inflater, convertView.FindViewById(cardIds[0]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 1, inflater, convertView.FindViewById(cardIds[1]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 2, inflater, convertView.FindViewById(cardIds[2]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 3, inflater, convertView.FindViewById(cardIds[3]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 4, inflater, convertView.FindViewById(cardIds[4]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 5, inflater, convertView.FindViewById(cardIds[5]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 6, inflater, convertView.FindViewById(cardIds[6]), Card<T>.CardType.Small);
                        break;
                    case LineType.MediumSmallCardAndSmallCards:
                        if (convertView == null)
                        {
                            convertView = new LinearLayout(Context);

                            convertView.LayoutParameters = new AbsListView.LayoutParams(AbsListView.LayoutParams.FillParent, AbsListView.LayoutParams.WrapContent);
                            convertView.SetPadding(Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0, Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0);
                            (convertView as LinearLayout).Orientation = Orientation.Horizontal;

                            var mediumSmallView = inflater.Inflate(Resource.Layout.CardMediumSmallBase, convertView as LinearLayout, false);
                            mediumSmallView.Id = cardIds[0];

                            (convertView as LinearLayout).AddView(mediumSmallView);

                            for (int i = 1; i < count; i++)
                            {
                                var smallView = inflater.Inflate(Resource.Layout.CardSmallBase, convertView as LinearLayout, false);
                                smallView.Id = cardIds[i];

                                (convertView as LinearLayout).AddView(smallView);
                            }
                        }

                        InflateAndFillChildView(position, 0, inflater, convertView.FindViewById(cardIds[0]), Card<T>.CardType.MediumSmall);
                        InflateAndFillChildView(position, 1, inflater, convertView.FindViewById(cardIds[1]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 2, inflater, convertView.FindViewById(cardIds[2]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 3, inflater, convertView.FindViewById(cardIds[3]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 4, inflater, convertView.FindViewById(cardIds[4]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 5, inflater, convertView.FindViewById(cardIds[5]), Card<T>.CardType.Small);
                        break;
                    case LineType.MediumCardAndSmallCards:
                        if (convertView == null)
                        {
                            convertView = new LinearLayout(Context);

                            convertView.LayoutParameters = new AbsListView.LayoutParams(AbsListView.LayoutParams.FillParent, AbsListView.LayoutParams.WrapContent);
                            convertView.SetPadding(Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0, Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0);
                            (convertView as LinearLayout).Orientation = Orientation.Horizontal;

                            var mediumView = inflater.Inflate(Resource.Layout.CardMediumBase, (convertView as LinearLayout), false);
                            mediumView.Id = cardIds[0];

                            (convertView as LinearLayout).AddView(mediumView);

                            var rowCount = (int)Math.Ceiling(((double)(count - 1)) / 2);

                            var layout = new LinearLayout(Context);
                            layout.LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, Context.Resources.GetInteger(Resource.Integer.MediumCardWeight));
                            layout.Orientation = Orientation.Vertical;

                            var rowOne = new LinearLayout(Context);
                            rowOne.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent);
                            rowOne.Orientation = Orientation.Horizontal;

                            var rowTwo = new LinearLayout(Context);
                            rowTwo.LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent);
                            rowTwo.Orientation = Orientation.Horizontal;

                            for (int i = 0; i < rowCount; i++)
                            {
                                var smallView = inflater.Inflate(Resource.Layout.CardSmallBase, rowOne, false);
                                smallView.Id = cardIds[i + 1];

                                rowOne.AddView(smallView);


                                smallView = inflater.Inflate(Resource.Layout.CardSmallBase, rowTwo, false);
                                smallView.Id = cardIds[i + rowCount + 1];

                                rowTwo.AddView(smallView);
                            }

                            var rowContainer = new LinearLayout(Context);

                            rowContainer.LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.WrapContent, Context.Resources.GetInteger(Resource.Integer.SmallSmallCardWeight) * rowCount);
                            rowContainer.Orientation = Orientation.Vertical;

                            rowContainer.AddView(rowOne);
                            rowContainer.AddView(rowTwo);

                            (convertView as LinearLayout).AddView(rowContainer);
                        }


                        InflateAndFillChildView(position, 0, inflater, convertView.FindViewById(cardIds[0]), Card<T>.CardType.Medium);
                        InflateAndFillChildView(position, 1, inflater, convertView.FindViewById(cardIds[1]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 2, inflater, convertView.FindViewById(cardIds[2]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 3, inflater, convertView.FindViewById(cardIds[3]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 4, inflater, convertView.FindViewById(cardIds[4]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 5, inflater, convertView.FindViewById(cardIds[5]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 6, inflater, convertView.FindViewById(cardIds[6]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 7, inflater, convertView.FindViewById(cardIds[7]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 8, inflater, convertView.FindViewById(cardIds[8]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 9, inflater, convertView.FindViewById(cardIds[9]), Card<T>.CardType.Small);
                        InflateAndFillChildView(position, 10, inflater, convertView.FindViewById(cardIds[10]), Card<T>.CardType.Small);

                        break;
                    case LineType.MediumSmallCards:
                        if (convertView == null)
                        {
                            convertView = new LinearLayout(Context);

                            convertView.LayoutParameters = new AbsListView.LayoutParams(AbsListView.LayoutParams.FillParent, AbsListView.LayoutParams.WrapContent);
                            convertView.SetPadding(Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0, Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0);
                            (convertView as LinearLayout).Orientation = Orientation.Horizontal;

                            for (int i = 0; i < count; i++)
                            {
                                var mediumSmallView = inflater.Inflate(Resource.Layout.CardMediumSmallBase, convertView as LinearLayout, false);
                                mediumSmallView.Id = cardIds[i];

                                (convertView as LinearLayout).AddView(mediumSmallView);
                            }
                        }

                        InflateAndFillChildView(position, 0, inflater, convertView.FindViewById(cardIds[0]), Card<T>.CardType.MediumSmall);
                        InflateAndFillChildView(position, 1, inflater, convertView.FindViewById(cardIds[1]), Card<T>.CardType.MediumSmall);
                        InflateAndFillChildView(position, 2, inflater, convertView.FindViewById(cardIds[2]), Card<T>.CardType.MediumSmall);

                        break;
                    case LineType.MediumWrappedCards:
                        if (convertView == null)
                        {
                            convertView = new LinearLayout(Context);

                            convertView.LayoutParameters = new AbsListView.LayoutParams(AbsListView.LayoutParams.FillParent, AbsListView.LayoutParams.WrapContent);
                            convertView.SetPadding(Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0, Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardHalfMargin), 0);
                            (convertView as LinearLayout).Orientation = Orientation.Horizontal;

                            for (int i = 0; i < count; i++)
                            {
                                var view = inflater.Inflate(Resource.Layout.CardMediumWrappedBase, convertView as LinearLayout, false);
                                view.Id = cardIds[i];

                                (convertView as LinearLayout).AddView(view);
                            }
                        }

                        InflateAndFillChildView(position, 0, inflater, convertView.FindViewById(cardIds[0]), Card<T>.CardType.MediumWrapped);
                        InflateAndFillChildView(position, 1, inflater, convertView.FindViewById(cardIds[1]), Card<T>.CardType.MediumWrapped);
                        InflateAndFillChildView(position, 2, inflater, convertView.FindViewById(cardIds[2]), Card<T>.CardType.MediumWrapped);

                        break;

                    case LineType.SingleWrappedCard:
                        if (convertView == null)
                        {
                            convertView = inflater.Inflate(Resource.Layout.CardSingleWrappedBase, null);
                            convertView.Id = cardIds[0];
                        }

                        InflateAndFillChildView(position, 0, inflater, convertView.FindViewById(cardIds[0]), Card<T>.CardType.Single);

                        break;
                }
            }


            LogUtils.Log("GetView End");

            if (HasBackground)
            {
                if (position == 0)
                    convertView.SetBackgroundResource(Resource.Drawable.transparent_background_gradient);
                else
                    convertView.SetBackgroundResource(Resource.Color.backgroundcolor);
            }

            return convertView;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override void NotifyDataSetChanged()
        {
            lineCount = 0;

            rowStartingItem.Clear();
            rowStartingItem[0] = 0;

            base.NotifyDataSetChanged();
        }

        public override int ViewTypeCount
        {
            get { return LineTypes.Count; }
        }

        protected int CardWidth(int row, int col)
        {
            var type = LineTypes[GetItemViewType(row)];
            var count = NumberOfCardsInRow(type);

            /*var windowService = (Context.GetSystemService(Context.WindowService) as IWindowManager);
            var width = windowService.DefaultDisplay.Width;*/
            var width = Width;

            var padding = Context.Resources.GetDimensionPixelSize(Resource.Dimension.CardMargin);
            var totalPadding = padding * (count + 1);

            switch (type)
            {
                case LineType.SmallCards:
                case LineType.MediumSmallCards:
                case LineType.SmallExtraSmallTwoCards:
                case LineType.MediumWrappedCards:
                    return (width - totalPadding) / count;

                case LineType.MediumCardAndSmallCards:
                case LineType.MediumSmallCardAndSmallCards:
                    if (col == 0)
                    {
                        return ((width - totalPadding) / count) * 2 + padding;
                    }
                    else
                    {
                        return (width - totalPadding) / count;
                    }
            }

            return width - totalPadding;
        }

        public override int GetItemViewType(int position)
        {
            if (isLoading && (Items == null || Items.Count == 0 || (rowStartingItem[position] >= Items.Count)))
                return LineTypes.IndexOf(LineType.LoadingIndicator);
            if (this[position] is CardHeader<T>)
                return LineTypes.IndexOf(LineType.Header);
            return GetViewType(position);
        }

        protected string GenerateTag(int row, int col)
        {
            return row + ":" + col;
        }

        protected int GetRowFromTag(string tag)
        {
            return Int32.Parse(tag.Split(':')[0]);
        }

        protected int GetColFromTag(string tag)
        {
            return Int32.Parse(tag.Split(':')[1]);
        }

        public abstract void OnClick(View v);
        public abstract int GetViewType(int position);
        public abstract void FillCard(int row, int col, Card<T>.CardType cardType, View view);
    }
}
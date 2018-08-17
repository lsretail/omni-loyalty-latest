using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation
{
    public class TransactionDetailsTableSource : UITableViewSource
    {
        private List<LoySaleLine> tableSaleLines;
        private List<TotalLine> totalLines;
        private LoyTransaction transaction;
        private const string nibName = "TransactionCell";
        private const string cellIdentifier = "TransactionCellID";
        private NSString transactionLineCellIdentifier = (NSString)"TransactionLineCellID";
        private NSString totalLineCellIdentifier = (NSString)"TotalLineCellID";
        private NSString headerFooterCellIdentifier = (NSString)"HeaderFooterLineCellID";

        public delegate void PushToItemDetailEventHandler(LoySaleLine line);
        public PushToItemDetailEventHandler PushToItemDetail;

        public TransactionDetailsTableSource(LoyTransaction transaction)
        {
            SetData(transaction);
        }

        public void SetData(LoyTransaction transaction) 
        {
            this.transaction = transaction;
			tableSaleLines = transaction.SaleLines;

			totalLines = new List<TotalLine>();
			totalLines.Add(new TotalLine()
			{
				Description = LocalizationUtilities.LocalizedString("TransactionView_Net", "Net Total:"),
				Amount = transaction.Amount,
				DividerAbove = true
			});

			foreach (var taxLine in transaction.TaxLines)
			{
				totalLines.Add(new TotalLine()
				{
					Description = string.Format(LocalizationUtilities.LocalizedString("TransactionView_Vat2", "VAT ({0}):"), taxLine.TaxDesription),
					Amount = taxLine.TaxAmount
				});
			}

			totalLines.Add(new TotalLine()
			{
				Description = LocalizationUtilities.LocalizedString("TransactionView_Total", "Total:"),
				Amount = transaction.Amount,
				DividerAbove = true,
			});

			totalLines.Add(new TotalLine()
			{
				Description = LocalizationUtilities.LocalizedString("TransactionView_Discount", "Discount:"),
				Amount = transaction.DiscountAmount,
				DividerAbove = true,
				DividerBelow = true
			});
			foreach (var tenderLine in transaction.TenderLines)
			{
				totalLines.Add(new TotalLine()
				{
					Description = tenderLine.Description,
					Amount = tenderLine.Amount,
				});
			}

			totalLines[totalLines.Count - 1].DividerBelow = true;
        }

        /// <summary>
        /// Called by the TableView to determine how many sections(groups) there are.
        /// </summary>
        public override nint NumberOfSections(UITableView tableView)
        {
            return 4;
        }

        /// <summary>
        /// Called by the TableView to determine how many cells to create for that particular section.
        /// </summary>
        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (section == 0)
            {
                if (transaction.TransactionHeaders != null)
                    return transaction.TransactionHeaders.Count;
                else
                    return 0;
            }
            else if (section == 1)
            {
                if (tableSaleLines != null)
                    return tableSaleLines.Count;
                else
                    return 0;
            }
            else if (section == 2)
            {
                if (totalLines != null)
                    return totalLines.Count;
                else
                    return 0;
            }
            else if (section == 3)
            {
                if (transaction.TransactionFooters != null)
                    return transaction.TransactionFooters.Count;
                else
                    return 0;
            }
            else
            {
                return 0;
            }
        }

        //		//must set the row height, same as in TransactionCell.xib heigth.
        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Section == 0)
            {
                return HeaderFooterCell.CalculateHeight(transaction.TransactionHeaders[indexPath.Row].HeaderDescription);
            }
            else if (indexPath.Section == 1)
            {
                return TransactionItemCell.CalculateHeight(tableSaleLines[indexPath.Row]);
                //the only way I found to get the cell height from nib..
                /*var cell = new TransactionCell();
                var views = NSBundle.MainBundle.LoadNib(nibName, cell, null);
                cell = MonoTouch.ObjCRuntime.Runtime.GetNSObject( views.ValueAt(0) ) as TransactionCell;
                return cell.Frame.Height;//110f;*/
            }
            else if (indexPath.Section == 2)
            {
                return TotalLineCell.CalculateHeight(totalLines[indexPath.Row]);
            }
            else if (indexPath.Section == 3)
            {
                return HeaderFooterCell.CalculateHeight(transaction.TransactionFooters[indexPath.Row].FooterDescription);
            }

            return 0f;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = null;
            if (indexPath.Section == 0)      //HEADER
            {
                var header = transaction.TransactionHeaders[indexPath.Row];

                cell = tableView.DequeueReusableCell(headerFooterCellIdentifier) as HeaderFooterCell;

                if (cell == null)
                {
                    cell = new HeaderFooterCell(headerFooterCellIdentifier);
                }

                (cell as HeaderFooterCell).UpdateCell(header.HeaderDescription);
            }
            else if (indexPath.Section == 1)     //SALE LINES
            {
                LoySaleLine saleLine = this.tableSaleLines[indexPath.Row];

                cell = tableView.DequeueReusableCell(transactionLineCellIdentifier) as TransactionItemCell;

                if (cell == null)
                {
                    cell = new TransactionItemCell(transactionLineCellIdentifier);
                }

                (cell as TransactionItemCell).UpdateCell(saleLine);
            }
            else if (indexPath.Section == 2)     //TOTAL LINES
            {
                var totalLine = totalLines[indexPath.Row];

                cell = tableView.DequeueReusableCell(totalLineCellIdentifier) as TotalLineCell;

                if (cell == null)
                {
                    cell = new TotalLineCell(totalLineCellIdentifier);
                }

                (cell as TotalLineCell).UpdateCell(totalLine);
            }
            else if (indexPath.Section == 3)     //FOOTERS
            {
                var footer = transaction.TransactionFooters[indexPath.Row];

                cell = tableView.DequeueReusableCell(headerFooterCellIdentifier) as HeaderFooterCell;

                if (cell == null)
                {
                    cell = new HeaderFooterCell(headerFooterCellIdentifier);
                }

                (cell as HeaderFooterCell).UpdateCell(footer.FooterDescription);
            }

            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight

            if (indexPath.Section == 1)
            {
                PushToItemDetail?.Invoke(this.tableSaleLines[indexPath.Row]);
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            UIView headerView = null;

            if (section == 0)
            {
                headerView = new UIView(new CGRect(0, 0, tableView.Frame.Width, 44));
                headerView.Add(new UILabel(new CGRect(0, 0, tableView.Frame.Width, 42)) { Text = transaction.DateToShortFormat, TextAlignment = UITextAlignment.Center, TextColor = Utils.AppColors.PrimaryColor, BackgroundColor = UIColor.White });
                headerView.Add(new UIView(new CGRect(0, 42, tableView.Frame.Width, 2)) { BackgroundColor = Utils.AppColors.PrimaryColor });
            }
            else if (section == 1)
            {
                headerView = new UIView(new CGRect(0, 0, tableView.Frame.Width, 23)) { BackgroundColor = UIColor.White };
                headerView.Add(new UIView(new CGRect(0, -1, tableView.Frame.Width, 1)) { BackgroundColor = Utils.AppColors.PrimaryColor });

                headerView.Add(new UILabel()
                {
                    Font = UIFont.FromName("Helvetica", 13f),
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear,
                    TextColor = Utils.AppColors.PrimaryColor,
                    Text = LocalizationUtilities.LocalizedString("TransactionView_ItemName", "Item name"),
                    Frame = new CGRect(15, 0, tableView.Frame.Width / 2, 22)
                });
                headerView.Add(new UILabel()
                {
                    Font = UIFont.FromName("Helvetica", 13f),
                    TextAlignment = UITextAlignment.Left,
                    BackgroundColor = UIColor.Clear,
                    TextColor = Utils.AppColors.PrimaryColor,
                    Text = LocalizationUtilities.LocalizedString("TransactionView_Qty", "Qty"),
                    Frame = new CGRect(180, 0, 45, 22)
                });
                headerView.Add(new UILabel()
                {
                    Font = UIFont.FromName("Helvetica", 13f),
                    TextAlignment = UITextAlignment.Right,
                    BackgroundColor = UIColor.Clear,
                    TextColor = Utils.AppColors.PrimaryColor,
                    Text = LocalizationUtilities.LocalizedString("TransactionView_Amount", "Amount"),
                    Frame = new CGRect(235, 0, 60, 22)
                });
                headerView.Add(new UIView(new CGRect(0, 22, tableView.Frame.Width, 1)) { BackgroundColor = Utils.AppColors.PrimaryColor });
            }
            else
            {
                headerView = new UIView();
            }

            return headerView;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            return GetViewForHeader(tableView, section).Frame.Height;
        }

        public class TotalLine
        {
            public string Description { get; set; }
            public string Amount { get; set; }
            public bool DividerAbove { get; set; }
            public bool DividerBelow { get; set; }
        }
        /*public override UIView GetViewForFooter (UITableView tableView, int sectionIdx)
        {
            TotalsView totalsView = new TotalsView();
            UIView bb= totalsView.View; //must call the view before setting the labels
            totalsView.SetTotal(Utils.Localization.LocalizedString("TransactionView_Total"),transaction.AmountForDisplay);
            totalsView.SetDiscount(Utils.Localization.LocalizedString("TransactionView_Discount"),transaction.DiscountAmount);

            totalsView.SetVat(Utils.Localization.LocalizedString("TransactionView_Vat"),transaction.VatAmount);
            totalsView.SetNet(Utils.Localization.LocalizedString("TransactionView_Net"),transaction.NetAmount);
            string desc = "";
            string amt = "";
            if (transaction.TenderLines != null & transaction.TenderLines.Count > 0)
            {
                foreach(TenderLine tl in transaction.TenderLines)
                {
                    desc += tl.Tender.Description + "\r\n";
                    amt += tl.Tender.Amount + "\r\n";;
                }
            }
            totalsView.SetTender(desc,amt);
            return totalsView.View; 
        }
        
        public override float GetHeightForFooter (UITableView tableView, int sectionIdx)
        {
            TotalsView totalsView = new TotalsView();
            return totalsView.View.Frame.Height;  //60f;
        }*/
    }




    //Transaction Details Cells

    public class HeaderFooterCell : UITableViewCell
    {
        UILabel description;

        public HeaderFooterCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView.BackgroundColor = UIColor.FromRGB(255, 255, 255);

            description = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 15f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Center,
                BackgroundColor = UIColor.Clear
            };

            ContentView.Add(description);
        }

        public void UpdateCell(string descriptionText)
        {
            description.Text = descriptionText;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            //description.Frame = new CGRect(15, 4, 290, 25);
            description.Frame = new CGRect(0, 4, this.ContentView.Frame.Width, 25);
        }

        public static float CalculateHeight(string description)
        {
            return 34;
        }
    }

    public class TransactionItemCell : UITableViewCell
    {
        private LoySaleLine saleLine;
        UILabel description, qty, amount, extraLines;

        public TransactionItemCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView.BackgroundColor = UIColor.FromRGB(255, 255, 255);
            Accessory = UITableViewCellAccessory.DisclosureIndicator;



            description = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 13f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            qty = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 13f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            amount = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 13f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Right,
                BackgroundColor = UIColor.Clear
            };

            extraLines = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 13f),
                TextColor = UIColor.DarkGray,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            ContentView.Add(description);
            ContentView.Add(qty);
            ContentView.Add(amount);
            ContentView.Add(extraLines);
        }

        public void UpdateCell(LoySaleLine saleLine)
        {
            this.saleLine = saleLine;

            description.Text = saleLine.Item.Description;
            qty.Text = saleLine.FormatQuantity(saleLine.Quantity);
            amount.Text = saleLine.Amount;

            extraLines.LineBreakMode = UILineBreakMode.WordWrap;
            extraLines.Lines = 0;
            extraLines.Text = GetExtraLines(saleLine);

            extraLines.SizeToFit();
        }

        static string GetExtraLines(LoySaleLine saleLine)
        {
            //return string.Empty;
            var extraLineText = string.Empty;

            if (saleLine.ExtraInfoLines != null && saleLine.ExtraInfoLines.Count > 0)
            {
                extraLineText = saleLine.ExtraInfoLines[0];
                for (int i = 1; i < saleLine.ExtraInfoLines.Count; i++)
                {
                    extraLineText += System.Environment.NewLine + saleLine.ExtraInfoLines[i];
                }
            }
            return extraLineText;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var height = 23;
            if (string.IsNullOrEmpty(GetExtraLines(saleLine)))
                height = 34;

            description.Frame = new CGRect(15, 5, 175, height);
            qty.Frame = new CGRect(190, 5, 45, height);
            amount.Frame = new CGRect(235, 5, 60, height);
            extraLines.Frame = new CGRect(30, 28, 290, 23);

            extraLines.SizeToFit();
        }

        public static nfloat CalculateHeight(LoySaleLine saleLine)
        {
            var label = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 13f),
                TextColor = UIColor.DarkGray,
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear
            };

            label.Frame = new CGRect(30, 0, 290, 0);

            label.LineBreakMode = UILineBreakMode.WordWrap;
            label.Lines = 0;

            label.Text = GetExtraLines(saleLine);

            label.SizeToFit();

            return (nfloat)Math.Max(label.Frame.Height + 37, 44);

            //return label.StringSize(GetExtraLines(saleLine), UIFont.FromName("Helvetica", 13f)).Height + 28;
        }
    }


    public class TotalLineCell : UITableViewCell
    {
        //private TransactionDetailsTableSource.TotalLine totalLine;
        UIView dividerAbove, dividerBelow;
        UILabel description, amount;

        public TotalLineCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
            ContentView.BackgroundColor = UIColor.FromRGB(255, 255, 255);
            //Accessory = UITableViewCellAccessory.DisclosureIndicator;



            description = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 13f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Right,
                BackgroundColor = UIColor.Clear
            };

            amount = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 13f),
                TextColor = UIColor.Black,
                TextAlignment = UITextAlignment.Right,
                BackgroundColor = UIColor.Clear
            };

            dividerAbove = new UIView()
            {
                BackgroundColor = UIColor.Black
            };

            dividerBelow = new UIView()
            {
                BackgroundColor = UIColor.Black
            };

            ContentView.Add(description);
            ContentView.Add(amount);
            ContentView.Add(dividerAbove);
            ContentView.Add(dividerBelow);

        }

        public void UpdateCell(TransactionDetailsTableSource.TotalLine totalLine)
        {
            //this.totalLine = totalLine;

            description.Text = totalLine.Description;
            amount.Text = totalLine.Amount;

            if (totalLine.DividerAbove)
                dividerAbove.Hidden = false;
            else
                dividerAbove.Hidden = true;

            if (totalLine.DividerBelow)
                dividerBelow.Hidden = false;
            else
                dividerBelow.Hidden = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Utils.Util.AppDelegate.DeviceScreenWidth == 320f)
            {
                description.Frame = new CGRect(15, 5, 220, 23);
                amount.Frame = new CGRect(235, 5, 70, 23);
            }
            else
            {
                description.Frame = new CGRect(55, 5, 220, 23);
                amount.Frame = new CGRect(275, 5, 70, 23);
            }
            dividerAbove.Frame = new CGRect(0, 0, this.ContentView.Frame.Width, 1);
            dividerBelow.Frame = new CGRect(0, 32, this.ContentView.Frame.Width, 1);
        }

        public static float CalculateHeight(TransactionDetailsTableSource.TotalLine totalLine)
        {
            return 34;
        }
    }
}


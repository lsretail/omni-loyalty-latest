using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation.Tables
{
    public class SearchTableSource : UITableViewSource
    {
        private List<SearchScreenDto> tableSearchRs;
        private bool searchResultsEmpty;

        public delegate void ItemOnClickEventHandler(object searchObj);
        public ItemOnClickEventHandler ItemOnClick;

        public SearchTableSource(List<SearchScreenDto> searchRs, bool searchResultsEmpty)
        {
            tableSearchRs = searchRs;
            this.searchResultsEmpty = searchResultsEmpty;
        }

        /// <summary>
        /// Called by the TableView to determine how many sections(groups) there are.
        /// </summary>
        public override nint NumberOfSections(UITableView tableView)
        {
            return tableSearchRs.Count;
        }


        public override nint RowsInSection(UITableView tableview, nint section)
        {

            return tableSearchRs[(int)(section)].Data.Count;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 70f;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            SearchScreenDto SearchScreenDto = tableSearchRs[indexPath.Section];

            if (SearchScreenDto.Description == LocalizationUtilities.LocalizedString("SearchScreen_Coupons", "Coupons") || SearchScreenDto.Description == LocalizationUtilities.LocalizedString("SearchScreen_Offers", "Offers")
                || SearchScreenDto.Description == LocalizationUtilities.LocalizedString("SearchScreen_ItemCategories", "Item Categories") || SearchScreenDto.Description == LocalizationUtilities.LocalizedString("SearchScreen_Items", "Items")
                || SearchScreenDto.Description == LocalizationUtilities.LocalizedString("SearchScreen_ProductGroups", "Product Groups") || SearchScreenDto.Description == LocalizationUtilities.LocalizedString("SearchScreen_Stores", "Stores"))
            {
                var cell = tableView.DequeueReusableCell(SearchCell.Key) as SearchCell;
                if (cell == null)
                {
                    cell = new SearchCell();
                }
                cell.UpdateCell(indexPath.Row, SearchScreenDto.Data[indexPath.Row]);
                return cell;
            }
            if (SearchScreenDto.Description == LocalizationUtilities.LocalizedString("SearchScreen_Transactions", "Transactions"))
            {
                TransactionHistoryCell cell = tableView.DequeueReusableCell(TransactionHistoryCell.Key) as TransactionHistoryCell;
                if (cell == null)
                    cell = new TransactionHistoryCell();

                LoyTransaction transaction = (LoyTransaction)SearchScreenDto.Data[indexPath.Row];
                string locationString = transaction.Store.Description;
                cell.SetValues(indexPath.Row, transaction.DateToShortFormat, locationString, transaction.Amount);
                return cell;
            }
            if (SearchScreenDto.Description == LocalizationUtilities.LocalizedString("SearchScreen_Notifications", "Notifications"))
            {
                NotificationsCell cell = tableView.DequeueReusableCell(NotificationsCell.Key) as NotificationsCell;
                if (cell == null)
                    cell = new NotificationsCell();

                Notification notification = (Notification)SearchScreenDto.Data[indexPath.Row];

                string title = notification.Description;
                string extraInfo = notification.Details;

                // Image
                //string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
                //string imageId = (imageView != null ? imageView.Id : string.Empty);

                cell.SetValues(indexPath.Row, title, extraInfo, string.Empty, string.Empty);

                return cell;
            }
            if (SearchScreenDto.Description == "Profiles")
            {

            }
            if (SearchScreenDto.Description == "ShoppingLists")
            {

            }

            return null;
            //---- create a shortcut to our item
            //object objSearch = this.tableSearchRs[indexPath.Row];
            //cell.UpdateCell(objSearch);

        }


        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //this.controller.HideSearchBarKeyboard();

            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            string header = string.Empty;
            header += tableSearchRs[(int)section].Description + " (" + tableSearchRs[(int)section].Data.Count + ")";

            return header;
        }


        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var view = new UIView()
            {
                Frame = new CoreGraphics.CGRect(0, 0, tableView.Frame.Width, 30f),
                BackgroundColor = Utils.AppColors.BackgroundGray
            };

            var lblCaption = new UILabel()
            {
                Font = UIFont.FromName("Helvetica", 16),
                TextAlignment = UITextAlignment.Left,
                BackgroundColor = UIColor.Clear,
                Tag = 200,
                Frame = new CoreGraphics.CGRect(view.Frame.X + 10f, view.Frame.Y, tableView.Frame.Width, view.Frame.Height)
            };

            var firstAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.Black,
                BackgroundColor = UIColor.Clear,
                Font = UIFont.FromName("Helvetica", 16)
            };

            var secondAttributes = new UIStringAttributes
            {
                ForegroundColor = Utils.AppColors.PrimaryColor,
                BackgroundColor = UIColor.Clear,
                Font = UIFont.FromName("Helvetica", 16)
            };

            view.AddSubview(lblCaption);

            string headerBlack = string.Empty;
            string headerBlue = string.Empty;
            headerBlack += tableSearchRs[(int)section].Description;
            headerBlue += " (" + tableSearchRs[(int)section].Data.Count + ")";

            var twoColoredString = new NSMutableAttributedString(headerBlack + headerBlue);
            twoColoredString.SetAttributes(firstAttributes.Dictionary, new NSRange(0, headerBlack.Length));
            twoColoredString.SetAttributes(secondAttributes.Dictionary, new NSRange(headerBlack.Length, headerBlue.Length));

            lblCaption.AttributedText = twoColoredString;

            return view;
        }

        public override void AccessoryButtonTapped(UITableView tableView, NSIndexPath indexPath)
        {
            //this.controller.HideSearchBarKeyboard();

            if (!this.searchResultsEmpty)
            {
                //must navigate from one controller to next..
                object objSearch = this.tableSearchRs[indexPath.Row];
                if (ItemOnClick != null)
                {
                    ItemOnClick(objSearch);
                }
            }

            tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }

        public override NSIndexPath WillSelectRow(UITableView tableView, NSIndexPath indexPath)
        {
            SearchScreenDto searchScreenDto = tableSearchRs[indexPath.Section];

            //this.controller.HideSearchBarKeyboard();

            if (!this.searchResultsEmpty)
            {
                object objSearch = searchScreenDto.Data[indexPath.Row];
                if (ItemOnClick != null)
                {
                    ItemOnClick(objSearch);
                }
            }
            tableView.DeselectRow(indexPath, true);

            return indexPath;
        }
    }
}


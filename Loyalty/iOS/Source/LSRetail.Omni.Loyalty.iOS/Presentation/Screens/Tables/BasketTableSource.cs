using System;
using UIKit;
using Foundation;
using Presentation.Utils;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation.Tables
{
    public class BasketTableSource : UITableViewSource
    {
        public delegate void RemoveItemFromBasketDelegate(int itemPosition);
        public RemoveItemFromBasketDelegate RemoveItemFromBasket;

        public delegate void ItemPressedDelegate(int itemPosition, OneListItem item);
        public ItemPressedDelegate ItemPressed;

        public BasketTableSource()
        { 
        }

        public bool HasData
        {
            get
            {
                if (AppData.UserLoggedIn && AppData.Device.UserLoggedOnToDevice.Basket.Items.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (AppData.UserLoggedIn)
                return AppData.Device.UserLoggedOnToDevice.Basket.Items.Count;
            else
                return 0;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            OneListItem basketItem = AppData.Device.UserLoggedOnToDevice.Basket.Items[indexPath.Row];

            if (basketItem != null)
            {
                return BasketCell.GetCellHeight(GetExtraInfoString(basketItem));
            }
            else
            {
                return 0f;
            }
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            OneListItem selectedBasketItem = AppData.Device.UserLoggedOnToDevice.Basket.Items[indexPath.Row];

            if (selectedBasketItem != null && this.ItemPressed != null)
                this.ItemPressed(indexPath.Row, selectedBasketItem);

            tableView.DeselectRow(indexPath, true);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            BasketCell cell = tableView.DequeueReusableCell(BasketCell.KEY) as BasketCell;
            if (cell == null)
                cell = new BasketCell();

            OneListItem basketItem = AppData.Device.UserLoggedOnToDevice.Basket.Items[indexPath.Row];

            ImageView imageView = basketItem.Image;
            string imageAvgColor = (imageView != null ? imageView.AvgColor : string.Empty);
            string imageId = (imageView != null ? imageView.Id : string.Empty);

            cell.SetValues(
                indexPath.Row,
                basketItem.Item.Description,
                GetExtraInfoString(basketItem),
                basketItem.Quantity.ToString(),
                AppData.Device.UserLoggedOnToDevice.Environment.Currency.FormatDecimal(basketItem.Amount),
                imageAvgColor,
                imageId
            );

            cell.RemoveItemFromBasket = RemoveItemInCellFromBasket;

            return cell;
        }

        private void RemoveItemInCellFromBasket(int cellIndexPathRow)
        {
            if (this.RemoveItemFromBasket != null)
                this.RemoveItemFromBasket(cellIndexPathRow);
        }

        private string GetExtraInfoString(OneListItem basketItem)
        {
            return basketItem.VariantReg != null ? basketItem.VariantReg.ToString() : string.Empty;
        }
    }
}

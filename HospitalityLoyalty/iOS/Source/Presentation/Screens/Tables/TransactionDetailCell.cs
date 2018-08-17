using System;
using Presentation;
using Presentation.Screens;
using UIKit;

namespace Presentation.Screens
{
	public class TransactionDetailCell : ItemOverviewCell
	{
		TransactionDetailView.ITransactionDetailListeners listener;
		                     
		public TransactionDetailCell(TransactionDetailView.ITransactionDetailListeners listener) : base()
		{
			this.listener = listener;

			this.btnFavorite.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.MenuItemToggleFavorite(this.id);
				this.btnFavorite.SetImage(base.GetFavoriteButtonIcon(listener.MenuItemCheckIfFavorite(this.id)), UIControlState.Normal);

			};

			this.btnReorder.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.MenuItemAddToBasket(this.id);
			};
		}

		public void UpdateData(int id, string title, string extraInfo, string quantity, string formattedPrice, string imageAvgColorHex, string imageId, bool isFavorite)
		{
			base.SetValues(id,title,extraInfo,quantity,formattedPrice,imageAvgColorHex,imageId,isFavorite);
		}
	}
}


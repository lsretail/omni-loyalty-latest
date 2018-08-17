using System;
using Foundation;
using Presentation;
using Presentation.Screens;
using UIKit;
namespace Presentation
{
	public class FavouriteTransactionCell : ItemOverviewCell
	{
		FavouriteView.IFavouritesListeners listener;

		public FavouriteTransactionCell(FavouriteView.IFavouritesListeners listener) : base()
		{
			this.listener = listener;
			this.btnFavorite.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.OnToggleFavourite(this.id, false);
			};
			this.btnReorder.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.AddFavoriteToBasket(this.id, false);
			};
		}
	}
}

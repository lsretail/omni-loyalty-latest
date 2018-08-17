using System;
using Foundation;
using Presentation;
using Presentation.Screens;
using UIKit;

namespace Presentation
{
	public class FavoriteItemCell : ItemOverviewCell
	{
		FavouriteView.IFavouritesListeners listener;

		public FavoriteItemCell(FavouriteView.IFavouritesListeners listener) : base()
		{
			this.listener = listener;
			this.btnFavorite.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.OnToggleFavourite(this.id, true);
			};
			this.btnReorder.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.AddFavoriteToBasket(this.id, true);
			};
		}
	}
}


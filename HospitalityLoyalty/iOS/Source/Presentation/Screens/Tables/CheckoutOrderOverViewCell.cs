using System;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class CheckoutOrderOverViewCell : ItemOverviewCell
	{
		private CellType cellType;
		private readonly CheckoutOrderOverView.ICheckoutOrderOverViewListener listener;

		public enum CellType
		{
			Item,
			Offer
		}

		public CheckoutOrderOverViewCell(CheckoutOrderOverView.ICheckoutOrderOverViewListener listener) : base()
		{
			this.listener = listener;
		}

		public override void SetLayout()
		{
			base.SetLayout();

			UIView customContentView = this.ContentView.ViewWithTag(1);

			this.btnFavorite.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.BasketItemToggleFavorite(this.id);
				this.btnFavorite.SetImage(base.GetFavoriteButtonIcon(listener.BasketItemCheckIfFavorite(this.id)), UIControlState.Normal);

			};

			//Hide the reorder button
			this.btnReorder.Hidden = true;

			// Show delete button instead
			UIButton btnDelete = new UIButton();
			btnDelete.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.Gray), UIControlState.Normal);
			btnDelete.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
			btnDelete.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.listener.RemoveBasketItemPressed(this.cellType, this.id);
			};
			customContentView.AddSubview(btnDelete);

			customContentView.ConstrainLayout(() =>

				btnDelete.Frame.Top == btnReorder.Frame.Top &&
				btnDelete.Frame.Left == btnReorder.Frame.Left &&
				btnDelete.Frame.Bottom == btnReorder.Frame.Bottom &&
				btnDelete.Frame.Right == btnReorder.Frame.Right

			 );
		}

		public void SetValues(
			int id,
			string title,
			string extraInfo,
			string quantity,
			string formattedPrice,
			string imageAvgColorHex,
			string imageId,
			bool isFavorited,
			CellType cellType
		)
		{
			base.SetValues(id, title, extraInfo, quantity, formattedPrice, imageAvgColorHex, imageId, isFavorited);

			this.cellType = cellType;

			if (cellType != CellType.Item)
			{
				UIView btnFavorite = this.ContentView.ViewWithTag(300);
				btnFavorite.Hidden = true;
			}
		}
	}
}


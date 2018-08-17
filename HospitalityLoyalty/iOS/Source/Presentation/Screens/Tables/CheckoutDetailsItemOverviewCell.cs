using System;
using Presentation.Screens;
using Presentation.Utils;
using UIKit;

namespace Presentation.Screens
{
	public class CheckoutDetailsItemOverviewCell : ItemOverviewCell
	{
		//private Action<CellType, int> onDeleteButtonPressed;

		public CheckoutDetailsItemOverviewCell() : base()
		{ }

		public enum CellType
		{
			Item,
			Offer
		}

		public override void SetLayout()
		{
			base.SetLayout();

			UIView customContentView = this.ContentView.ViewWithTag(1);

			// Hide reorder button
			UIButton btnReorder = this.ContentView.ViewWithTag(600) as UIButton;
			btnReorder.Hidden = true;

			// Show delete button instead
			UIButton btnDelete = new UIButton();
			btnDelete.SetImage(Utils.UI.GetColoredImage(UIImage.FromBundle("CancelIcon"), UIColor.Gray), UIControlState.Normal);
			btnDelete.ImageEdgeInsets = new UIEdgeInsets(3, 3, 3, 3);
			btnDelete.TouchUpInside += (object sender, EventArgs e) =>
			{
				//this.onDeleteButtonPressed(this.cellType, this.id); 
			};
			customContentView.AddSubview(btnDelete);

			customContentView.ConstrainLayout(() =>

				btnDelete.Frame.Top == btnReorder.Frame.Top &&
				btnDelete.Frame.Left == btnReorder.Frame.Left &&
				btnDelete.Frame.Bottom == btnReorder.Frame.Bottom &&
				btnDelete.Frame.Right == btnReorder.Frame.Right

			);
		}

		public void SetValues(int id, Action<int> onAddToBasketButtonPressed, Action<int> onFavoriteButtonPressed, Func<int, bool> isFavorited,
			string title, string extraInfo, string quantity, string formattedPrice, string imageAvgColorHex, string imageId,
			Action<CellType, int> onDeleteButtonPressed, CellType cellType)
		{
			//base.SetValues(id, onAddToBasketButtonPressed, onFavoriteButtonPressed, isFavorited,
			//	title, extraInfo, quantity, formattedPrice, imageAvgColorHex, imageId);

			//this.onDeleteButtonPressed = onDeleteButtonPressed;

			if (cellType != CellType.Item)
			{
				UIView btnFavorite = this.ContentView.ViewWithTag(300);
				btnFavorite.Hidden = true;
			}
		}
	}
}


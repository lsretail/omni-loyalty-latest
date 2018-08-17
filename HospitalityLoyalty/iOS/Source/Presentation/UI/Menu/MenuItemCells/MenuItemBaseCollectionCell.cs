using System;
using CoreGraphics;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using UIKit;

namespace Presentation.UI
{
	public class MenuItemBaseCollectionCell : MenuBaseCollectionCell
	{
		protected UIView addToBasketView;

		private WeakReference<IBasketSelectedListener> basketSelectedListener;

		private IBasketSelectedListener BasketSelectedListener
		{
			get
			{
				IBasketSelectedListener listener = null;
				basketSelectedListener.TryGetTarget(out listener);

				return listener;
			}
		}

		public MenuItemBaseCollectionCell(CGRect frame) : base(frame)
		{
			addToBasketView = new UIView();
		}

		public void SetListener(IBasketSelectedListener listener)
		{
			if (basketSelectedListener == null && listener != null)
			{
				basketSelectedListener = new WeakReference<IBasketSelectedListener>(listener);
				addToBasketView.AddGestureRecognizer(new UITapGestureRecognizer(() => { BasketSelectedListener.BasketSelected(this.menu); }));
			}
		}
	}
}

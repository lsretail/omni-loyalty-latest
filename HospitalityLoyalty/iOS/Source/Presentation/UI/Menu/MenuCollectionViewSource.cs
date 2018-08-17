﻿using System;
using System.Collections.Generic;
using Foundation;
using LSRetail.Omni.Domain.DataModel.Base.Menu;
using LSRetail.Omni.Domain.Services.Loyalty.Hospitality.Menus;
using Presentation.Utils;
using UIKit;

namespace Presentation.UI
{
	public class MenuCollectionViewSource : UICollectionViewDataSource
	{
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

		public bool Loaded = false;
		List<MobileMenuNode> menus;
		MenuService menuService;
		public MenuCollectionCellLayouts Style { get; internal set; }

		public MenuCollectionViewSource(IBasketSelectedListener listener)
		{
			this.basketSelectedListener = new WeakReference<IBasketSelectedListener>(listener);

			menus = new List<MobileMenuNode>();
			menuService = new MenuService();
		}

		public void SetData(List<MobileMenuNode> menus)
		{
			this.menus = menus;
			Loaded = true;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			MobileMenuNode node = menus[indexPath.Row];
			string cellKey;
			if (Style != MenuCollectionCellLayouts.Row)
			{
				cellKey = node.NodeType == MobileMenuNodeType.Group ? MenuGroupCollectionThumbnailCell.CellIdentifier : MenuItemCollectionViewThumbnailCell.CellIdentifier;
			} else {
				cellKey = node.NodeType == MobileMenuNodeType.Group ? MenuGroupCollectionRowCell.CellIdentifier : MenuItemCollectionViewRowCell.CellIdentifier;
			}
			var cell = collectionView.DequeueReusableCell(cellKey, indexPath) as MenuBaseCollectionCell;

			cell.SetValue(node);
			if(node.NodeType == MobileMenuNodeType.Item)
			{
				var item = menuService.GetMenuItem(AppData.MobileMenu, node.Id, node.NodeLineType);

				cell.SetPrice(AppData.FormatCurrency(menuService.GetItemFullPrice(AppData.MobileMenu, item)));
				(cell as MenuItemBaseCollectionCell).SetListener(BasketSelectedListener);
			}

			return cell;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return menus.Count;
		}

		public MobileMenuNode GetMenu(int index)
		{
			return menus[index];
		}
	}
}
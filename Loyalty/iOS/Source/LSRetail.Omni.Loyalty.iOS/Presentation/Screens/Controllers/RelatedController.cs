using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class RelatedController : UIViewController
	{
		private RelatedView rootView;
		private RelatedTableSource.RelatedType relatedType;
		private List<LoyItem> items;
		private List<PublishedOffer> offers;
		private Action<string> onSuccess;
		private bool navigationAllowed;

		public RelatedController (RelatedTableSource.RelatedType  relatedType, List<LoyItem> items, List<PublishedOffer> offers, Action<string> onSuccess,bool navigationAllowed = true)
		{
			this.relatedType = relatedType;
			this.navigationAllowed = navigationAllowed;
			this.onSuccess = onSuccess;

			if (items != null) {
				this.items = items;
			}

			if (offers != null) {
				this.offers = offers;
			}

			this.rootView = new RelatedView ();

			switch (this.relatedType) {
				case RelatedTableSource.RelatedType.item: {
						this.Title = LocalizationUtilities.LocalizedString ("Related_ItemTitle", "Related items");
						break;
					}
				case RelatedTableSource.RelatedType.offer: {
						this.Title = LocalizationUtilities.LocalizedString ("Related_OfferTitle", "Related offers & coupons");
						break;
					}
				default:
					this.Title = "";
					break;
			}

			this.rootView.ItemSelected += ItemSelected;
			this.rootView.PublishedOfferSelected += PublishedOfferSelected;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);

			switch (this.relatedType) {
				case RelatedTableSource.RelatedType.item: {
						this.rootView.UpdateData (this.items);
						break;
					}
				case RelatedTableSource.RelatedType.offer: {
						this.rootView.UpdateData (this.offers);
						break;
					}
				default: {
						this.Title = "";
						break;
					}
			}
				
			SetLeftBarButtonItems ();

			this.View = rootView;
		}

		public void SetLeftBarButtonItems()
		{
			UIButton closeButton = new UIButton ();
			closeButton.Frame = new CGRect (0, 0, 50, 30);
			closeButton.SetTitleColor (Utils.AppColors.PrimaryColor,UIControlState.Normal);
			closeButton.SetTitle (LocalizationUtilities.LocalizedString ("General_Close", "Close"), UIControlState.Normal);
			closeButton.TouchUpInside += (sender, e) => {
				this.NavigationController.DismissViewController (true, () =>  {});
			};
				
			this.NavigationItem.LeftBarButtonItem = new UIBarButtonItem (closeButton);
		}

		private void HandleRelatedItemSelectedFromPublishedOffersScreen(LoyItem item, Action<bool> dismissSelf)
		{
			UINavigationController nc = this.NavigationController;
			ItemDetailsController itemDetailsController = new ItemDetailsController(item);
			nc.PopToRootViewController(false);
			nc.PushViewController (itemDetailsController, false);

			dismissSelf(true);
		}

		private void HandleRelatedPublishedOfferSelectedFromItemDetailsScreen(PublishedOffer publishedOffer, Action<bool> dismissSelf)
		{
			UINavigationController nc = this.NavigationController;
			PublishedOfferDetailController publishedOfferDetailController = new PublishedOfferDetailController(publishedOffer);
			nc.PopToRootViewController(false);
			nc.PushViewController (publishedOfferDetailController, false);

			dismissSelf(true);
		}

		private void ItemSelected (string id) 
		{
			if (this.navigationAllowed) {
				DismissViewController (false, null);
				onSuccess (id);
			}
		}

		private void PublishedOfferSelected (string id) 
		{
			if (this.navigationAllowed) {
				DismissViewController (false, null);
				onSuccess (id);
			}
		}
	}
}


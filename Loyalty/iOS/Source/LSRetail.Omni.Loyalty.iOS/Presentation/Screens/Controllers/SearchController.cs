using System;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Loyalty.Setup;
using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Items;
using LSRetail.Omni.Domain.DataModel.Base.Setup;
using LSRetail.Omni.Domain.DataModel.Loyalty.Transactions;

namespace Presentation
{
    public class SearchController : UIViewController
	{
		private SearchView rootView;
		private UIButton SearchFilterButton;

		public SearchController ()
		{
			this.rootView = new SearchView ();
			this.rootView.Search += GetSearchResults;
			this.rootView.ItemOnClick += PushToSearchDetails;
			this.Title = LocalizationUtilities.LocalizedString("SlideoutMenu_Search", "Search");
			this.View = rootView;
		}
				

		public override void ViewWillLayoutSubviews ()
		{
			base.ViewWillLayoutSubviews ();

			this.rootView.TopLayoutGuideLength = (float) this.TopLayoutGuide.Length;
			this.rootView.BottomLayoutGuideLength = (float) this.BottomLayoutGuide.Length;
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);

			Utils.UI.StyleNavigationBar(this.NavigationController.NavigationBar);
			SetRightBarButtonItems ();
		}

		private List<SearchScreenDto> GetSearchSuccess(SearchRs searchRs, SearchPopUpDto searchPopUpDto)
		{
			//put all into one flat list
			List<SearchScreenDto> objList = new List<SearchScreenDto>();

			if(searchPopUpDto.Item)
				objList.Add (new SearchScreenDto () { Description = LocalizationUtilities.LocalizedString("SearchScreen_ItemCategories", "Item Categories"), Data = new List<object> (searchRs.ItemCategories) });
			if(searchPopUpDto.Item)
				objList.Add (new SearchScreenDto () {Description = LocalizationUtilities.LocalizedString("SearchScreen_Items", "Items"), Data = new List<object> (searchRs.Items)});
			if(searchPopUpDto.Notification)
				objList.Add (new SearchScreenDto () {Description = LocalizationUtilities.LocalizedString("SearchScreen_Notifications", "Notifications"), Data = new List<object> (searchRs.Notifications)});
			if(searchPopUpDto.Item)
				objList.Add (new SearchScreenDto () {Description = LocalizationUtilities.LocalizedString("SearchScreen_ProductGroups", "Product Groups"), Data = new List<object> (searchRs.ProductGroups)});
				//objList.Add (new SearchScreenDto () {Description = LocalizationUtilities.LocalizedString("SearchScreen_Profiles", "Profiles"), Data = new List<object> (searchRs.Profiles)});
			//objList.Add (new SearchScreenDto () {Description = LocalizationUtilities.LocalizedString("SearchScreen_ShoppingLists", "ShoppingLists"), Data = new List<object> (searchRs.ShoppingLists)});
			if(searchPopUpDto.Store)
				objList.Add (new SearchScreenDto () {Description = LocalizationUtilities.LocalizedString("SearchScreen_Stores", "Stores"), Data = new List<object> (searchRs.Stores)});
			if(searchPopUpDto.History)
				objList.Add (new SearchScreenDto () {Description = LocalizationUtilities.LocalizedString("SearchScreen_Transactions", "Transactions"), Data = new List<object> (searchRs.Transactions)});

			// We use a custom search result cell to indicate that the "Search returned no results".
			// So actual search result array won't be empty, so we explicitly indicate that there 
			// were no search results with a boolean variable
			Utils.UI.HideLoadingIndicator ();

			return objList;
		}

		public void SetRightBarButtonItems()
		{
			List<UIBarButtonItem> barButtonItemList = new List<UIBarButtonItem>();

			this.SearchFilterButton = new UIButton ();
			this.SearchFilterButton.Frame = new CoreGraphics.CGRect (0, 0, 30, 30);
            this.SearchFilterButton.SetImage(ImageUtilities.GetColoredImage(UIImage.FromBundle ("FilterList"), Utils.UI.NavigationBarContentColor), UIControlState.Normal);
			this.SearchFilterButton.ImageEdgeInsets = new UIEdgeInsets (5, 5, 5, 5);
			this.SearchFilterButton.TouchUpInside += (object sender, EventArgs e) =>
			{
				this.rootView.HideSearchBarKeyboard ();
				this.rootView.ShowSearchFilterPopUp ();
			};
			barButtonItemList.Add (new UIBarButtonItem(this.SearchFilterButton));
			this.NavigationItem.RightBarButtonItems = barButtonItemList.ToArray ();
		}
			
		private bool isObjListEmpty( List<SearchScreenDto> SearchScreenDtos)
		{
			foreach(var searchScreenDto in SearchScreenDtos)
			{
				if (searchScreenDto.Data.Count > 0)
					return false;
			}
			return true;
		}

		public async void PushToSearchDetails(object searchObj)
		{
			if (searchObj is PublishedOffer)
			{
				PublishedOfferDetailController publishedOfferDetailsController = new PublishedOfferDetailController((PublishedOffer) searchObj);
				this.NavigationController.PushViewController (publishedOfferDetailsController, true);
			}
			else if (searchObj is Notification) 
			{
				NotificationDetailsController detailsController = new NotificationDetailsController ((Notification) searchObj);
				this.NavigationController.PushViewController (detailsController, true);
			}
			else if (searchObj is LoyItem )
			{				
				ItemDetailsController itemDetailsController = new ItemDetailsController((LoyItem) searchObj);
				this.NavigationController.PushViewController(itemDetailsController, true);       
			}
			else if (searchObj is Store)
			{
				LocationDetailController detailsScreen = new LocationDetailController ((Store) searchObj, AppData.Stores, false);	// TODO The details screen shouldn't have all stores
				this.NavigationController.PushViewController (detailsScreen, true);
			}
			else if (searchObj is LoyTransaction)
			{
				var transaction = (LoyTransaction) searchObj;

				if (transaction.Platform == Platform.Standard)
				{
					TransactionDetailController transactionDetailController = new TransactionDetailController (transaction);
					this.NavigationController.PushViewController (transactionDetailController, true);
				}
				else
				{
					await AlertView.ShowAlert(
						this,
						LocalizationUtilities.LocalizedString("TransactionView_TransactionUnavailable", "Transaction unavailable"),
						LocalizationUtilities.LocalizedString("TransactionView_TransactionUnavailableMsg", "This transaction is unavailable at the moment"),
						LocalizationUtilities.LocalizedString("General_OK", "OK")
					);
				}

			}
		}
			
		private async void GetSearchResults(string search, SearchPopUpDto searchPopUpDto,Action<List<SearchScreenDto>> onSuccess, Action onFailure)
		{
			Utils.UI.ShowLoadingIndicator ();
            SearchRs searchRs = await new Models.SearchModel().GetSearch(search);
            if (searchRs != null)
			{
				onSuccess(GetSearchSuccess(searchRs, searchPopUpDto));

			}
            else
			{
				Utils.UI.HideLoadingIndicator ();
				onFailure();
			}
			
		}
	}
}


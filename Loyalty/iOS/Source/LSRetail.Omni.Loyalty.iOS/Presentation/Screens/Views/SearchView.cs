using System;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;
using Presentation.Tables;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class SearchView : BaseView
    {
        private UISearchBar searchBar;
        private UITableView tblSearch;
        private UIView vwNoData;
        private SearchPopUpDto searchPopUpDto;
        private SearchPopUpView vwSearchFilterPopUp;
        private UILabel lblNoData;
        private const int margin = 5;

        public delegate void SearchEventHandler(string searchText, SearchPopUpDto searchPopUpDto, Action<List<SearchScreenDto>> onSuccess, Action onFailure);
        public delegate void ItemOnClickEventHandler(object searchObj);

        public event SearchEventHandler Search;
        public event ItemOnClickEventHandler ItemOnClick;

        public SearchView()
        {
            this.searchPopUpDto = new SearchPopUpDto();

            this.searchBar = new UISearchBar();
            this.searchBar.BarStyle = UIBarStyle.Default;
            this.searchBar.Translucent = true;
            this.searchBar.ShowsCancelButton = false;
            this.searchBar.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.searchBar.SearchButtonClicked += (object sender, EventArgs e) =>
            {

                if (string.IsNullOrWhiteSpace(this.searchBar.Text.Trim()) || this.searchBar.Text.Trim().Length < 2)
                {
                    this.tblSearch.Source = null;
                    this.tblSearch.ReloadData();
                    ShowNoDataView();

                    Utils.UI.HideLoadingIndicator();
                }
                else
                {
                    onSearch();
                }
                this.searchBar.ResignFirstResponder();
            };

            this.searchBar.TextChanged += (object sender, UISearchBarTextChangedEventArgs e) =>
            {
                if (string.IsNullOrWhiteSpace(this.searchBar.Text.Trim()) || this.searchBar.Text.Trim().Length < 2)
                {
                    this.tblSearch.Source = null;
                    this.tblSearch.ReloadData();
                    ShowNoDataView();

                    Utils.UI.HideLoadingIndicator();
                }
                else
                {
                    onSearch();
                }
            };

            this.tblSearch = new UITableView();
            this.tblSearch.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.tblSearch.SeparatorStyle = UITableViewCellSeparatorStyle.None;

            this.vwNoData = new UIView();
            this.vwNoData.BackgroundColor = UIColor.Clear;

            this.lblNoData = new UILabel();
            this.lblNoData.Text = LocalizationUtilities.LocalizedString("Search_Instruction", "Use the search bar for general search.");
            this.lblNoData.TextColor = UIColor.Gray;
            this.lblNoData.TextAlignment = UITextAlignment.Center;
            this.lblNoData.Font = UIFont.SystemFontOfSize(14);

            this.vwSearchFilterPopUp = new SearchPopUpView(this.searchPopUpDto);
            this.vwSearchFilterPopUp.Ok += OnSearchPopUpOk;
            this.vwSearchFilterPopUp.Hidden = true;

            this.vwNoData.AddSubview(lblNoData);
            this.AddSubview(this.vwNoData);
            this.AddSubview(this.searchBar);
            this.AddSubview(this.tblSearch);
            this.AddSubview(this.vwSearchFilterPopUp);

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            this.searchBar.Frame = new CGRect(
                0,
                this.TopLayoutGuideLength,
                this.Bounds.Width,
                44f
            );

            this.tblSearch.Frame = new CGRect(
                0,
                this.searchBar.Frame.Height + this.TopLayoutGuideLength + margin,
                this.Bounds.Width,
                this.Bounds.Height - this.searchBar.Frame.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength - margin
            );

            this.vwNoData.Frame = new CGRect(
                0,
                this.searchBar.Frame.Height + this.TopLayoutGuideLength,
                this.Bounds.Width,
                this.Bounds.Height - this.searchBar.Frame.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength
            );

            this.lblNoData.Frame = new CGRect(
                0,
                vwNoData.Bounds.Height / 2 - 20f,
                vwNoData.Bounds.Width,
                20f);

            this.vwSearchFilterPopUp.SetFrame(new CGRect(
                20,
                this.TopLayoutGuideLength + 2 * margin,
                this.Frame.Width - 40,
                this.Frame.Height - this.TopLayoutGuideLength - this.BottomLayoutGuideLength - 8 * margin),
                false
            );
            HideSearchFilterPopUp();
            ShowNoDataView();
        }

        private void ShowNoDataView()
        {
            this.vwNoData.Hidden = false;
            this.tblSearch.Hidden = true;
        }

        private void HideNoDataView()
        {
            this.vwNoData.Hidden = true;
            this.tblSearch.Hidden = false;
        }

        public void ShowSearchFilterPopUp()
        {
            this.tblSearch.Hidden = true;
            this.vwSearchFilterPopUp.ShowWithAnimation();

        }

        public void HideSearchFilterPopUp()
        {
            this.tblSearch.Hidden = false;
            this.vwSearchFilterPopUp.HideWithAnimation();
        }

        public void HideSearchBarKeyboard()
        {
            //hide the keyboard
            this.searchBar.ResignFirstResponder();
        }

        private void onSearch()
        {
            if (Search != null)
            {
                Search(this.searchBar.Text.Trim(), this.searchPopUpDto, SearchOnSuccess, () => { });
            }
        }

        private void onItemClick(object objSearch)
        {
            if (ItemOnClick != null)
            {
                ItemOnClick(objSearch);
            }
        }

        private void OnSearchPopUpOk(SearchPopUpDto searchPopUpDto)
        {
            this.searchPopUpDto = searchPopUpDto;
            onSearch();
            HideSearchFilterPopUp();
        }

        private void SearchOnSuccess(List<SearchScreenDto> searchList)
        {
            HideNoDataView();
            SearchTableSource tableData = new SearchTableSource(searchList, false);
            tableData.ItemOnClick += onItemClick;
            this.tblSearch.Source = tableData;
            this.tblSearch.ReloadData();
        }
    }
}


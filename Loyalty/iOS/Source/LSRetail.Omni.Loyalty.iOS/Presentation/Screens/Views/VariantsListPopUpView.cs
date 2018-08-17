using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.Linq;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class VariantsListPopUpView : PopUpView
    {
        private VariantExt VariantExt;

        private UITableView tblVariants;
        private UILabel lblTitle;
        private UIButton btnOk;

        nfloat labelHeight = 40f;
        private const int margin = 5;
        private const int btnHeight = 50;
        nfloat tableHeight;

        public delegate void EditingDoneEventHandler();
        public EditingDoneEventHandler EditingDone;

        public VariantsListPopUpView() : base(true)
        {
            this.BackgroundColor = Utils.AppColors.BackgroundGray;

            this.VariantExt = null;
            this.lblTitle = new UILabel();
            this.lblTitle.Font = UIFont.SystemFontOfSize(18);
            this.lblTitle.TextColor = Utils.AppColors.PrimaryColor;
            this.lblTitle.TextAlignment = UITextAlignment.Center;
            this.lblTitle.BackgroundColor = UIColor.Clear;

            this.tblVariants = new UITableView();
            this.tblVariants.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.tblVariants.SeparatorStyle = UITableViewCellSeparatorStyle.None;
            this.tblVariants.ScrollEnabled = true;
            this.tblVariants.Bounces = true;
            this.tblVariants.AlwaysBounceVertical = true;
            this.tblVariants.ShowsVerticalScrollIndicator = true;
            this.tblVariants.ShowsHorizontalScrollIndicator = true;

            this.btnOk = new UIButton();
            this.btnOk.SetTitle(LocalizationUtilities.LocalizedString("General_OK", "OK"), UIControlState.Normal);
            this.btnOk.SetTitleColor(Utils.AppColors.SoftWhite, UIControlState.Normal);
            this.btnOk.BackgroundColor = Utils.AppColors.PrimaryColor;
            this.btnOk.TouchUpInside += (sender, e) =>
            {
                if (EditingDone != null)
                {
                    EditingDone();
                }
            };

            this.AddSubview(this.lblTitle);
            this.AddSubview(this.tblVariants);
            this.AddSubview(this.btnOk);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            this.lblTitle.Frame = new CoreGraphics.CGRect(
                2 * margin,
                2 * margin,
                this.Frame.Width - 4 * margin,
                labelHeight
            );

            tableHeight = (this.Frame.Height - this.lblTitle.Frame.Height - btnHeight - 8 * margin);

            this.tblVariants.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.lblTitle.Frame.Bottom + 2 * margin,
                this.Frame.Width - 4 * margin,
                tableHeight
            );

            this.btnOk.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.tblVariants.Frame.Bottom + 2 * margin,
                this.Frame.Width - 4 * margin,
                btnHeight
            );

        }

        public void UpdateValues(VariantExt variantExt)
        {
            this.VariantExt = variantExt;
            this.lblTitle.Text = variantExt.Code;

            bool isVariantChosen = false;

            foreach (var x in variantExt.Values)
            {
                if (x.IsSelected == true)
                {
                    isVariantChosen = true;
                }
            }

            if (isVariantChosen == false)
            {
                variantExt.Values[0].IsSelected = true;
            }

            SetVariantTableSource();
        }

        private void SetVariantTableSource()
        {
            ChangeVariantPopUpTableSource changeVariantPopUpTableSource = new ChangeVariantPopUpTableSource(this.VariantExt.Values);
            changeVariantPopUpTableSource.VariantSelected += (DimValue x) => SetSelectedVariant(x);
            this.tblVariants.Source = changeVariantPopUpTableSource;

            this.tblVariants.ReloadData();
            this.tblVariants.SelectRow(
                (this.tblVariants.Source as ChangeVariantPopUpTableSource).GetIndexPathForSelectedVariant(),
                false,
                UITableViewScrollPosition.Top
            );

            this.tblVariants.Hidden = false;
        }

        public void SetSelectedVariant(DimValue variant)
        {
            System.Diagnostics.Debug.WriteLine("ItemVariantUOMScreen.SetSelectedVariant() - Setting selected variant as: " + variant);

            foreach (var x in this.VariantExt.Values)
            {
                if (x.Value == variant.Value)
                {
                    x.IsSelected = true;
                }
                else
                {
                    x.IsSelected = false;
                }
            }
        }

        private class ChangeVariantPopUpTableSource : UITableViewSource
        {
            private const string CELL_IDENTIFIER = "VariantCellID";
            private List<DimValue> variants;

            public delegate void VariantSelectedEventHandler(DimValue variant);
            public event VariantSelectedEventHandler VariantSelected;

            public ChangeVariantPopUpTableSource(List<DimValue> variants)
            {
                this.variants = variants;
            }

            public override nint NumberOfSections(UITableView tableView)
            {
                return 1;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return this.variants.Count;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                var cell = tableView.DequeueReusableCell(CELL_IDENTIFIER) as UITableViewCell;
                if (cell == null)
                    cell = new UITableViewCell(UITableViewCellStyle.Value1, CELL_IDENTIFIER);

                cell.TextLabel.Text = this.variants[indexPath.Row].Value;
                cell.BackgroundColor = Utils.AppColors.BackgroundGray;
               /* if (this.variants[indexPath.Row].IsAvailable == false)
                {
                    cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                    cell.TextLabel.TextColor = Utils.AppColors.MediumGray;
                }
                else*/
               // {
                    cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
                    cell.TextLabel.TextColor = UIColor.Black;
                //}

                return cell;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                // you can only choose a variant that is available
                //if (this.variants[indexPath.Row].IsAvailable)
                //{
                    if (this.VariantSelected != null)
                        this.VariantSelected(this.variants[indexPath.Row]);
                //}
            }

            public NSIndexPath GetIndexPathForVariant(string variantId)
            {
                int row = 0;
                var variant = this.variants.Where(x => x.Value == variantId).FirstOrDefault();
                if (variant != null)
                    row = this.variants.IndexOf(variant);

                return NSIndexPath.FromRowSection(row, 0);
            }

            public NSIndexPath GetIndexPathForSelectedVariant()
            {
                int row = 0;
                var variant = this.variants.Where(x => x.IsSelected == true).FirstOrDefault();
                if (variant != null)
                    row = this.variants.IndexOf(variant);

                return NSIndexPath.FromRowSection(row, 0);
            }
        }
    }
}


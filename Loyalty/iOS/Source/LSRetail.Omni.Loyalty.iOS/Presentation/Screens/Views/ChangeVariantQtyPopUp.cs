using System;
using UIKit;
using System.Collections.Generic;
using Presentation.Utils;
using System.Linq;
using CoreGraphics;
using LSRetail.Omni.GUIExtensions.iOS;
using LSRetail.Omni.Domain.DataModel.Base.Retail;

namespace Presentation
{
    public class ChangeVariantQtyPopUp : PopUpView
    {
        private List<VariantExt> VariantsExt;
        private List<VariantRegistration> VariantsRegistration;
        private VariantRegistration OldVariantRegistration;

        private UILabel lblTitle;
        private VariantsListPopUpView variantListPopUpView;

        private UIScrollView vwDimensions;
        private UIView vwChangeQty;

        private UILabel lblChangeQty;
        private UIButton btnPlusQty;
        private UILabel lblItemQuantity;
        private UIButton btnMinusQty;
        private UIButton btnOk;
        private UIButton btnCancel;

        private UIButton btnDimension0;
        private UIButton btnDimension1;
        private UIButton btnDimension2;
        private UIButton btnDimension3;
        private UIButton btnDimension4;
        private UIButton btnDimension5;

        private const int margin = 5;
        private const int btnHeight = 50;
        private int qty;
        private int oldQty;
        nfloat labelHeight = 40f;
        nfloat changeQtyViewHeight = 40f;
        nfloat changeQtyViewMargin = 10f;
        nfloat changeQtyViewButtonWidth = 50f;
        nfloat TotalHeight = 0;

        public delegate void EditingDoneEventHandler(decimal quantity, VariantRegistration VariantRegistration);
        public EditingDoneEventHandler EditingDone;

        public ChangeVariantQtyPopUp() : base(true)
        {
            this.BackgroundColor = Utils.AppColors.BackgroundGray;
            this.OldVariantRegistration = null;
            this.oldQty = 1;

            this.VariantsExt = new List<VariantExt>();
            this.VariantsRegistration = new List<VariantRegistration>();

            this.lblTitle = new UILabel();
            this.lblTitle.Font = UIFont.SystemFontOfSize(18);
            this.lblTitle.TextColor = Utils.AppColors.PrimaryColor;
            this.lblTitle.TextAlignment = UITextAlignment.Center;
            this.lblTitle.BackgroundColor = UIColor.Clear;

            this.vwDimensions = new UIScrollView();
            this.vwDimensions.BackgroundColor = UIColor.Clear;
            this.vwDimensions.ScrollEnabled = true;
            this.vwDimensions.UserInteractionEnabled = true;

            this.btnDimension0 = new UIButton();
            this.btnDimension0.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
            this.btnDimension0.BackgroundColor = Utils.AppColors.SoftWhite;
            this.btnDimension0.TouchUpInside += (sender, e) =>
            {
                this.variantListPopUpView.ShowWithAnimation();
                this.variantListPopUpView.UpdateValues(this.VariantsExt[0]);
            };

            this.btnDimension1 = new UIButton();
            this.btnDimension1.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
            this.btnDimension1.BackgroundColor = Utils.AppColors.SoftWhite;
            this.btnDimension1.TouchUpInside += (sender, e) =>
            {
                this.variantListPopUpView.ShowWithAnimation();
                this.variantListPopUpView.UpdateValues(this.VariantsExt[1]);
            };

            this.btnDimension2 = new UIButton();
            this.btnDimension2.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
            this.btnDimension2.BackgroundColor = Utils.AppColors.SoftWhite;
            this.btnDimension2.TouchUpInside += (sender, e) =>
            {
                this.variantListPopUpView.ShowWithAnimation();
                this.variantListPopUpView.UpdateValues(this.VariantsExt[2]);
            };

            this.btnDimension3 = new UIButton();
            this.btnDimension3.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
            this.btnDimension3.BackgroundColor = Utils.AppColors.SoftWhite;
            this.btnDimension3.TouchUpInside += (sender, e) =>
            {
                this.variantListPopUpView.ShowWithAnimation();
                this.variantListPopUpView.UpdateValues(this.VariantsExt[3]);
            };

            this.btnDimension4 = new UIButton();
            this.btnDimension4.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
            this.btnDimension4.BackgroundColor = Utils.AppColors.SoftWhite;
            this.btnDimension4.TouchUpInside += (sender, e) =>
            {
                this.variantListPopUpView.ShowWithAnimation();
                this.variantListPopUpView.UpdateValues(this.VariantsExt[4]);
            };

            this.btnDimension5 = new UIButton();
            this.btnDimension5.SetTitleColor(Utils.AppColors.PrimaryColor, UIControlState.Normal);
            this.btnDimension5.BackgroundColor = Utils.AppColors.SoftWhite;
            this.btnDimension5.TouchUpInside += (sender, e) =>
            {
                this.variantListPopUpView.ShowWithAnimation();
                this.variantListPopUpView.UpdateValues(this.VariantsExt[5]);
            };

            // Change quantity bar

            this.vwChangeQty = new UIView();
            this.vwChangeQty.BackgroundColor = UIColor.Clear;

            this.lblChangeQty = new UILabel();
            this.lblChangeQty.Text = LocalizationUtilities.LocalizedString("ItemDetails_Quantity", "Quantity") + ":";

            this.btnPlusQty = new UIButton();
            this.btnPlusQty.SetTitle("+", UIControlState.Normal);
            this.btnPlusQty.SetTitleColor(AppColors.PrimaryColor, UIControlState.Normal);
            this.btnPlusQty.BackgroundColor = UIColor.Clear;
            this.btnPlusQty.TouchUpInside += (object sender, EventArgs e) => { IncreaseQuantityToAddToBasket(); };

            this.lblItemQuantity = new UILabel();
            this.lblItemQuantity.TextColor = AppColors.PrimaryColor;
            this.lblItemQuantity.Font = UIFont.SystemFontOfSize(14);
            this.lblItemQuantity.TextAlignment = UITextAlignment.Center;

            this.btnMinusQty = new UIButton();
            this.btnMinusQty.SetTitle("-", UIControlState.Normal);
            this.btnMinusQty.SetTitleColor(AppColors.PrimaryColor, UIControlState.Normal);
            this.btnMinusQty.BackgroundColor = UIColor.Clear;
            this.btnMinusQty.TouchUpInside += (object sender, EventArgs e) => { DecreaseQuantityToAddToBasket(); };

            this.btnOk = new UIButton();
            this.btnOk.SetTitle(LocalizationUtilities.LocalizedString("General_OK", "OK"), UIControlState.Normal);
            this.btnOk.SetTitleColor(Utils.AppColors.SoftWhite, UIControlState.Normal);
            this.btnOk.BackgroundColor = Utils.AppColors.PrimaryColor;
            this.btnOk.TouchUpInside += (sender, e) =>
            {
                OnOk();
            };

            this.btnCancel = new UIButton();
            this.btnCancel.SetTitle(LocalizationUtilities.LocalizedString("General_Cancel", "Cancel"), UIControlState.Normal);
            this.btnCancel.SetTitleColor(Utils.AppColors.SoftWhite, UIControlState.Normal);
            this.btnCancel.BackgroundColor = Utils.AppColors.PrimaryColor;
            this.btnCancel.TouchUpInside += (sender, e) =>
            {
                OnCancel();
                this.HideWithAnimation();
            };

            this.variantListPopUpView = new VariantsListPopUpView();
            this.variantListPopUpView.EditingDone = () =>
            {
                UpdateView();
                this.variantListPopUpView.HideWithAnimation();
            };

            this.AddSubview(this.lblTitle);
            this.vwDimensions.AddSubview(this.btnDimension0);
            this.vwDimensions.AddSubview(this.btnDimension1);
            this.vwDimensions.AddSubview(this.btnDimension2);
            this.vwDimensions.AddSubview(this.btnDimension3);
            this.vwDimensions.AddSubview(this.btnDimension4);
            this.vwDimensions.AddSubview(this.btnDimension5);
            this.vwChangeQty.AddSubview(lblChangeQty);
            this.vwChangeQty.AddSubview(btnPlusQty);
            this.vwChangeQty.AddSubview(lblItemQuantity);
            this.vwChangeQty.AddSubview(btnMinusQty);
            this.AddSubview(this.vwDimensions);
            this.AddSubview(this.vwChangeQty);
            this.AddSubview(this.btnOk);
            this.AddSubview(this.btnCancel);

            this.AddSubview(this.variantListPopUpView);
        }

        private void RefreshQuantityToAddToBasketLabel()
        {
            this.lblItemQuantity.Text = this.qty.ToString();
        }

        private void IncreaseQuantityToAddToBasket()
        {
            this.qty++;
            RefreshQuantityToAddToBasketLabel();
        }

        private void DecreaseQuantityToAddToBasket()
        {
            if (this.qty > 1)
                this.qty--;
            RefreshQuantityToAddToBasketLabel();
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

            // If variantExt and VariantRegistration does not contain any item then there is no need to show the vwDimensions

            nfloat vwDimensionsHeight = 0;

            if (this.VariantsExt.Count > 0 && this.VariantsRegistration.Count > 0)
            {
                vwDimensionsHeight = this.Frame.Height - this.lblTitle.Frame.Height - btnHeight - changeQtyViewHeight - 8 * margin;
            }

            this.vwDimensions.Frame = new CoreGraphics.CGRect(
                0,
                this.lblTitle.Frame.Bottom,
                this.Frame.Width,
                vwDimensionsHeight
            );

            this.btnDimension0.Frame = new CoreGraphics.CGRect(
                2 * margin,
                2 * margin,
                this.Frame.Width - 4 * margin,
                btnHeight
            );

            this.btnDimension1.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.btnDimension0.Frame.Bottom + 2 * margin,
                this.Frame.Width - 4 * margin,
                btnHeight
            );

            this.btnDimension2.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.btnDimension1.Frame.Bottom + 2 * margin,
                this.Frame.Width - 4 * margin,
                btnHeight
            );

            this.btnDimension3.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.btnDimension2.Frame.Bottom + 2 * margin,
                this.Frame.Width - 4 * margin,
                btnHeight
            );

            this.btnDimension4.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.btnDimension3.Frame.Bottom + 2 * margin,
                this.Frame.Width - 4 * margin,
                btnHeight
            );

            this.btnDimension5.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.btnDimension4.Frame.Bottom + 2 * margin,
                this.Frame.Width - 4 * margin,
                btnHeight
            );

            this.vwChangeQty.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.vwDimensions.Frame.Bottom + 2 * margin,
                this.Frame.Width - 4 * margin,
                changeQtyViewHeight
            );

            this.lblChangeQty.Frame = new CoreGraphics.CGRect(
                changeQtyViewMargin,
                0f,
                80f,
                this.vwChangeQty.Frame.Height
            );

            this.btnPlusQty.Frame = new CoreGraphics.CGRect(
                this.vwChangeQty.Frame.Width - changeQtyViewButtonWidth,
                0,
                changeQtyViewButtonWidth,
                this.vwChangeQty.Frame.Height
            );

            this.lblItemQuantity.Frame = new CoreGraphics.CGRect(
                btnPlusQty.Frame.Left - 40f,
                0,
                40f,
                this.vwChangeQty.Frame.Height
            );

            this.btnMinusQty.Frame = new CoreGraphics.CGRect(
                lblItemQuantity.Frame.Left - changeQtyViewButtonWidth,
                0,
                changeQtyViewButtonWidth,
                this.vwChangeQty.Frame.Height
            );

            this.btnCancel.Frame = new CoreGraphics.CGRect(
                2 * margin,
                this.vwChangeQty.Frame.Bottom + 2 * margin,
                this.Frame.Width / 2 - 3 * margin,
                btnHeight
            );

            this.btnOk.Frame = new CoreGraphics.CGRect(
                this.btnCancel.Frame.Right + 2 * margin,
                this.vwChangeQty.Frame.Bottom + 2 * margin,
                this.Frame.Width / 2 - 3 * margin,
                btnHeight
            );

            // Change variant and quantity popup

            this.variantListPopUpView.TopLayoutGuideLength = this.TopLayoutGuideLength;
            this.variantListPopUpView.BottomLayoutGuideLength = this.BottomLayoutGuideLength;

            this.variantListPopUpView.SetFrame(
                new CGRect(
                    0,
                    0,
                    this.Frame.Width,
                    this.Frame.Height
                )
            );

            SetRequiredHeight();
        }

        public void UpdateValues(List<VariantExt> variantsExt, List<VariantRegistration> variantsRegistration, string itemDescription, decimal quantity, VariantRegistration selectedVariant)
        {
            this.VariantsExt = variantsExt;
            this.VariantsRegistration = variantsRegistration;
            this.lblTitle.Text = itemDescription;
            this.qty = (int)quantity;
            this.oldQty = (int)quantity;
            this.lblItemQuantity.Text = this.qty.ToString();
            this.OldVariantRegistration = selectedVariant;

            VariantExt.SetIsSelectedFromVariantReg(variantsExt, selectedVariant);

            for (int i = 0; i < variantsExt.Count; i++)
            {
                if (i == 0)
                {
                    this.btnDimension0.Hidden = false;
                }
                if (i == 1)
                {
                    this.btnDimension1.Hidden = false;
                }
                if (i == 2)
                {
                    this.btnDimension2.Hidden = false;
                }
                if (i == 3)
                {
                    this.btnDimension3.Hidden = false;
                }
                if (i == 4)
                {
                    this.btnDimension4.Hidden = false;
                }
                if (i == 5)
                {
                    this.btnDimension5.Hidden = false;
                }
            }

            for (int i = 5; i >= variantsExt.Count; i--)
            {
                if (i == 0)
                {
                    this.btnDimension0.Hidden = true;
                }
                if (i == 1)
                {
                    this.btnDimension1.Hidden = true;
                }
                if (i == 2)
                {
                    this.btnDimension2.Hidden = true;
                }
                if (i == 3)
                {
                    this.btnDimension3.Hidden = true;
                }
                if (i == 4)
                {
                    this.btnDimension4.Hidden = true;
                }
                if (i == 5)
                {
                    this.btnDimension5.Hidden = true; ;
                }
            }

            UpdateView();
            LayoutSubviews();
        }

        private void SetRequiredHeight()
        {
            this.TotalHeight = Utils.Util.AppDelegate.Window.Frame.Height - TopLayoutGuideLength - BottomLayoutGuideLength;
        }

        public nfloat GetRequiredHeight(bool onlyQuantity = false)
        {
            if (onlyQuantity)
            {
                return this.vwChangeQty.Frame.Height + this.lblTitle.Frame.Height + this.btnOk.Frame.Height + 8 * margin;
            }
            else
            {
                return this.TotalHeight;
            }
        }

        public void UpdateView()
        {
            DimValue tempVariant = null;
            for (int i = 0; i < this.VariantsExt.Count; i++)
            {
                if (i == 0)
                {
                    tempVariant = this.VariantsExt[0].Values.Where(x => x.IsSelected == true).FirstOrDefault();
                    if (tempVariant != null)
                    {
                        this.btnDimension0.SetTitle(tempVariant.Value, UIControlState.Normal);
                    }
                    else
                    {
                        this.btnDimension0.SetTitle(this.VariantsExt[0].Code, UIControlState.Normal);
                    }
                }
                if (i == 1)
                {
                    tempVariant = this.VariantsExt[1].Values.Where(x => x.IsSelected == true).FirstOrDefault();
                    if (tempVariant != null)
                    {
                        this.btnDimension1.SetTitle(tempVariant.Value, UIControlState.Normal);
                    }
                    else
                    {
                        this.btnDimension1.SetTitle(this.VariantsExt[1].Code, UIControlState.Normal);
                    }
                }
                if (i == 2)
                {
                    tempVariant = this.VariantsExt[2].Values.Where(x => x.IsSelected == true).FirstOrDefault();
                    if (tempVariant != null)
                    {
                        this.btnDimension2.SetTitle(tempVariant.Value, UIControlState.Normal);
                    }
                    else
                    {
                        this.btnDimension2.SetTitle(this.VariantsExt[2].Code, UIControlState.Normal);
                    }
                }
                if (i == 3)
                {
                    tempVariant = this.VariantsExt[3].Values.Where(x => x.IsSelected == true).FirstOrDefault();
                    if (tempVariant != null)
                    {
                        this.btnDimension3.SetTitle(tempVariant.Value, UIControlState.Normal);
                    }
                    else
                    {
                        this.btnDimension3.SetTitle(this.VariantsExt[3].Code, UIControlState.Normal);
                    }
                }
                if (i == 4)
                {
                    tempVariant = this.VariantsExt[4].Values.Where(x => x.IsSelected == true).FirstOrDefault();
                    if (tempVariant != null)
                    {
                        this.btnDimension4.SetTitle(tempVariant.Value, UIControlState.Normal);
                    }
                    else
                    {
                        this.btnDimension4.SetTitle(this.VariantsExt[4].Code, UIControlState.Normal);
                    }
                }
                if (i == 5)
                {
                    tempVariant = this.VariantsExt[5].Values.Where(x => x.IsSelected == true).FirstOrDefault();
                    if (tempVariant != null)
                    {
                        this.btnDimension5.SetTitle(tempVariant.Value, UIControlState.Normal);
                    }
                    else
                    {
                        this.btnDimension5.SetTitle(this.VariantsExt[5].Code, UIControlState.Normal);
                    }
                }
            }
        }

        public async void OnOk()
        {
            if (this.VariantsRegistration.Count == 0 || this.VariantsExt.Count == 0)
            {
                if (EditingDone != null)
                {
                    EditingDone(qty, null);
                }
            }
            else
            {
                if (this.VariantsExt != null)
                {
                    if (EditingDone != null)
                    {
                        VariantRegistration variantRegistrationToReturn = VariantRegistration.GetVariantRegistrationFromVariantExts(this.VariantsExt, this.VariantsRegistration);
                        if (variantRegistrationToReturn != null)
                        {
                            EditingDone(qty, variantRegistrationToReturn);
                        }
                        else
                        {
                            EditingDone(qty, null);
                        }
                        this.HideWithAnimation();
                    }
                }
                else
                {
                    await AlertView.ShowAlert(
                        null,
                        LocalizationUtilities.LocalizedString("General_Error", "Error"),
                        LocalizationUtilities.LocalizedString("ChangeVariantQtyPopUp_PleaseChooseVariant", "Please choose variant for all dimesions"),
                        LocalizationUtilities.LocalizedString("General_OK", "OK")
                    );
                }
            }
        }

        public void OnCancel()
        {
            this.qty = this.oldQty;

            foreach (var variantExt in VariantsExt)
            {
                foreach (var values in variantExt.Values)
                {
                    values.IsSelected = false;
                }
            }

            if (this.OldVariantRegistration != null)
                //VariantExt.SetIsSelectedFromVariantReg (this.VariantsExt, this.OldVariantRegistration);

                UpdateView();
            RefreshQuantityToAddToBasketLabel();
        }
    }

    public class CustomToolbarDelegate : UIToolbarDelegate
    {
        public override UIBarPosition GetPositionForBar(IUIBarPositioning barPositioning)
        {
            return UIBarPosition.TopAttached;
        }
    }

}


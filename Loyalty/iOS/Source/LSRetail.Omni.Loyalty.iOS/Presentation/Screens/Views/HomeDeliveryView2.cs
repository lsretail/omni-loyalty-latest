using System;
using Presentation.Utils;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using LSRetail.Omni.GUIExtensions.iOS;

namespace Presentation
{
    public class HomeDeliveryView2 : BaseView
    {
        private UIScrollView container;
        private AddressForm shippingAdr;
        private AddressForm billingAdr;
        private CardInfoForm payment;

        private RadioGroup paymentOpt;

        private UILabel shiplbl;
        private UILabel payOptlbl;
        private UILabel paylbl;
        private UILabel billlbl;


        private UIView btnCtn;
        private UIButton btnPlaceOrder;
        public delegate void SendOrderEventHandler(Action onSuccess);
        public SendOrderEventHandler SendOrder;

        public HomeDeliveryView2()
        {
            container = new UIScrollView();

            shiplbl = new UILabel()
            {
                Text = "Shipping Address",
                TextColor = UIColor.Gray,
                Font = UIFont.SystemFontOfSize(14)
            };

            shippingAdr = new AddressForm(AppData.Device.UserLoggedOnToDevice);
            payOptlbl = new UILabel()
            {
                Text = "Payment Options",
                TextColor = UIColor.Gray,
                Font = UIFont.SystemFontOfSize(14)
            };

            List<string> values = new List<string>();

            values.Add("Pay on delivery");
            values.Add("Credit Card");

            paymentOpt = new RadioGroup(values);
            paymentOpt.Selected += Selected;

            paylbl = new UILabel()
            {
                Text = "Payment",
                TextColor = UIColor.Gray,
                Font = UIFont.SystemFontOfSize(14),
                Hidden = true
            };

            payment = new CardInfoForm();
            payment.Hidden = true;

            billlbl = new UILabel()
            {
                Text = "Billing Address",
                TextColor = UIColor.Gray,
                Font = UIFont.SystemFontOfSize(14),
                Hidden = true
            };

            btnPlaceOrder = new UIButton();
            btnPlaceOrder.SetTitle(LocalizationUtilities.LocalizedString("Checkout_PlaceOrder", "Place order"), UIControlState.Normal);
            btnPlaceOrder.BackgroundColor = Utils.AppColors.PrimaryColor;
            btnPlaceOrder.Layer.CornerRadius = 2;
            /*this.btnPlaceOrder.TouchUpInside += (object sender, EventArgs e) =>
			{
				if (this.SendOrder != null)
				{
					this.SendOrder(() =>
					{
						
					});
				}
			};*/

            btnCtn = new UIView();
            btnCtn.AddSubview(btnPlaceOrder);
            billingAdr = new AddressForm(AppData.Device.UserLoggedOnToDevice);
            billingAdr.Hidden = true;
            container.AddSubviews(shiplbl, shippingAdr, payOptlbl, paymentOpt, paylbl, payment, billlbl, billingAdr, btnCtn);
            AddSubview(container);

        }

        private void Selected(nint row)
        {
            if (row == 0)
            {
                billingAdr.Hidden = true;
                billlbl.Hidden = true;
                paylbl.Hidden = true;
                payment.Hidden = true;
                container.ContentSize = new CGSize(Frame.Width, shiplbl.Frame.Height + shippingAdr.Frame.Height + payOptlbl.Frame.Height + paymentOpt.Frame.Height + btnCtn.Frame.Height + 20f);
                btnCtn.Frame = new CGRect(0, paymentOpt.Frame.Bottom, Frame.Width, 60f);
            }
            else
            {
                billingAdr.Hidden = false;
                billlbl.Hidden = false;
                paylbl.Hidden = false;
                payment.Hidden = false;
                container.ContentSize = new CGSize(Frame.Width, shiplbl.Frame.Height + shippingAdr.Frame.Height + payOptlbl.Frame.Height + paymentOpt.Frame.Height + paylbl.Frame.Height + payment.Frame.Height + billlbl.Frame.Height + billingAdr.Frame.Height + btnCtn.Frame.Height + 20f);
                btnCtn.Frame = new CGRect(0, billingAdr.Frame.Bottom, Frame.Width, 60f);

            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            container.Frame = new CGRect(0, 0, Frame.Width, Frame.Height);
            shiplbl.Frame = new CGRect(10f, 0, Frame.Width, 60f);
            shippingAdr.LayoutSubviews();
            nfloat sHeight = 0;
            foreach (UIView child in shippingAdr.Subviews)
            {
                sHeight = sHeight + child.Frame.Height;
            }
            shippingAdr.Frame = new CGRect(0, shiplbl.Frame.Bottom, Frame.Width, sHeight);
            payOptlbl.Frame = new CGRect(10f, shippingAdr.Frame.Bottom, Frame.Width, 60f);
            paymentOpt.Frame = new CGRect(0, payOptlbl.Frame.Bottom, Frame.Width, paymentOpt.rHeight * 2 + paymentOpt.padding);

            paylbl.Frame = new CGRect(10f, paymentOpt.Frame.Bottom, Frame.Width, 60f);

            payment.Frame = new CGRect(0, paylbl.Frame.Bottom, Frame.Width, 120f);

            billlbl.Frame = new CGRect(10f, payment.Frame.Bottom, Frame.Width, 60f);

            billingAdr.LayoutSubviews();
            sHeight = 0;
            foreach (UIView child in billingAdr.Subviews)
            {
                sHeight = sHeight + child.Frame.Height;
            }
            billingAdr.Frame = new CGRect(0, billlbl.Frame.Bottom, Frame.Width, sHeight);
            btnCtn.Frame = new CGRect(0, paymentOpt.Frame.Bottom, Frame.Width, 60f);
            btnPlaceOrder.Frame = new CGRect(10f, 10f, Frame.Width - 20f, 40f);

            container.ContentSize = new CGSize(Frame.Width, shiplbl.Frame.Height + shippingAdr.Frame.Height + payOptlbl.Frame.Height + paymentOpt.Frame.Height + btnCtn.Frame.Height + 20f);
            //container.ContentSize = new CGSize(Frame.Width, shiplbl.Frame.Height + shippingAdr.Frame.Height + paylbl.Frame.Height + paymentOpt.Frame.Height + billlbl.Frame.Height + billingAdr.Frame.Height);

            // container.ContentSize = new CGSize(0,0,0,0);

        }

    }
}


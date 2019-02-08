﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using LSRetail.Omni.Domain.DataModel.Base.Retail;
using LSRetail.Omni.Domain.DataModel.Loyalty.Baskets;
using LSRetail.Omni.Domain.DataModel.Base.Base;
using LSRetail.Omni.Domain.DataModel.Base.SalesEntries;

namespace LSRetail.Omni.Domain.DataModel.Loyalty.Orders
{
    [DataContract(Namespace = "http://lsretail.com/LSOmniService/Loy/2017")]
    public class Order : Entity, IDisposable
    { 
        public Order(string id) : base(id)
        {
            SourceType = SourceType.LSOmni;
            OrderStatus = SalesEntryStatus.Pending;
            ShippingStatus = ShippingStatus.ShippigNotRequired;
            PaymentStatus = PaymentStatus.PreApproved;

            OrderLines = new List<OrderLine>();
            OrderDiscountLines = new List<OrderDiscountLine>();
            OrderPayments = new List<OrderPayment>();
            ContactAddress = new Address();
            ShipToAddress = new Address();
        }

        public Order() : this(string.Empty)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (OrderLines != null)
                    OrderLines.Clear();
                if (OrderDiscountLines != null)
                    OrderDiscountLines.Clear();
                if (OrderPayments != null)
                    OrderPayments.Clear();
            }
        }

        /// <summary>
        /// Order Number
        /// </summary>
        [DataMember]
        public string DocumentId { get; set; }
        [DataMember]
        public DateTime DocumentRegTime { get; set; }
        [DataMember]
        public string StoreId { get; set; }
        /// <summary>
        /// Member Contact Card Id
        /// </summary>
        [DataMember]
        public string CardId { get; set; }
        /// <summary>
        /// Transaction Receipt Number
        /// </summary>
        [DataMember]
        public string ReceiptNo { get; set; }

        [DataMember]
        public SourceType SourceType { get; set; }
        [DataMember]
        public SalesEntryStatus OrderStatus { get; set; }
        [DataMember]
        public PaymentStatus PaymentStatus { get; set; }
        [DataMember]
        public ShippingStatus ShippingStatus { get; set; }

        [DataMember]
        public decimal TotalNetAmount { get; set; }
        [DataMember]
        public decimal TotalAmount { get; set; }
        [DataMember]
        public decimal TotalDiscount { get; set; }
        [DataMember]
        public int LineItemCount { get; set; }

        /// <summary>
        /// True if Order has already been Posted in Accounting
        /// </summary>
        [DataMember]
        public bool Posted { get; set; }
        /// <summary>
        /// Sales Order or Click and Collect Order
        /// Click And Collect order are processed in the store at POS Terminal
        /// </summary>
        [DataMember]
        public bool ClickAndCollectOrder { get; set; }
        /// <summary>
        /// AnonymousOrder
        /// </summary>
        [DataMember]
        public bool AnonymousOrder { get; set; }
        /// <summary>
        /// Store to collect Click And Collect Order from
        /// </summary>
        [DataMember]
        public string CollectLocation { get; set; }
        /// <summary>
        /// Click And Collect Order should be shipped after store processing
        /// </summary>
        [DataMember]
        public bool ShipClickAndCollect { get; set; }

        [DataMember]
        public List<OrderLine> OrderLines { get; set; }
        [DataMember]
        public List<OrderDiscountLine> OrderDiscountLines { get; set; }
        [DataMember]
        public List<OrderPayment> OrderPayments { get; set; }

        /// <summary>
        /// Member Contact Id
        /// </summary>
        [DataMember]
        public string ContactId { get; set; }
        [DataMember]
        public string ContactName { get; set; }
        [DataMember]
        public Address ContactAddress { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public string MobileNumber { get; set; }
        [DataMember]
        public string DayPhoneNumber { get; set; }

        // shipping info
        [DataMember]
        public string ShipToName { get; set; }
        [DataMember]
        public Address ShipToAddress { get; set; }
        [DataMember]
        public string ShipToEmail { get; set; }
        [DataMember]
        public string ShipToPhoneNumber { get; set; }
        [DataMember]
        public string ShippingAgentCode { get; set; }
        [DataMember]
        public string ShippingAgentServiceCode { get; set; }

        /// <summary>
        /// Current Member Contact Point balance 
        /// </summary>
        [DataMember]
        public decimal PointBalance { get; set; }
        /// <summary>
        /// Points used as payment in order
        /// </summary>
        [DataMember]
        public decimal PointsUsedInOrder { get; set; }
        /// <summary>
        /// Number of points needed to pay full amount
        /// </summary>
        [DataMember]
        public decimal PointAmount { get; set; }
        /// <summary>
        /// Cash Needed with points, if Point balance is not enough to pay order in full
        /// </summary>
        [DataMember]
        public decimal PointCashAmountNeeded { get; set; }
        /// <summary>
        /// Points Rewarded for this Order
        /// </summary>
        [DataMember]
        public decimal PointsRewarded { get; set; }

        // local properties to find transactions
        public string TransId { get; set; }
        public string TransStore { get; set; }
        public string TransTerminal { get; set; }

        public override string ToString()
        {
            string req = string.Empty, osldreq = string.Empty;
            try
            {
                foreach (OrderLine line in OrderLines)
                    req += "[" + line.ToString() + "] ";
                foreach (OrderDiscountLine line in OrderDiscountLines)
                    osldreq += "[" + line.ToString() + "] ";
            }
            catch
            {
            }

            string s = string.Format("Id: {0} StoreId: {1} CardId: {2} ContactId: {3} SourceType: {4}  OrderLineCreateRequests: {5}  OrderDiscountLineCreateRequests: {6}",
                Id, StoreId, CardId, ContactId, SourceType.ToString(), req, osldreq);
            return s;
        }
    }
}


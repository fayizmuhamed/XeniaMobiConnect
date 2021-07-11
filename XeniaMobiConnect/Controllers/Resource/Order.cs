﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Controllers.Resource
{
    public class Order
    {
        public long invid { get; set; }
        public string invType { get; set; }
        public string invno { get; set; }
        public Nullable<System.DateTime> invdate { get; set; }
        public string party { get; set; }
        public Nullable<long> lid { get; set; }
        public string mop { get; set; }
        public string taxType { get; set; }
        public string contactno { get; set; }
        public string remarks { get; set; }
        public string address { get; set; }
        public Nullable<int> SalePersonID { get; set; }
        public Nullable<double> grossAmount { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> taxableAmount { get; set; }
        public Nullable<double> taxAmount { get; set; }
        public Nullable<decimal> Exempted { get; set; }
        public Nullable<double> roundOff { get; set; }
        public Nullable<int> roundOffMode { get; set; }
        public Nullable<double> cashDiscount { get; set; }
        public Nullable<double> billAmount { get; set; }
        public Nullable<double> otherExpense { get; set; }
        public Nullable<double> freightCoolie { get; set; }
        public Nullable<double> cost { get; set; }
        public Nullable<double> SaleValues { get; set; }
        public Nullable<double> BillProfit { get; set; }
        public Nullable<bool> blnProcessed { get; set; }
        public Nullable<bool> cancelled { get; set; }
        public string tinNo { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
        public Nullable<long> AutoNo { get; set; }
        public string referenceNo { get; set; }
        public Nullable<double> costfactor { get; set; }
        public string compName { get; set; }
        public Nullable<long> userid { get; set; }
        public string OrderDetail { get; set; }
        public string DespatchDetail { get; set; }
        public Nullable<double> cessAmount { get; set; }
        public Nullable<double> AddCessAmt { get; set; }
        public Nullable<int> rndCutoff { get; set; }
        public Nullable<int> ccID { get; set; }
        public string ccName { get; set; }
        public string cardDetails { get; set; }
        public Nullable<double> creditAmount { get; set; }
        public Nullable<double> cardAmount { get; set; }
        public Nullable<double> cashAmount { get; set; }
        public Nullable<double> itemDiscount { get; set; }
        public string orderID { get; set; }
        public Nullable<double> labourCost { get; set; }
        public Nullable<double> otherCost { get; set; }
        public Nullable<double> serviceTax { get; set; }
        public Nullable<double> cessOnTax { get; set; }
        public Nullable<int> typeID { get; set; }
        public Nullable<int> counterid { get; set; }
        public Nullable<bool> IsSettled { get; set; }
        public Nullable<int> dccID { get; set; }
        public string dccName { get; set; }
        public Nullable<double> cgstAmount { get; set; }
        public Nullable<double> sgstAmount { get; set; }
        public Nullable<double> igstAmount { get; set; }
        public Nullable<int> AgentId { get; set; }
        public Nullable<double> kfcessAmt { get; set; }
        public Nullable<System.DateTime> deliverydate { get; set; }
        public Nullable<long> ReturnId { get; set; }
        public Nullable<System.DateTime> CreditDate { get; set; }
        public string CreditStatus { get; set; }
        public Nullable<bool> blnOrder { get; set; }
        public Nullable<int> loyID { get; set; }
        public Nullable<double> pointIn { get; set; }
        public Nullable<double> pointOut { get; set; }
        public List<OrderDetail> orderDetails { get; set; }
        public string CompanyVPA { get; set; }
        public Nullable<double> totalTax { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Controllers.Resource
{
    public class OrderDetail
    {
        public long invID { get; set; }
        public Nullable<long> itemid { get; set; }
        public Nullable<double> taxper { get; set; }
        public Nullable<decimal> mrp { get; set; }
        public Nullable<double> quantity { get; set; }
        public Nullable<double> freeQty { get; set; }
        public Nullable<double> grossAmount { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> taxable { get; set; }
        public Nullable<double> taxAmount { get; set; }
        public Nullable<double> netAmount { get; set; }
        public string batch { get; set; }
        public Nullable<double> cost { get; set; }
        public Nullable<double> saleValue { get; set; }
        public Nullable<double> profit { get; set; }
        public Nullable<int> rOrder { get; set; }
        public string extraColumn { get; set; }
        public byte[] SSMA_TimeStamp { get; set; }
        public Nullable<double> unitfactor { get; set; }
        public Nullable<bool> blnAltUnit { get; set; }
        public string unit { get; set; }
        public string expDate { get; set; }
        public Nullable<double> sRate { get; set; }
        public Nullable<double> pRate { get; set; }
        public string modifier { get; set; }
        public Nullable<double> cgstPer { get; set; }
        public Nullable<double> cgstAmount { get; set; }
        public Nullable<double> sgstPer { get; set; }
        public Nullable<double> sgstAmount { get; set; }
        public Nullable<double> igstPer { get; set; }
        public Nullable<double> igstAmount { get; set; }
        public Nullable<double> cessPer { get; set; }
        public Nullable<double> cessAmount { get; set; }
        public Nullable<double> AddCessRate { get; set; }
        public Nullable<double> AddCessAmt { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public string SerialNo { get; set; }
        public Nullable<double> kfcessper { get; set; }
        public Nullable<double> kfcessamt { get; set; }
        public string ItemName { get; set; }
        public string Image { get; set; }
        public Nullable<double> totalTax { get; set; }
    }
}
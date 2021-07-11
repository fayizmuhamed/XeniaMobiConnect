using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Controllers.Resource
{
    public class Customer
    {
        public long lid { get; set; }
        public string ledger { get; set; }
        public string LedgerName { get; set; }
        public string contactno { get; set; }
        public string phoneNo { get; set; }
        public string address { get; set; }
        public string tinno { get; set; }
        public string email { get; set; }
        public string regionalName { get; set; }
        public string remarks { get; set; }
        public decimal ledgerBalance { get; set; }

        public Customer(mtrledger mtrledger)
        {
            this.lid = mtrledger.lid;
            this.ledger = mtrledger.ledger;
            this.LedgerName = mtrledger.LedgerName;
            this.contactno = mtrledger.contactno;
            this.phoneNo = mtrledger.phoneNo;
            this.address = mtrledger.address;
            this.tinno = mtrledger.tinno;
            this.email = mtrledger.email;
            this.regionalName = mtrledger.regionalName;
            this.remarks = mtrledger.remarks;
            this.ledgerBalance = 0;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Controllers.Resource
{
    public class CartItem
    {
        public int rOrder { get; set; }
        public Nullable<long> itemid { get; set; }
        public Nullable<double> quantity { get; set; }
        public Nullable<double> freeQty { get; set; }
        public string unit { get; set; }
        public Nullable<bool> blnAltUnit { get; set; }
       
        


    }
}
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace XeniaMobiConnect
{
    using System;
    using System.Collections.Generic;
    
    public partial class trsItemOffer
    {
        public int id { get; set; }
        public string offerName { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> startTime { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<System.DateTime> endTime { get; set; }
        public int Itemid { get; set; }
        public string batch { get; set; }
        public Nullable<double> mrp { get; set; }
        public Nullable<double> rate { get; set; }
        public Nullable<double> Discount { get; set; }
        public Nullable<double> netRate { get; set; }
        public Nullable<bool> deActive { get; set; }
    }
}

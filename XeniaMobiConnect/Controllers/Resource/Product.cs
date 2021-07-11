using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Controllers.Resource
{
    public class Product
    {
        public long prodID { get; set; }
        public string prodCode { get; set; }
        public string prodName { get; set; }
        public long categoryid { get; set; }
        public Nullable<decimal> gstPer { get; set; }
        public Nullable<decimal> igstPer { get; set; }
        public Nullable<bool> staxincl { get; set; }
        public string mnf { get; set; }
        public string mainUnit { get; set; }
        public Nullable<decimal> srate { get; set; }
        public Nullable<decimal> mrp { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<double> cessPer { get; set; }
        public string remarks { get; set; }
        public string regionalName { get; set; }
        public string alternateUnit { get; set; }
        public Nullable<double> convfactor { get; set; }
        public string Batch { get; set; }
        public string hsnCode { get; set; }
        public Nullable<double> AddCess { get; set; }
        public Nullable<double> OSRate { get; set; }
        public string outerBatch { get; set; }
        public Nullable<double> kfcessPer { get; set; }
        public Nullable<long> mcategoryid { get; set; }
        public string Image { get; set; }

        public Product(mtrProduct mtrProduct)
        {
            this.prodID = mtrProduct.prodID;
            this.prodCode = mtrProduct.prodCode;
            this.prodName = mtrProduct.prodName;
            this.categoryid = mtrProduct.categoryid;
            this.gstPer = mtrProduct.gstPer;
            this.igstPer = mtrProduct.igstPer;
            this.staxincl = mtrProduct.staxincl;
            this.mnf = mtrProduct.mnf;
            this.mainUnit = mtrProduct.mainUnit;
            this.srate = mtrProduct.srate;
            this.mrp = mtrProduct.mrp;
            this.active = mtrProduct.active;
            this.cessPer = mtrProduct.cessPer;
            this.remarks = mtrProduct.remarks;
            this.regionalName = mtrProduct.regionalName;
            this.alternateUnit = mtrProduct.alternateUnit;
            this.convfactor = mtrProduct.convfactor;
            this.Batch = mtrProduct.Batch;
            this.hsnCode = hsnCode;
            this.AddCess = mtrProduct.AddCess;
            this.OSRate = mtrProduct.OSRate;
            this.outerBatch = mtrProduct.outerBatch;
            this.kfcessPer = mtrProduct.kfcessPer;
            this.mcategoryid = mtrProduct.mcategoryid;
        }
    }
}
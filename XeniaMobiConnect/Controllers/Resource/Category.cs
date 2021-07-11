using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Controllers.Resource
{
    public class Category
    {
        public long categoryid { get; set; }
        public string category { get; set; }
        public Nullable<bool> deActive { get; set; }
        public string regionalName { get; set; }
        public Nullable<int> McatId { get; set; }

        public Category(mtrCategory mtrCategory)
        {
            this.categoryid = mtrCategory.categoryid;
            this.category = mtrCategory.category;
            this.deActive = mtrCategory.deActive;
            this.regionalName = mtrCategory.regionalName;
            this.McatId = mtrCategory.McatId;
        }
    }
}
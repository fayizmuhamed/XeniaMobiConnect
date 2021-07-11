using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using XeniaMobiConnect.Controllers.Resource;
using XeniaMobiConnect.Util;

namespace XeniaMobiConnect.Controllers
{
    public class ProductController : ApiController
    {

        private Entities db = new Entities();

        [HttpGet]
        [Route("api/Categories")]
        public APIResponse GetCategories(int pageNumber,int pageSize)
        {
            var headers = Request.Headers;
            int customerId = APIUtil.GetCustomerId(db, headers);

            var priceListItems = GetPriceList(customerId);

            var count = db.mtrCategories.Count();
            var categories = db.mtrCategories.Where(c => c.deActive == false).OrderBy(c => c.category).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList().Select(c => new Category(c)).ToList();
            
            if (categories == null)
            {
                categories = new List<Category>();
            }
           
            PagedResponse<Product> pagedResponse = new PagedResponse<Product>(categories, count, pageNumber, pageSize);

            return pagedResponse;

        }

        [HttpGet]
        [Route("api/Products")]
        public APIResponse GetProducts(long categoryid,string query, int pageNumber, int pageSize)
        {
            var headers = Request.Headers;
            int customerId = APIUtil.GetCustomerId(db, headers);

            var priceListItems = GetPriceList(customerId);
            List<Product> products = new List<Product>();
            var count = 0;
            if (query == null || query == "")
            {
                count = db.mtrProducts.Where(p => p.categoryid == (categoryid == 0 ? p.categoryid : categoryid) && p.prodName.ToLower().Contains(query == null || query == "" ? p.prodName.ToLower() : query.ToLower()) && p.itemClassID == 0 && p.active == false).Count();
                products =db.mtrProducts.Where(p => p.categoryid == (categoryid == 0 ? p.categoryid : categoryid) && p.prodName.ToLower().Contains(query == null || query == "" ? p.prodName.ToLower() : query.ToLower()) && p.itemClassID == 0 && p.active == false).OrderBy(p => p.prodName).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList().Select(p => new Product(p)).ToList();
            }
            else
            {
                count = db.mtrProducts.Where(p => p.categoryid == (categoryid == 0 ? p.categoryid : categoryid) && p.prodName.ToLower().Contains(query == null || query == "" ? p.prodName.ToLower() : query.ToLower()) && p.itemClassID == 0 && p.active == false).Count();
                products =db.mtrProducts.Where(p => p.categoryid == (categoryid == 0 ? p.categoryid : categoryid) && p.prodName.ToLower().Contains(query == null || query == "" ? p.prodName.ToLower() : query.ToLower()) && p.itemClassID == 0 && p.active == false).OrderBy(p => p.prodName.ToLower().IndexOf(query == null || query == "" ? p.prodName.ToLower() : query.ToLower())).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList().Select(p => new Product(p)).ToList();
            }
            /*if (query == null || query == "")
            {
                count = db.mtrProducts.Where(p => p.categoryid == (categoryid == 0 ? p.categoryid : categoryid) && p.prodName.ToLower().Contains(query == null || query == "" ? p.prodName.ToLower() : query.ToLower()) && p.itemClassID == 0 && p.active == false).Count();
                products = db.mtrProducts.Where(p => p.categoryid == (categoryid == 0 ? p.categoryid : categoryid) && p.prodName.ToLower().Contains(query == null || query == "" ? p.prodName.ToLower() : query.ToLower()) && p.itemClassID == 0 && p.active == false).OrderBy(p => p.prodName).ToList().Select(p => new Product(p)).ToList();
            }
            else
            {
                count = db.mtrProducts.Where(p => p.categoryid == (categoryid == 0 ? p.categoryid : categoryid) && p.prodName.ToLower().Contains(query == null || query == "" ? p.prodName.ToLower() : query.ToLower()) && p.itemClassID == 0 && p.active == false).Count();
                products = db.mtrProducts.Where(p => p.categoryid == (categoryid == 0 ? p.categoryid : categoryid) && p.prodName.ToLower().Contains(query == null || query == "" ? p.prodName.ToLower() : query.ToLower()) && p.itemClassID == 0 && p.active == false).OrderBy(p => p.prodName.ToLower().IndexOf(query == null || query == "" ? p.prodName.ToLower() : query.ToLower())).ToList().Select(p => new Product(p)).ToList();
            }*/
            if (products == null)
                products = new List<Product>();

            products.ForEach(p =>
            {
                mtrItemImage productImage=db.mtrItemImages.Where(image => image.itemId == p.prodID).FirstOrDefault();
                if (productImage != null && productImage.itemImage!=null)
                {
                    p.Image = Convert.ToBase64String(productImage.itemImage);
                }

                if (priceListItems != null && priceListItems.Count() > 0)
                {

                    var priceListPrice = priceListItems.Where(item => item.prodId == p.prodID).FirstOrDefault();
                    if (priceListPrice != null)
                    {
                        p.srate = (decimal)priceListPrice.price;
                    }
                }
            });

            PagedResponse<Product> pagedResponse = new PagedResponse<Product>(products, count, pageNumber, pageSize);

            return pagedResponse;

           // return new APIResponse(APIResponseStatus.success, products, "");

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public List<trsPricelist> GetPriceList(int customerId)
        {
           
            //Check PriceList Enabled Or Not
            var blnPricelist = db.companySettings.Where(e => e.KeyName == "blnPricelist").FirstOrDefault();

            if (blnPricelist == null || blnPricelist.ValueName == null || blnPricelist.ValueName.Trim() == "" && blnPricelist.ValueName == "False")
            {
                return null;
            }

            var customer = db.mtrledgers.Find(customerId);
            if (customer == null || customer.plID == null )
            {
                return null;
            }

            var priceList = db.mtrPricelists.Where(p => p.plId == customer.plID && p.deActive == false).FirstOrDefault();
            if (priceList == null)
            {
                return null;
            }

            var priceListItems = db.trsPricelists.Where(p => p.plId == customer.plID).ToList();
            if (priceListItems == null || priceListItems.Count()==0)
            {
                return null;
            }

            return priceListItems;


        }
    }
}

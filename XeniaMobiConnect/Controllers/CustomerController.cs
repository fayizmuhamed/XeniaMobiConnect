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
    public class CustomerController : ApiController
    {
        private Entities db = new Entities();


        // GET: api/Customer
        [HttpPost]
        [Route("api/Customer/Login")]
        public APIResponse Login(string mobileNo,string otpCode)
        {

            var customer = db.mtrledgers.Where(c => c.accid == 9 && c.contactno == mobileNo && c.deActive == false).FirstOrDefault();

            if (customer == null)
            {
                return new APIResponse(APIResponseStatus.failed, null, "Customer Not Found");
            }

            trsOTP trsOTP = db.trsOTPs.Where(o =>  o.mobileno == mobileNo && o.type == OTPType.LOGIN && o.otpCode == otpCode).FirstOrDefault();
            if (trsOTP == null)
            {
                return new APIResponse(APIResponseStatus.failed, null, "Invalid OTP");
            }

            if (trsOTP.expiryDate < DateTime.Now)
            {
                return new APIResponse(APIResponseStatus.failed, null, "OTP Expired");
            }
            
            
            return new APIResponse(APIResponseStatus.success, Build(customer));
        }

       

        // GET: api/Customer/5
        [HttpGet]
        [Route("api/Customer/info")]
        public APIResponse GetCustomerInfo()
        {
            var headers = Request.Headers;
            int customerId = APIUtil.GetCustomerId(db, headers);

            var customer = db.mtrledgers.Where(c => c.lid == customerId && c.deActive == false).FirstOrDefault();

            if (customer == null)
            {
                return new APIResponse(APIResponseStatus.failed, null, "Customer Not Found");
            }

            return new APIResponse(APIResponseStatus.success, Build(customer), "");

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public Customer Build(mtrledger mtrledger)
        {
            Customer customer = new Customer(mtrledger);
            var amtDecimal = 2;
            try
            {
                var companySetting= db.companySettings.Where(e => e.KeyName == "amtDecimal").FirstOrDefault();
                amtDecimal = int.Parse((companySetting == null || companySetting.ValueName == null || companySetting.ValueName.Equals("")) ? "2" : companySetting.ValueName);
            }
            catch
            {

            }
            // var ledgerBalance = db.trsAccounts.Where(a => a.Drlid == customer.lid && a.IsHold==false).Sum(a => ((a.AmountD == null ? 0 : a.AmountD) - (a.AmountC == null ? 0 : a.AmountC)));
            var ledgerBalance = db.Database.SqlQuery<decimal>("Select ISNULL(sum(ISNULL(AmountD,0)-ISNULL(AmountC,0)),0) from [dbo].trsAccounts where Drlid=" + customer.lid + " and IsHold=0").FirstOrDefault();
            customer.ledgerBalance = Math.Round((ledgerBalance == null ? 0 : (decimal)ledgerBalance),amtDecimal);
            return customer;
        }

       

        
    }
}

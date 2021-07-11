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
    public class OTPType
    {
        public static string LOGIN = "LOGIN";
    }

    public class OTPController : ApiController
    {
        private Entities db = new Entities();

        [HttpPost]
        [Route("api/OTP/Login")]
        public APIResponse GenerateLoginOTP(string mobileNo)
        {
            var customer = db.mtrledgers.Where(c => c.accid==9 && c.contactno == mobileNo && c.deActive == false).FirstOrDefault();

            if (customer == null)
            {
                return new APIResponse(APIResponseStatus.failed, null, "Customer Not Found");
            }

            trsOTP trsOTP = new trsOTP
            {
                type = OTPType.LOGIN,
                mobileno = mobileNo,
                otpCode = GetOTPCode(),
                expiryDate = GetExpiryTime()
            };
            db.trsOTPs.Add(trsOTP);
            db.SaveChanges();

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("<otpcode>", trsOTP.otpCode);
            parameters.Add("<expiry>", GetExpiry());
            var template = db.companySettings.Where(e => e.KeyName == "logintemplate").FirstOrDefault();
            var templateId = db.companySettings.Where(e => e.KeyName == "logintemplateid").FirstOrDefault();
            if (template == null || template.ValueName == null || template.ValueName.Trim() == "" &&
                templateId == null || templateId.ValueName == null || templateId.ValueName.Trim() == "")
            {
                return null;
            }
            new NotificationUtil().SendSMS( mobileNo, template.ValueName, templateId.ValueName,  parameters);

            return new APIResponse(APIResponseStatus.success);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string GetOTPCode()
        {
            Random generator = new Random();
            String otpCode = generator.Next(0, 999999).ToString("D6");
            return otpCode;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public DateTime GetExpiryTime()
        {
            int otpValidity = 180;
            return DateTime.Now.AddSeconds(otpValidity);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [NonAction]
        public string GetExpiry()
        {
            int otpValidity = 180;
            return string.Format("{0}:{1:00}", otpValidity / 60, otpValidity % 60); ;
        }

        
    }
}

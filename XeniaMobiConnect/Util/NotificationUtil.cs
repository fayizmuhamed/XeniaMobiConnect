using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace XeniaMobiConnect.Util
{
    public class NotificationUtil
    {
        private Entities db = new Entities();
        private readonly ITraceWriter _tracer;

        public NotificationUtil()
        {
            _tracer = GlobalConfiguration.Configuration.Services.GetTraceWriter();

        }

        public string SendSMS( string mobileNo, string template,string templateId, Dictionary<string, object> parameters)
        {
            if (mobileNo == null || mobileNo == "")
            {
                _tracer.Error(null, "SendSMS", "Send SMS Failed, Reason: Mobile number not supplied");
                return "0";
            }


            var tblEmailSmsSettings = db.companySettings.Where(e => e.KeyName == "smsapi").FirstOrDefault();

            if (tblEmailSmsSettings == null || tblEmailSmsSettings.ValueName == null || tblEmailSmsSettings.ValueName.Trim() == "")
            {
                _tracer.Error(null, "SendSMS", "Send SMS Failed, Reason: SMS Gateway Not Configured");
                return "0";
            }
            try
            {

                string url = tblEmailSmsSettings.ValueName;
                url = url.Replace("<MobNo>", mobileNo);
                string message = ReplaceVariables(template, parameters);
                url = url.Replace("<Message>", message);
                url = url.Replace("<TemplateId>", templateId);

                HttpWebRequest httpreq = (HttpWebRequest)WebRequest.Create(url);

                HttpWebResponse httpres = (HttpWebResponse)httpreq.GetResponse();
                StreamReader sr = new StreamReader(httpres.GetResponseStream());
                string results = sr.ReadToEnd();
                httpres.Close();
                sr.Close();
                return results;
            }
            catch (Exception ex)
            {
                _tracer.Error(null, "SendSMS", "Send SMS Failed, Reason:" + ex.ToString());
                return "0";
            }
        }

        public string ReplaceVariables(string template, Dictionary<string, object> parameters)
        {
            foreach (var item in parameters)
            {
                template = template.Replace(item.Key, item.Value.ToString());
            }
            return template;
        }
    }
}
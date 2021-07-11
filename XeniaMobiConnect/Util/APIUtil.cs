using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;

namespace XeniaMobiConnect.Util
{
    public class APIUtil
    {
        public static int GetCustomerId(Entities db, HttpRequestHeaders httpRequestHeaders)
        {
            if (httpRequestHeaders.Contains("CustomerId"))
            {
                int customerId = Int32.Parse(httpRequestHeaders.GetValues("CustomerId").First());
                var Customer = db.mtrledgers.Where(c => c.lid == customerId && c.deActive == false).FirstOrDefault();
                if (Customer == null)
                {
                    throw new HttpException("Invalid Customer");
                }
                else
                {
                    return customerId;
                }
            }
            else
            {
                throw new HttpException("Invalid Customer");
            }

        }

        public static mtrledger GetCustomer(Entities db, HttpRequestHeaders httpRequestHeaders)
        {
            if (httpRequestHeaders.Contains("CustomerId"))
            {
                int customerId = Int32.Parse(httpRequestHeaders.GetValues("CustomerId").First());
                var Customer = db.mtrledgers.Where(c => c.lid == customerId && c.deActive == false).FirstOrDefault();
                if (Customer == null)
                {
                    throw new HttpException("Invalid Customer");
                }
                else
                {
                    return Customer;
                }
            }
            else
            {
                throw new HttpException("Invalid Customer");
            }

        }

        public static Dictionary<string, object> DictionaryFromType(object atype)
        {
            if (atype == null) return new Dictionary<string, object>();
            Type t = atype.GetType();
            PropertyInfo[] props = t.GetProperties();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (PropertyInfo prp in props)
            {
                object value = prp.GetValue(atype, new object[] { });
                dict.Add(prp.Name, value);
            }
            return dict;
        }
    }
}
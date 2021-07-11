using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XeniaMobiConnect.Controllers.Resource
{
    class APIResponseStatus
    {
        public static string failed = "failed";
        public static string success = "success";

    }

    public class APIResponse
    {
        public string Status { get; set; }

        public Object Data { get; set; }

        public string Message { get; set; }

        public int Type { get; set; }

        public APIResponse()
        {

        }

        public APIResponse(string status)
        {
            Status = status;
        }

        public APIResponse(string status, object data)
        {
            Status = status;
            Data = data;
        }

        public APIResponse(string status, object data, string message)
        {
            Status = status;
            Message = message;
            Data = data;
        }

        public APIResponse(string status, object data, string message, int type)
        {
            Status = status;
            Message = message;
            Data = data;
            Type = type;
        }
    }
}
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Tracing;
using XeniaMobiConnect.Controllers.Resource;

namespace XeniaMobiConnect.ExLogger
{
    /// <summary>  
    /// Action filter to handle for Global application errors.  
    /// </summary>  
    public class GlobalExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(ITraceWriter), new NLogger());
            var trace = GlobalConfiguration.Configuration.Services.GetTraceWriter();
            trace.Error(context.Request, "Controller : " + context.ActionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName + Environment.NewLine + "Action : " + context.ActionContext.ActionDescriptor.ActionName, context.Exception);

            var exceptionType = context.Exception.GetType();
            

            if (exceptionType == typeof(ValidationException))
            {
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.BadRequest, new APIResponse(APIResponseStatus.failed, null, context.Exception.Message)));

            }
            else if (exceptionType == typeof(UnauthorizedAccessException))
            {
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.Unauthorized, new APIResponse(APIResponseStatus.failed, null, context.Exception.Message)));
            }
            else if (exceptionType == typeof(HttpException))
            {
                var httpException = context.Exception as HttpException;
                throw new HttpResponseException(context.Request.CreateResponse((HttpStatusCode)httpException.GetHttpCode(), new APIResponse(APIResponseStatus.failed, null, httpException.Message)));
            }
            else
            {
                throw new HttpResponseException(context.Request.CreateResponse(HttpStatusCode.InternalServerError));
            }

            
        }
    }
}
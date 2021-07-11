using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.Results;
using XeniaMobiConnect.Controllers.Resource;

namespace XeniaMobiConnect.ExLogger
{
    public class ExceptionHandler : IExceptionHandler
    {
        
        public virtual Task HandleAsync(ExceptionHandlerContext context,
                                        CancellationToken cancellationToken)
        {
            if (!ShouldHandle(context))
            {
                return Task.FromResult(0);
            }

            return HandleAsyncCore(context, cancellationToken);
        }

        public virtual Task HandleAsyncCore(ExceptionHandlerContext context,
                                           CancellationToken cancellationToken)
        {
            HandleCore(context);
            return Task.FromResult(0);
        }

        public virtual void HandleCore(ExceptionHandlerContext context)
        {
        }

        public virtual bool ShouldHandle(ExceptionHandlerContext context)
        {
            return context.CatchBlock.IsTopLevel;
        }
    }

    
    public class CustomExceptionHandler:ExceptionHandler
    {
        public override void HandleCore(ExceptionHandlerContext context)
        {
            var exception = context.Exception;

            var httpException = exception as HttpException;
            if (httpException != null)
            {
                
                context.Result = new ResponseMessageResult(context.Request.CreateResponse<APIResponse>((HttpStatusCode)httpException.GetHttpCode(), new APIResponse(APIResponseStatus.failed, null, httpException.Message, httpException.GetHttpCode())));
                return;
            }

            // Return HttpStatusCode for other types of exception.

            context.Result = new ResponseMessageResult(context.Request.CreateResponse<APIResponse>(HttpStatusCode.InternalServerError, new APIResponse(APIResponseStatus.failed, null, exception.Message)));

            
        }
    }
}
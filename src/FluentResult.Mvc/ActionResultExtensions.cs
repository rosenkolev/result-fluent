using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

using FluentResult;

#if NETCOREAPP3_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Controller = Microsoft.AspNetCore.Mvc.ControllerBase;
#elif NET47_OR_GREATER
using System.Net;
using System.Web.Http.Results;

using Controller = System.Web.Http.ApiController;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

namespace FluentResult
{
    /// <summary>A MVC controller extensions.</summary>
    [ExcludeFromCodeCoverage]
    public static class ActionResultExtensions
    {
        /// <summary>Create a HTTP action result from a service result.</summary>
        /// <typeparam name="T">The type of the result data.</typeparam>
        public static IActionResult ToActionResult<T>(this Result<T> result, Controller controller) =>
#if NETCOREAPP3_0_OR_GREATER
            result?.Status switch
            {
                ResultComplete.Success => controller.Ok(result),
                ResultComplete.NotFound => controller.NotFound(result),
                ResultComplete.InvalidArgument => controller.BadRequest(result),
                ResultComplete.OperationFailed => controller.StatusCode(StatusCodes.Status500InternalServerError, result),
                ResultComplete.Conflict => controller.Conflict(result),
                _ => throw new NotSupportedException()
            };
#elif NET47_OR_GREATER
        result?.Status switch
            {
                ResultComplete.Success => new OkNegotiatedContentResult<Result<T>>(result, controller),
                ResultComplete.NotFound => new NegotiatedContentResult<Result<T>>(HttpStatusCode.NotFound, result, controller),
                ResultComplete.InvalidArgument => new NegotiatedContentResult<Result<T>>(HttpStatusCode.BadRequest, result, controller),
                ResultComplete.OperationFailed => new NegotiatedContentResult<Result<T>>(HttpStatusCode.InternalServerError, result, controller),
                ResultComplete.Conflict => new NegotiatedContentResult<Result<T>>(HttpStatusCode.Conflict, result, controller),
                _ => throw new NotSupportedException()
            };
#endif

        /// <summary>Create a HTTP action result from a service result asynchronously.</summary>
        /// <typeparam name="T">The type of the result data.</typeparam>
        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<Result<T>> taskResult, Controller controller) =>
            ToActionResult(await taskResult, controller);

        /// <summary>Create a HTTP action result from a service result of items asynchronously.</summary>
        /// <typeparam name="T">The type of the result data.</typeparam>
        public static async Task<IActionResult> ToActionResultAsync<T>(this Task<ResultOfItems<T>> result, Controller controller) =>
            ToActionResult(await result, controller);
    }
}

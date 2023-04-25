using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FluentResult
{
    /// <summary>Extensions related to result validation.</summary>
    public static class ResultValidationExtensions
    {
        /// <summary>Resolve the result data when it is valid status code.</summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <remarks>
        /// <code>Result.Create(5).AsValidData(); <i>// 5</i></code>
        /// </remarks>
        /// <exception cref="ResultValidationException">When status code is not `Success`.</exception>
        [DebuggerStepThrough]
        public static T AsValidData<T>(this Result<T> result)
        {
            if (!result.IsSuccessfulStatus())
            {
                throw new ResultValidationException(result.Status, result.Messages ?? Array.Empty<string>());
            }

            return result.Data;
        }

        /// <summary>Resolve the result data when it is valid status code.</summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <remarks>
        /// <code>Result.Create(5).MapAsync(Task.FromResult).AsValidDataAsync(); <i>// 5</i></code>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When status code is not `Success`.</exception>
        [DebuggerStepThrough]
        public static async Task<T> AsValidDataAsync<T>(this Task<Result<T>> asyncResult) =>
            AsValidData(await asyncResult);

        /// <summary>Resolve the <see cref="ResultOfItems{TItem}"/> data, when it is valid status code.</summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <remarks>
        /// <code>
        /// IReadOnlyCollection&lt;int&gt; data = new [] { 1 };
        /// Task.Create(data).MapAsync(Task.FromResult).ToResultOfItemsAsync().AsValidDataAsync(); <i>// [1]</i>
        /// </code>
        /// </remarks>
        /// <exception cref="InvalidOperationException">When status code is not `Success`.</exception>
        [DebuggerStepThrough]
        public static async Task<IEnumerable<T>> AsValidDataAsync<T>(this Task<ResultOfItems<T>> asyncResult) =>
            AsValidData(await asyncResult);
    }
}

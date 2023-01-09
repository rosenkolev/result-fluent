using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FluentResult
{
    /// <summary>Switch extensions to Result.</summary>
    public static class SwitchExtensions
    {
        /// <summary>Convert to another result.</summary>
        /// <typeparam name="TIn">The input object.</typeparam>
        /// <typeparam name="TOut">The output object.</typeparam>
        [DebuggerStepThrough]
        public static Result<TOut> To<TIn, TOut>(this Result<TIn> result, TOut defaultValue) =>
            new Result<TOut>(defaultValue, result.Status, result.Messages);

        /// <summary>Switch the result to a new result.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TModel">The model object.</typeparam>
        [DebuggerStepThrough]
        public static Result<TModel> Switch<TEntity, TModel>(this Result<TEntity> result, Func<TEntity, Result<TModel>> converter) =>
            result.Status == ResultComplete.Success ?
            converter(result.Data) :
            new Result<TModel>(default!, result.Status, result.Messages);

        /// <summary>Switch the result to a new result asynchronous.</summary>
        /// <typeparam name="TIn">The input object.</typeparam>
        /// <typeparam name="TOut">The output object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TOut>> SwitchAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> pipelineAction) =>
            result.Status == ResultComplete.Success ?
            await pipelineAction(result.Data) :
            new Result<TOut>(default!, result.Status, result.Messages);

        /// <summary>Switch the result to a new result asynchronous.</summary>
        /// <typeparam name="TIn">The input object.</typeparam>
        /// <typeparam name="TOut">The output object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TOut>> SwitchAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> pipelineAction) =>
            await SwitchAsync(await resultTask, pipelineAction);

        /// <summary>Resolve the result data when it is valid status code.</summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <exception cref="InvalidOperationException">When status code is not `Success`.</exception>
        [DebuggerStepThrough]
        public static T AsValidData<T>(this Result<T> result)
        {
            if (!result.IsSuccessfulStatus())
            {
                throw new InvalidOperationException($"Invalid status code '{result.Status}'.");
            }

            return result.Data;
        }

        /// <summary>Resolve the result data when it is valid status code.</summary>
        /// <typeparam name="T">The type of the data.</typeparam>
        /// <exception cref="InvalidOperationException">When status code is not `Success`.</exception>
        [DebuggerStepThrough]
        public static async Task<T> AsValidDataAsync<T>(this Task<Result<T>> asyncResult) =>
            AsValidData(await asyncResult);

        /// <summary>Handle exceptions in async call.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TEntity>> CatchAsync<TEntity>(this Task<Result<TEntity>> entityTask, Func<Exception, Result<TEntity>> onError)
        {
            try
            {
                return await entityTask;
            }
            catch (Exception ex)
            {
                return onError(ex);
            }
        }
    }
}

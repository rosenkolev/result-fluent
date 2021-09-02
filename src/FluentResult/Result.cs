using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FluentResult
{
    /// <summary>A static result class.</summary>
    public static class Result
    {
        /// <summary>Create an Result from data.</summary>
        /// <typeparam name="TResult">The type of the result data.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> Create<TResult>(TResult data) =>
            new Result<TResult>(data, ResultComplete.Success, null);

        /// <summary>Create an Result with single object.</summary>
        /// <typeparam name="TResult">The type of the single result data.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> CreateResultWithError<TResult>(ResultComplete status, params string[] messages) =>
            new Result<TResult>(default, status, messages);

        /// <summary>Switch the result to a new result.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TModel">The model object.</typeparam>
        [DebuggerStepThrough]
        public static Result<TModel> Switch<TEntity, TModel>(this Result<TEntity> result, Func<TEntity, Result<TModel>> converter) =>
            result.Status == ResultComplete.Success ?
            converter?.Invoke(result.Data) :
            new Result<TModel>(default, result.Status, result.Messages);

        /// <summary>Switch the result to a new result asynchronous.</summary>
        /// <typeparam name="TIn">The input object.</typeparam>
        /// <typeparam name="TOut">The output object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TOut>> SwitchAsync<TIn, TOut>(this Result<TIn> result, Func<TIn, Task<Result<TOut>>> pipelineAction) =>
            result.Status == ResultComplete.Success ?
            await pipelineAction?.Invoke(result.Data) :
            new Result<TOut>(default, result.Status, result.Messages);

        /// <summary>Switch the result to a new result asynchronous.</summary>
        /// <typeparam name="TIn">The input object.</typeparam>
        /// <typeparam name="TOut">The output object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TOut>> SwitchAsync<TIn, TOut>(this Task<Result<TIn>> resultTask, Func<TIn, Task<Result<TOut>>> pipelineAction) =>
            await SwitchAsync(await resultTask, pipelineAction);

        /// <summary>Convert entity result to model result.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TModel">The model object.</typeparam>
        [DebuggerStepThrough]
        public static Result<TModel> Map<TEntity, TModel>(this Result<TEntity> entity, Func<TEntity, TModel> converter) =>
            entity.Status == ResultComplete.Success ?
            Create(converter(entity.Data)) :
            new Result<TModel>(default, entity.Status, entity.Messages);

        /// <summary>Execute a asynchronous operation.</summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <typeparam name="TSource">The source type.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> MapAsync<TResult, TSource>(this Result<TSource> result, Func<TSource, Task<TResult>> pipelineAction)
        {
            if (result.Status == ResultComplete.Success)
            {
                var actionResult = await pipelineAction?.Invoke(result.Data);
                return Create(actionResult);
            }

            return new Result<TResult>(default, result.Status, result.Messages);
        }

        /// <summary>Convert an result entity to model.</summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TModel>> MapAsync<TEntity, TModel>(this Task<Result<TEntity>> entityTask, Func<TEntity, TModel> converter) =>
            await MapAsync(await entityTask, entity => Task.FromResult(converter(entity)));

        /// <summary>Convert an result entity to model.</summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TModel>> MapAsync<TEntity, TModel>(this Task<Result<TEntity>> entityTask, Func<TEntity, Task<TModel>> converterAsync) =>
            await MapAsync(await entityTask, converterAsync);

        /// <summary>Convert entity list result to model list result.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TModel">The model object.</typeparam>
        [DebuggerStepThrough]
        public static Result<IReadOnlyList<TModel>> MapList<TEntity, TModel>(this Result<TEntity[]> entity, Func<TEntity, TModel> converter) =>
            entity.Map(it => MapItems(it, converter));

        /// <summary>Convert entity list result to model list result.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TModel">The model object.</typeparam>
        [DebuggerStepThrough]
        public static Result<IReadOnlyList<TModel>> MapList<TEntity, TModel>(this Result<IReadOnlyList<TEntity>> entity, Func<TEntity, TModel> converter) =>
            entity.Map(it => MapItems(it, converter));

        /// <summary>Convert entity list result to model list result.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TModel">The model object.</typeparam>
        [DebuggerStepThrough]
        public static Result<IReadOnlyList<TModel>> MapList<TEntity, TModel>(this Result<IEnumerable<TEntity>> entity, Func<TEntity, TModel> converter) =>
            entity.Map(it => MapItems(it, converter));

        /// <summary>Validates the specified condition.</summary>
        [DebuggerStepThrough]
        public static Result<bool> Validate(bool condition, ResultComplete status, string message) =>
            condition ? Create(true) : CreateResultWithError<bool>(status, message);

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> Validate<TResult>(
            this Result<TResult> result,
            Predicate<TResult> predicate,
            ResultComplete status,
            string message,
            bool skipOnInvalidResult) =>
            (skipOnInvalidResult && !result.IsSuccessfulStatus()) || (predicate?.Invoke(result.Data) ?? false)
            ? result
            : CreateResultWithError<TResult>(status, CombineArray(result.Messages, message));

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> Validate<TResult>(
            this Result<TResult> result,
            bool condition,
            ResultComplete status,
            string message,
            bool skipOnInvalidResult) =>
            Validate(result, _ => condition, status, message, skipOnInvalidResult);

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> Validate<TResult>(this Result<TResult> result, bool condition, ResultComplete status, string message) =>
            Validate(result, condition, status, message, skipOnInvalidResult: false);

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> Validate<TResult>(
            this Result<TResult> result,
            Predicate<TResult> predicate,
            ResultComplete status,
            string message) =>
            Validate(result, predicate, status, message, skipOnInvalidResult: false);

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateAsync<TResult>(
            this Task<Result<TResult>> entityTask,
            Predicate<TResult> predicate,
            ResultComplete status,
            string message,
            bool skipOnInvalidResult) =>
            Validate(await entityTask, predicate, status, message, skipOnInvalidResult);

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateAsync<TResult>(
            this Task<Result<TResult>> entityTask,
            Predicate<TResult> predicate,
            ResultComplete status,
            string message) =>
            Validate(await entityTask, predicate, status, message, skipOnInvalidResult: false);

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateAsync<TResult>(
            this Task<Result<TResult>> entityTask,
            Func<TResult, Task<bool>> predicateAsync,
            ResultComplete status,
            string message)
        {
            var result = await entityTask;
            return Validate(result, await predicateAsync?.Invoke(result.Data), status, message, skipOnInvalidResult: false);
        }

        /// <summary>Create a successful result of items.</summary>
        /// <typeparam name="TEntity">The type of the data.</typeparam>
        [DebuggerStepThrough]
        public static ResultOfItems<TEntity> CreateResultOfItems<TEntity>(
            IReadOnlyCollection<TEntity> items,
            int? totalCount,
            int? pageSize = null,
            int? pageIndex = null) =>
            new ResultOfItems<TEntity>(items, ResultComplete.Success, null, totalCount, pageSize, pageIndex, items.Count);

        /// <summary>Convert an result entity to result of items.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<ResultOfItems<TEntity>> ToResultOfItemsAsync<TEntity>(
            this Result<IReadOnlyList<TEntity>> result,
            Func<IReadOnlyList<TEntity>, Task<ResultOfItems<TEntity>>> converterAsync) =>
            result.Status == ResultComplete.Success ?
            await converterAsync?.Invoke(result.Data) :
            new ResultOfItems<TEntity>(default, result.Status, result.Messages);

        /// <summary>Convert an result entity to result of items.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<ResultOfItems<TEntity>> ToResultOfItemsAsync<TEntity>(
            this Task<Result<IReadOnlyList<TEntity>>> resultAsync,
            Func<IReadOnlyList<TEntity>, Task<ResultOfItems<TEntity>>> converterAsync) =>
            await ToResultOfItemsAsync(await resultAsync, converterAsync);

        private static IReadOnlyList<TModel> MapItems<TEntity, TModel>(IEnumerable<TEntity> entities, Func<TEntity, TModel> converter) =>
            entities.Select(converter).ToList();

        private static string[] CombineArray(IEnumerable<string> messages, string message)
        {
            var messageArray = new[] { message };
            return messages?.Concat(messageArray).ToArray() ?? messageArray;
        }
    }
}
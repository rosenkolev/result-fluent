using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FluentResult
{
    /// <summary>ResultOfItems extension methods.</summary>
    public static class ResultOfItemsExtensions
    {
        /// <summary>Convert an result entity to result of items.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TCollection">The entity collection.</typeparam>
        [DebuggerStepThrough]
        public static ResultOfItems<TEntity> ToResultOfItems<TEntity, TCollection>(
            this Result<TCollection> result,
            Func<TCollection, ResultOfItems<TEntity>> converter)
            where TCollection : IReadOnlyCollection<TEntity> =>
            result.Status == ResultComplete.Success ?
            converter(result.Data) :
            new ResultOfItems<TEntity>(default!, result.Status, result.Messages);

        /// <summary>Convert an result entity to result of items.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TCollection">The entity collection.</typeparam>
        [DebuggerStepThrough]
        public static async Task<ResultOfItems<TEntity>> ToResultOfItemsAsync<TEntity, TCollection>(
            this Result<TCollection> result,
            Func<TCollection, Task<ResultOfItems<TEntity>>> converterAsync)
            where TCollection : IReadOnlyCollection<TEntity> =>
            result.Status == ResultComplete.Success ?
            await converterAsync(result.Data) :
            new ResultOfItems<TEntity>(default!, result.Status, result.Messages);

        /// <summary>Convert an result entity to result of items.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TCollection">The entity collection.</typeparam>
        [DebuggerStepThrough]
        public static async Task<ResultOfItems<TEntity>> ToResultOfItemsAsync<TEntity, TCollection>(
            this Task<Result<TCollection>> resultAsync,
            Func<TCollection, Task<ResultOfItems<TEntity>>> converterAsync)
            where TCollection : IReadOnlyCollection<TEntity> =>
            await ToResultOfItemsAsync(await resultAsync, converterAsync);

        /// <summary>Convert an result entity to result of items.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TCollection">The entity collection.</typeparam>
        [DebuggerStepThrough]
        public static async Task<ResultOfItems<TEntity>> ToResultOfItemsAsync<TEntity, TCollection>(
            this Task<Result<TCollection>> resultAsync,
            Func<TCollection, ResultOfItems<TEntity>> converter)
            where TCollection : IReadOnlyCollection<TEntity> =>
            ToResultOfItems(await resultAsync, converter);

        /// <summary>Convert an result entity to result of items.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<ResultOfItems<TEntity>> ToResultOfItemsAsync<TEntity>(
            this Task<Result<IReadOnlyCollection<TEntity>>> resultAsync)=>
            ToResultOfItems(await resultAsync, items => Result.CreateResultOfItems(items, items.Count));

        /// <summary>Convert an result entity to result of items.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        [DebuggerStepThrough]
        public static async Task<ResultOfItems<TEntity>> ToResultOfItemsAsync<TEntity>(
            this Task<Result<IReadOnlyList<TEntity>>> resultAsync) =>
            ToResultOfItems(await resultAsync, items => Result.CreateResultOfItems(items, items.Count));
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FluentResult
{
    /// <summary>Map extensions to Result.</summary>
    public static class MapExtensions
    {
        /// <summary>Convert entity result to model result.</summary>
        /// <typeparam name="TEntity">The entity object.</typeparam>
        /// <typeparam name="TModel">The model object.</typeparam>
        [DebuggerStepThrough]
        public static Result<TModel> Map<TEntity, TModel>(this Result<TEntity> entity, Func<TEntity, TModel> converter) =>
            entity.Status == ResultComplete.Success ?
            Result.Create(converter(entity.Data)) :
            new Result<TModel>(default!, entity.Status, entity.Messages);

        /// <summary>Execute a asynchronous operation.</summary>
        /// <typeparam name="TResult">The result type.</typeparam>
        /// <typeparam name="TSource">The source type.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> MapAsync<TResult, TSource>(this Result<TSource> result, Func<TSource, Task<TResult>> pipelineAction)
        {
            if (result.Status == ResultComplete.Success)
            {
                var actionResult = await pipelineAction(result.Data);
                return Result.Create(actionResult);
            }

            return new Result<TResult>(default!, result.Status, result.Messages);
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

        private static IReadOnlyList<TModel> MapItems<TEntity, TModel>(IEnumerable<TEntity> entities, Func<TEntity, TModel> converter) =>
            entities.Select(converter).ToList();
    }
}

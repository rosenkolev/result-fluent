using System.Collections.Generic;
using System.Diagnostics;

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

        /// <summary>Create an Result from data.</summary>
        /// <typeparam name="TResult">The type of the result data.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> Create<TResult>(TResult data, string message) =>
            new Result<TResult>(data, ResultComplete.Success, new [] { message });

        /// <summary>Create an Result with single object.</summary>
        /// <typeparam name="TResult">The type of the single result data.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> CreateResultWithError<TResult>(ResultComplete status, params string[] messages) =>
            new Result<TResult>(default!, status, messages);

        /// <summary>Validates the specified condition.</summary>
        [DebuggerStepThrough]
        public static Result<bool> Validate(bool condition, ResultComplete status, string message) =>
            condition ? Create(true) : CreateResultWithError<bool>(status, message);

        /// <summary>Create a successful result of items.</summary>
        /// <typeparam name="TEntity">The type of the data.</typeparam>
        [DebuggerStepThrough]
        public static ResultOfItems<TEntity> CreateResultOfItems<TEntity>(
            IReadOnlyCollection<TEntity> items,
            int? totalCount,
            int? pageSize = null,
            int? pageIndex = null) =>
            new ResultOfItems<TEntity>(items, ResultComplete.Success, null, totalCount, pageSize, pageIndex, items.Count);
    }
}

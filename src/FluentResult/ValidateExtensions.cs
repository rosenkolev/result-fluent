using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FluentResult
{
    /// <summary>Validate and ValidateAsync extensions to Result.</summary>
    public static class ValidateExtensions
    {
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
            : new Result<TResult>(result.Data, status, CombineArray(result.Messages, message));

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
        public static Result<TResult> Validate<TResult>(
            this Result<TResult> result,
            bool condition,
            ResultComplete status,
            string message) =>
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

        /// <summary>Validate result data is not null.</summary>
        /// <typeparam name="TResult">The type of the result data.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> Validate<TResult>(
            this Result<TResult> result,
            Predicate<TResult> predicate,
            ResultComplete status,
            Func<TResult, string> messageFunc,
            bool skipOnInvalidResult) =>
            Validate(result, predicate, status, messageFunc(result.Data), skipOnInvalidResult);

        /// <summary>Validate result data is not null.</summary>
        /// <typeparam name="TResult">The type of the result data.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> Validate<TResult>(
            this Result<TResult> result,
            Predicate<TResult> predicate,
            ResultComplete status,
            Func<TResult, string> messageFunc) =>
            Validate(result, predicate, status, messageFunc(result.Data), skipOnInvalidResult: false);

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

        /// <summary>Validate a result asynchronous.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateAsync<TResult>(
            this Result<TResult> result,
            Func<TResult, Task<bool>> predicateAsync,
            ResultComplete status,
            string message,
            bool skipOnInvalidResult = true) =>
            Validate(
                result,
                !(!result.IsSuccessfulStatus() && skipOnInvalidResult) && await predicateAsync(result.Data),
                status,
                message,
                skipOnInvalidResult);

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
            return Validate(result, await predicateAsync(result.Data), status, message, skipOnInvalidResult: false);
        }

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateAsync<TResult>(
            this Task<Result<TResult>> entityTask,
            Predicate<TResult> predicate,
            ResultComplete status,
            Func<TResult, string> messageFunc)
        {
            var result = await entityTask;
            return Validate(result, predicate, status, messageFunc(result.Data));
        }

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateAsync<TResult>(
            this Task<Result<TResult>> entityTask,
            Predicate<TResult> predicate,
            ResultComplete status,
            Func<TResult, string> messageFunc,
            bool skipOnInvalidResult)
        {
            var result = await entityTask;
            return Validate(result, predicate, status, messageFunc(result.Data), skipOnInvalidResult);
        }

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateAsync<TResult>(
            this Task<Result<TResult>> entityTask,
            Func<TResult, Task<bool>> predicateAsync,
            ResultComplete status,
            Func<TResult, string> messageFunc)
        {
            var result = await entityTask;
            return Validate(result, await predicateAsync(result.Data), status, messageFunc(result.Data));
        }

        /// <summary>Validates the specified condition.</summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateAsync<TResult>(
            this Task<Result<TResult>> entityTask,
            Func<TResult, Task<bool>> predicateAsync,
            ResultComplete status,
            Func<TResult, string> messageFunc,
            bool skipOnInvalidResult)
        {
            var result = await entityTask;
            return Validate(result, await predicateAsync(result.Data), status, messageFunc(result.Data), skipOnInvalidResult);
        }

        /// <summary>Validate result data is not null.</summary>
        /// <typeparam name="TResult">The type of the result data.</typeparam>
        [DebuggerStepThrough]
        public static Result<TResult> ValidateNotNull<TResult>(
            this Result<TResult?> result,
            ResultComplete status,
            string message,
            bool skipOnInvalidResult = true)
            where TResult : class
        {
            if (skipOnInvalidResult && !result.IsSuccessfulStatus())
            {
                return new Result<TResult>(default!, result.Status, result.Messages);
            }

            return result.Data is null ?
                new Result<TResult>(default!, status, CombineArray(result.Messages, message)) :
                new Result<TResult>(result.Data, result.Status, result.Messages);
        }

        /// <summary>Validate result data is not null.</summary>
        /// <typeparam name="TResult">The type of the result data.</typeparam>
        [DebuggerStepThrough]
        public static async Task<Result<TResult>> ValidateNotNullAsync<TResult>(
            this Task<Result<TResult?>> entityTask,
            ResultComplete status,
            string message,
            bool skipOnInvalidResult = true)
            where TResult : class =>
            ValidateNotNull(await entityTask, status, message, skipOnInvalidResult);

        private static string[] CombineArray(IEnumerable<string>? messages, string message)
        {
            var messageArray = new[] { message };
            if (messages == null)
            {
                return messageArray;
            }

            return messages.Concat(messageArray).ToArray();
        }
    }
}

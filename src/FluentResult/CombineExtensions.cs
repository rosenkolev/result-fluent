using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FluentResult
{
    /// <summary>Combine extensions to Result.</summary>
    public static class CombineExtensions
    {
        /// <summary>Combine two results into a single result.</summary>
        /// <remarks>
        /// <code>
        /// var result = Result.Create(user)
        ///   .Combine(
        ///     user => _roleRepository.GetByRoleId(user.roleId),
        ///     (user, role) => $"User {user.Name} has role {role.Name}.");
        /// </code>
        /// <i>User Jhon has role Administrator</i>.
        /// </remarks>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Result<TOut> Combine<TIn, T1, TOut>(
            this Result<TIn> result,
            Func<TIn, Result<T1>> requests,
            Func<TIn, T1, TOut> map) =>
            Result.Switch(
                result,
                data => CombineResults(data, requests(data), map));

        /// <summary>Combine three Results into a single Result.</summary>
        /// A simple example:
        /// <code>
        /// var result = Result.Create(await GetUserById(5))
        ///   .Combine(
        ///     user => (
        ///       _roleRepository.GetByRoleId(user.roleId),
        ///       _friendsRepository.GetFriendsCountByUserId(user.UserId)),
        ///     (user, role, friendsCount) => $"User {user.Name} with role {role.Name} has {friendsCount} friends.");
        /// </code>
        /// <i>User Jhon with role Administrator has 23 friends</i>.
        /// </remarks>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Result<TOut> Combine<TIn, T1, T2, TOut>(
            this Result<TIn> result,
            Func<TIn, (Result<T1>, Result<T2>)> requests,
            Func<TIn, T1, T2, TOut> map) =>
            result.Switch(
                data =>
                {
                    var (r1, r2) = requests(data);
                    return CombineResults(data, r1, r2, map);
                });

        /// <summary>Combine four Results into a single Result.</summary>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="T3">The type of the third result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Result<TOut> Combine<TIn, T1, T2, T3, TOut>(
            this Result<TIn> result,
            Func<TIn, (Result<T1>, Result<T2>, Result<T3>)> requests,
            Func<TIn, T1, T2, T3, TOut> map) =>
            result.Switch(
                data =>
                {
                    var (r1, r2, r3) = requests(data);
                    return CombineResults(data, r1, r2, r3, map);
                });

        /// <summary>Combine five Results into a single Result.</summary>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="T3">The type of the third result.</typeparam>
        /// <typeparam name="T4">The type of the fourth result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Result<TOut> Combine<TIn, T1, T2, T3, T4, TOut>(
            this Result<TIn> result,
            Func<TIn, (Result<T1>, Result<T2>, Result<T3>, Result<T4>)> requests,
            Func<TIn, T1, T2, T3, T4, TOut> map) =>
            result.Switch(
                data =>
                {
                    var (r1, r2, r3, r4) = requests(data);
                    return CombineResults(data, r1, r2, r3, r4, map);
                });

        /// <summary>Combine many Results into a single Result.</summary>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="T3">The type of the third result.</typeparam>
        /// <typeparam name="T4">The type of the fourth result.</typeparam>
        /// <typeparam name="T5">The type of the sixth result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Result<TOut> Combine<TIn, T1, T2, T3, T4, T5, TOut>(
            this Result<TIn> result,
            Func<TIn, (Result<T1>, Result<T2>, Result<T3>, Result<T4>, Result<T5>)> requests,
            Func<TIn, T1, T2, T3, T4, T5, TOut> map) =>
            result.Switch(
                data =>
                {
                    var (r1, r2, r3, r4, r5) = requests(data);
                    return CombineResults(data, r1, r2, r3, r4, r5, map);
                });

        /// <summary>Combine two results into a single result.</summary>
        /// <remarks>
        /// A simple example:
        /// <code>
        /// var result = Result.Create(user)
        ///   .CombineAsync(
        ///     user => _roleRepository.GetByRoleIdAsync(user.roleId),
        ///     (user, role) => $"User {user.Name} has role {role.Name}.");
        /// </code>
        /// <i>User Jhon has role Administrator</i>.
        /// </remarks>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Task<Result<TOut>> CombineAsync<TIn, T1, TOut>(
            this Task<Result<TIn>> resultAsync,
            Func<TIn, Task<Result<T1>>> requests,
            Func<TIn, T1, TOut> map) =>
            Result.SwitchAsync(
                resultAsync,
                async data =>
                {
                    var r1 = await requests(data).ConfigureAwait(false);
                    return CombineResults(data, r1, map);
                });

        /// <summary>Combine three Results into a single Result.</summary>
        /// <remarks>
        /// A simple example:
        /// <code>
        /// var result = Result.Create(await GetUserById(5))
        ///   .CombineAsync(
        ///     user => (
        ///       _roleRepository.GetByRoleIdAsync(user.roleId),
        ///       _friendsRepository.GetFriendsCountByUserIdAsync(user.UserId)),
        ///     (user, role, friendsCount) => $"User {user.Name} with role {role.Name} has {friendsCount} friends.");
        /// </code>
        /// <i>User Jhon with role Administrator has 23 friends</i>.
        /// </remarks>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Task<Result<TOut>> CombineAsync<TIn, T1, T2, TOut>(
            this Task<Result<TIn>> resultAsync,
            Func<TIn, (Task<Result<T1>>, Task<Result<T2>>)> requests,
            Func<TIn, T1, T2, TOut> map) =>
            resultAsync.SwitchAsync(
                async data =>
                {
                    var (r1Async, r2Async) = requests(data);
                    await Task.WhenAll(r1Async, r2Async);
                    return CombineResults(data, r1Async.Result, r2Async.Result, map);
                });

        /// <summary>Combine four Results into a single Result.</summary>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="T3">The type of the third result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Task<Result<TOut>> CombineAsync<TIn, T1, T2, T3, TOut>(
            this Task<Result<TIn>> resultAsync,
            Func<TIn, (Task<Result<T1>>, Task<Result<T2>>, Task<Result<T3>>)> requests,
            Func<TIn, T1, T2, T3, TOut> map) =>
            resultAsync.SwitchAsync(
                async data =>
                {
                    var (r1Async, r2Async, r3Async) = requests(data);
                    await Task.WhenAll(r1Async, r2Async, r3Async);
                    return CombineResults(data, r1Async.Result, r2Async.Result, r3Async.Result, map);
                });

        /// <summary>Combine many Results into a single Result.</summary>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="T3">The type of the third result.</typeparam>
        /// <typeparam name="T4">The type of the fourth result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Task<Result<TOut>> CombineAsync<TIn, T1, T2, T3, T4, TOut>(
            this Task<Result<TIn>> resultAsync,
            Func<TIn, (Task<Result<T1>>, Task<Result<T2>>, Task<Result<T3>>, Task<Result<T4>>)> requests,
            Func<TIn, T1, T2, T3, T4, TOut> map) =>
            resultAsync.SwitchAsync(
                async data =>
                {
                    var (r1Async, r2Async, r3Async, r4Async) = requests(data);
                    await Task.WhenAll(r1Async, r2Async, r3Async, r4Async);
                    return CombineResults(data, r1Async.Result, r2Async.Result, r3Async.Result, r4Async.Result, map);
                });

        /// <summary>Combine many Results into a single Result.</summary>
        /// <typeparam name="TIn">The source result type.</typeparam>
        /// <typeparam name="T1">The type of the first result.</typeparam>
        /// <typeparam name="T2">The type of the second result.</typeparam>
        /// <typeparam name="T3">The type of the third result.</typeparam>
        /// <typeparam name="T4">The type of the fourth result.</typeparam>
        /// <typeparam name="T5">The type of the fifth result.</typeparam>
        /// <typeparam name="TOut">The destination result type.</typeparam>
        [DebuggerStepThrough]
        public static Task<Result<TOut>> CombineAsync<TIn, T1, T2, T3, T4, T5, TOut>(
            this Task<Result<TIn>> resultAsync,
            Func<TIn, (Task<Result<T1>>, Task<Result<T2>>, Task<Result<T3>>, Task<Result<T4>>, Task<Result<T5>>)> requests,
            Func<TIn, T1, T2, T3, T4, T5, TOut> map) =>
            resultAsync.SwitchAsync(
                async data =>
                {
                    var (r1Async, r2Async, r3Async, r4Async, r5Async) = requests(data);
                    await Task.WhenAll(r1Async, r2Async, r3Async, r4Async, r5Async);
                    return CombineResults(data, r1Async.Result, r2Async.Result, r3Async.Result, r4Async.Result, r5Async.Result, map);
                });

        private static Result<TOut> CombineResults<TIn, T1, TOut>(
            TIn data,
            Result<T1> r1,
            Func<TIn, T1, TOut> map) =>
            !r1.IsSuccessfulStatus()
                ? Result.To(r1, default(TOut))
                : Result.Create(map(data, r1.Data));

        private static Result<TOut> CombineResults<TIn, T1, T2, TOut>(
            TIn data,
            Result<T1> r1,
            Result<T2> r2,
            Func<TIn, T1, T2, TOut> map)
        {
            if (!r1.IsSuccessfulStatus())
            {
                return r1.To(default(TOut)!);
            }
            else if (!r2.IsSuccessfulStatus())
            {
                return r2.To(default(TOut)!);
            }

            return Result.Create(map(data, r1.Data, r2.Data));
        }

        private static Result<TOut> CombineResults<TIn, T1, T2, T3, TOut>(
            TIn data,
            Result<T1> r1,
            Result<T2> r2,
            Result<T3> r3,
            Func<TIn, T1, T2, T3, TOut> map)
        {
            if (!r1.IsSuccessfulStatus())
            {
                return r1.To(default(TOut)!);
            }
            else if (!r2.IsSuccessfulStatus())
            {
                return r2.To(default(TOut)!);
            }
            else if (!r3.IsSuccessfulStatus())
            {
                return r3.To(default(TOut)!);
            }

            return Result.Create(map(data, r1.Data, r2.Data, r3.Data));
        }

        private static Result<TOut> CombineResults<TIn, T1, T2, T3, T4, TOut>(
            TIn data,
            Result<T1> r1,
            Result<T2> r2,
            Result<T3> r3,
            Result<T4> r4,
            Func<TIn, T1, T2, T3, T4, TOut> map)
        {
            if (!r1.IsSuccessfulStatus())
            {
                return r1.To(default(TOut)!);
            }
            else if (!r2.IsSuccessfulStatus())
            {
                return r2.To(default(TOut)!);
            }
            else if (!r3.IsSuccessfulStatus())
            {
                return r3.To(default(TOut)!);
            }
            else if (!r4.IsSuccessfulStatus())
            {
                return r4.To(default(TOut)!);
            }

            return Result.Create(map(data, r1.Data, r2.Data, r3.Data, r4.Data));
        }

        private static Result<TOut> CombineResults<TIn, T1, T2, T3, T4, T5, TOut>(
            TIn data,
            Result<T1> r1,
            Result<T2> r2,
            Result<T3> r3,
            Result<T4> r4,
            Result<T5> r5,
            Func<TIn, T1, T2, T3, T4, T5, TOut> map)
        {
            if (!r1.IsSuccessfulStatus())
            {
                return r1.To(default(TOut)!);
            }
            else if (!r2.IsSuccessfulStatus())
            {
                return r2.To(default(TOut)!);
            }
            else if (!r3.IsSuccessfulStatus())
            {
                return r3.To(default(TOut)!);
            }
            else if (!r4.IsSuccessfulStatus())
            {
                return r4.To(default(TOut)!);
            }
            else if (!r5.IsSuccessfulStatus())
            {
                return r5.To(default(TOut)!);
            }

            return Result.Create(map(data, r1.Data, r2.Data, r3.Data, r4.Data, r5.Data));
        }
    }
}

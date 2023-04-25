using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentResult.Tests
{
    [TestClass]
    public class SwitchTests
    {
        [TestMethod]
        public void Switch()
        {
            Result<int> secondFn(int model) =>
                Result.Create(model).Map(it => it + 10);

            var result = Result
                .Create(3)
                .Map(it => it * 2)
                .Switch(secondFn);

            Assert.AreEqual(16, result.Data);
        }

        [TestMethod]
        public async Task SwitchAsync()
        {
            Task<Result<int>> secondFnAsync(int model) =>
                Result.Create(model).MapAsync(it => Task.FromResult(it + 12));

            var result = await Result
                .Create(3)
                .Map(it => it * 2)
                .SwitchAsync(secondFnAsync);

            Assert.AreEqual(18, result.Data);
        }

        [TestMethod]
        public async Task SwitchAsyncToAsync()
        {
            Task<Result<int>> secondFnAsync(int model) =>
                Result.Create(model).MapAsync(it => Task.FromResult(it + 15));

            var result = await Result
                .Create(3)
                .MapAsync(it => Task.FromResult(it * 2))
                .SwitchAsync(secondFnAsync);

            Assert.AreEqual(21, result.Data);
        }

        [TestMethod]
        public async Task CatchAsyncShouldCatchOnError()
        {
            var result = await Result
                .Create(1)
                .MapAsync(_ => Task.FromException<int>(new InvalidOperationException()))
                .CatchAsync(ex => Result.Create(ex is InvalidOperationException ? 2 : 3))
                .ValidateAsync(value => value == 2, ResultComplete.OperationFailed, string.Empty);

            Assert.IsTrue(result.IsSuccessfulStatus());
            Assert.AreEqual(2, result.Data);
        }

        [TestMethod]
        public async Task CatchAsyncShouldBeSkippedWhenNoError()
        {
            var result = await Result
                .Create(1)
                .MapAsync(_ => Task.FromResult(5))
                .CatchAsync(ex => Result.Create(ex is InvalidOperationException ? 2 : 3));

            Assert.AreEqual(5, result.Data);
        }
    }
}

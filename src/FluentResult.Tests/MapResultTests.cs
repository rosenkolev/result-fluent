using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentResult.Tests
{
    [TestClass]
    public class MapResultTests
    {
        [TestMethod]
        public void MapMultiple()
        {
            var result = Result.Create(2)
                .Map(t => t * 2) // 4
                .Map(t => t + 2) // 6
                .Validate(t => t == 6, ResultComplete.OperationFailed, "Test")
                .Map(t => (t - 1).ToString());

            Assert.AreEqual("5", result.Data);
        }

        [TestMethod]
        public async Task MapAsync()
        {
            var result = await Result
                .Create(2)
                .MapAsync(t => Task.FromResult(t + 3)) // 5
                .MapAsync(t => t + 1);

            Assert.AreEqual(6, result.Data);
        }

        [TestMethod]
        public void MapList()
        {
            var models = new[] { 3, 5 };
            var result = Result
                .Create(models)
                .MapList(t => t * 2);

            Assert.AreEqual(6, result.Data[0]);
            Assert.AreEqual(10, result.Data[1]);
        }

        [TestMethod]
        public void MapListEnumerable()
        {
            var models = Enumerable.Range(1, 3);
            var result = Result
                .Create(models)
                .MapList(t => t * 2);

            Assert.AreEqual(2, result.Data[0]);
            Assert.AreEqual(4, result.Data[1]);
            Assert.AreEqual(6, result.Data[2]);
        }

        [TestMethod]
        public async Task MapAsyncAsync()
        {
            var result = await Result
                .Create(5)
                .MapAsync(t => Task.FromResult(t))
                .MapAsync(async t => await Task.Run(() => t * 3));

            Assert.AreEqual(15, result.Data);
        }

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

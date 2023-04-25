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
        public async Task MapAsyncShouldSkipInvalid()
        {
            var initialResult = new Result<int>(1, ResultComplete.InvalidArgument, new[] { "A" });
            var passThought = false;
            var result = await initialResult.MapAsync(t =>
            {
                passThought = true;
                return Task.FromResult("OK");
            });

            Assert.IsNull(result.Data);
            Assert.IsFalse(passThought);
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
    }
}

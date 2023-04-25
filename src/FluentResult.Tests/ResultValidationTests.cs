using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentResult.Tests
{
    [TestClass]
    public sealed class ResultValidationTests
    {
        [TestMethod]
        public void AsValidDataShouldThrowWhenNoSuccess()
        {
            Assert.ThrowsException<ResultValidationException>(
                () =>
                {
                    Result
                        .CreateResultWithError<string>(ResultComplete.Conflict, "test")
                        .AsValidData();
                });
        }

        [TestMethod]
        public void AsValidDataShouldThrowExceptionWithCorrectMessage()
        {
            var valid = false;
            try
            {
                Result
                    .CreateResultWithError<string>(ResultComplete.Conflict, "Test message")
                    .Validate(false, ResultComplete.InvalidArgument, "Second message")
                    .AsValidData();
            }
            catch (ResultValidationException ex)
            {
                valid = ex.Message.Equals("Validation failed with status InvalidArgument. Test message. Second message.");
            }

            Assert.IsTrue(valid);
        }

        [TestMethod]
        public void AsValidDataShouldReturnResult()
        {
            var data = new Result<string>("T", ResultComplete.Success, null).AsValidData();
            Assert.AreEqual("T", data);
        }

        [TestMethod]
        public async Task AsValidDataAsyncShouldThrowWhenNoSuccess()
        {
            await Assert.ThrowsExceptionAsync<ResultValidationException>(
                () => Helper.Async("A", ResultComplete.Conflict).AsValidDataAsync());
        }

        [TestMethod]
        public async Task AsValidDataAsyncShouldReturnResult()
        {
            var data = await Helper.Async("A").AsValidDataAsync();
            Assert.AreEqual("A", data);
        }

        [TestMethod]
        public void AsValidDataShouldReturnForResultOfItems()
        {
            var data = new int[] { 1 };
            var res = Result.CreateResultOfItems(data, 1).AsValidData();
            Assert.AreSame(data, res);
        }

        [TestMethod]
        public async Task AsValidDataShouldReturnForResultOfItemsAsync()
        {
            IReadOnlyCollection<int> data = new[] { 1 };
            var res = await Result.Create(data)
                .MapAsync(Task.FromResult)
                .ToResultOfItemsAsync()
                .AsValidDataAsync();

            Assert.AreSame(data, res);
        }

        [TestMethod]
        public async Task TestAsResAsync()
        {
            var val = await Result.Create(4).MapAsync(Task.FromResult).ToAsync();
            Assert.AreEqual(4, val.Data);
        }
    }
}

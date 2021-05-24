using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentResult.Tests
{
    [TestClass]
    public class ValidateResultTests
    {
        [TestMethod]
        public void ValidateSingleSuccess()
        {
            var res = Result.Validate(true, ResultComplete.InvalidArgument, null);

            Assert.IsTrue(res.IsSuccessfulStatus());
            Assert.IsTrue(res.Data);
        }

        [TestMethod]
        public void ValidateSingleFail()
        {
            var res = Result.Validate(false, ResultComplete.InvalidArgument, "Invalid");

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.IsFalse(res.Data);
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.IsTrue(res.Messages.Contains("Invalid"));
        }

        [TestMethod]
        public void ValidateMultiSuccess()
        {
            var res = Result
                .Validate(true, ResultComplete.InvalidArgument, null)
                .Validate(1 == 1, ResultComplete.InvalidArgument, null)
                .Validate(valid => 2 == 2, ResultComplete.InvalidArgument, null);

            Assert.IsTrue(res.IsSuccessfulStatus());
            Assert.IsTrue(res.Data);
        }

        [TestMethod]
        public void ValidateMultiFailed()
        {
            var res = Result
                .Validate(true, ResultComplete.InvalidArgument, "1")
                .Validate(2 == 1, ResultComplete.InvalidArgument, "2")
                .Validate(valid => 3 == 2, ResultComplete.InvalidArgument, "3");

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.IsFalse(res.Data);
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(2, res.Messages.Count);
            Assert.IsTrue(res.Messages.Contains("2"));
            Assert.IsTrue(res.Messages.Contains("3"));
        }

        [TestMethod]
        public async Task ValidateAsyncFailed()
        {
            var model = TestModel.Generate();
            var res = await Result
                .Create(model)
                .MapAsync(m => Task.FromResult(m))
                .ValidateAsync(model => model.Id < 1, ResultComplete.InvalidArgument, "Id should be smaller than 1");

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.IsNull(res.Data);
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(1, res.Messages.Count);
            Assert.IsTrue(res.Messages.Contains("Id should be smaller than 1"));
        }

        [TestMethod]
        public async Task ValidateAsyncSuccess()
        {
            var model = TestModel.Generate();
            var res = await Result
                .Create(model)
                .MapAsync(m => Task.FromResult(m))
                .ValidateAsync(model => model.Id > 1, ResultComplete.InvalidArgument, "Id should be bigger than 1");

            Assert.IsTrue(res.IsSuccessfulStatus());
            Assert.IsNull(res.Messages);
            Assert.AreEqual(model, res.Data);
        }

        [TestMethod]
        public async Task ValidateAsyncFuncSuccess()
        {
            var res = await Result
                .Create(2)
                .MapAsync(m => Task.FromResult(m))
                .ValidateAsync(async model => await Task.FromResult(2) == model, ResultComplete.OperationFailed, "Test");

            Assert.IsTrue(res.IsSuccessfulStatus());
            Assert.IsNull(res.Messages);
        }
    }
}

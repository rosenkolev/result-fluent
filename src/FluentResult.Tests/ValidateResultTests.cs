using System.Linq;
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
                .Validate(valid => 2 == 2, ResultComplete.InvalidArgument, default(string));

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
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(2, res.Messages.Count);
            Assert.IsTrue(res.Messages.Contains("2"));
            Assert.IsTrue(res.Messages.Contains("3"));
        }

        [TestMethod]
        public void ValidateMessageDelegateSuccess()
        {
            var res = Result
                .Create(new { Status = 0, Value = 1 })
                .Validate(
                    x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with status: {x.Status}");

            Assert.IsTrue(res.IsSuccessfulStatus());
            Assert.AreEqual(0, res.Data.Status);
            Assert.AreEqual(1, res.Data.Value);
        }

        [TestMethod]
        public void ValidateMessageDelegateFail()
        {
            var res = Result
                .Create(new { Status = 1, Value = 1 })
                .Validate(
                    x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with status: {x.Status}");

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(1, res.Messages.Count);
            Assert.AreEqual(res.Messages.First(), "Failed with status: 1");
        }

        [TestMethod]
        public void ValidateMessageDelegateMultiFail()
        {
            var res = Result
                .Create(new { Status = 1, Value = 1 })
                .Validate(
                    x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with status: {x.Status}")
                .Validate(
                    x => x.Value == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with value: {x.Value}");

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(2, res.Messages.Count);
            Assert.AreEqual(res.Messages.First(), "Failed with status: 1");
            Assert.AreEqual(res.Messages.Skip(1).First(), "Failed with value: 1");
        }

        [TestMethod]
        public void ValidateMessageDelegateMultiFailWithSkip()
        {
            var res = Result
                .Create(new { Status = 1, Value = 1 })
                .Validate(
                    x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with status: {x.Status}")
                .Validate(
                    x => x.Value == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with value: {x.Value}",
                    skipOnInvalidResult: true);

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(1, res.Messages.Count);
            Assert.AreEqual(res.Messages.First(), "Failed with status: 1");
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
            Assert.AreEqual(model, res.Data);
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

        [TestMethod]
        public void ValidateWithSkipOnFailWorks()
        {
            var res = Result
                .Validate(false, ResultComplete.InvalidArgument, "V1")
                .Validate(0 == 1, ResultComplete.InvalidArgument, "V2", skipOnInvalidResult: true)
                .Validate(_ => 1 == 2, ResultComplete.InvalidArgument, "V3", skipOnInvalidResult: true)
                .Validate(_ => 1 == 2, ResultComplete.InvalidArgument, "V4", skipOnInvalidResult: false);

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.AreEqual(2, res.Messages.Count);
            Assert.IsTrue(res.Messages.Contains("V1"), "V1 apply");
            Assert.IsTrue(res.Messages.Contains("V4"), "V4 apply");
        }

        [TestMethod]
        public void Validate_PassOnDataWhenFail()
        {
            var res = Result.Create(3)
                .Validate(_ => false, ResultComplete.InvalidArgument, "V1", skipOnInvalidResult: false);
            Assert.AreEqual(3, res.Data);
        }

        [TestMethod]
        public async Task ValidateWithSkipOnFailAsyncWorks()
        {
            var res = await Result.Create(2)
                .MapAsync(m => Task.FromResult(m))
                .ValidateAsync(m => m == 1, ResultComplete.InvalidArgument, "V1", skipOnInvalidResult: true)
                .ValidateAsync(m => m == 1, ResultComplete.InvalidArgument, "V2", skipOnInvalidResult: true)
                .ValidateAsync(m => m == 1, ResultComplete.InvalidArgument, "V3", skipOnInvalidResult: false);

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.AreEqual(2, res.Messages.Count);
            Assert.IsTrue(res.Messages.Contains("V1"), "V1 apply");
            Assert.IsTrue(res.Messages.Contains("V3"), "V3 apply");
        }

        [TestMethod]
        public async Task ValidateAsyncPredicate_Success()
        {
            var res = await Result.Create(2)
                .ValidateAsync(arg => Task.Run(() => true), ResultComplete.Conflict, "ERR");

            Assert.AreEqual(2, res.Data);
            Assert.IsTrue(res.IsSuccessfulStatus());
        }

        [TestMethod]
        public async Task ValidateAsyncPredicate_Fail()
        {
            var res = await Result.Create(2)
                .ValidateAsync(arg => Task.Run(() => false), ResultComplete.Conflict, "ERR");

            Assert.AreEqual(2, res.Data);
            Assert.AreEqual(ResultComplete.Conflict, res.Status);
            Assert.AreEqual("ERR", res.Messages.First());
        }

        [TestMethod]
        public async Task ValidateAsyncPredicate_SkipEval()
        {
            var evaluated = false;
            var res = await Result
                .Validate(false, ResultComplete.InvalidArgument, "A")
                .ValidateAsync(
                    _ =>
                    {
                        evaluated = true;
                        return Task.FromResult(true);
                    },
                    ResultComplete.Conflict,
                    "B",
                    skipOnInvalidResult: true);

            Assert.IsFalse(evaluated);
            Assert.AreEqual(1, res.Messages.Count);
        }

        [TestMethod]
        public async Task ValidateAsyncPredicate_Eval()
        {
            var evaluated = false;
            var res = await Result
                .Validate(false, ResultComplete.InvalidArgument, "A")
                .ValidateAsync(
                    _ =>
                    {
                        evaluated = true;
                        return Task.FromResult(false);
                    },
                    ResultComplete.Conflict,
                    "B",
                    skipOnInvalidResult: false);

            Assert.IsTrue(evaluated);
            Assert.AreEqual(2, res.Messages.Count);
            Assert.AreEqual("A", res.Messages.First());
            Assert.AreEqual("B", res.Messages.Last());
        }

        [TestMethod]
        public async Task ValidateAsyncMessageDelegateSuccess()
        {
            var res = await Result
                .Create(1)
                .MapAsync(x => Task.FromResult(new { Status = 0, Value = x }))
                .ValidateAsync(
                    x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with status: {x.Status}");

            Assert.IsTrue(res.IsSuccessfulStatus());
            Assert.AreEqual(0, res.Data.Status);
            Assert.AreEqual(1, res.Data.Value);
        }

        [TestMethod]
        public async Task ValidateAsyncMessageDelegateFail()
        {
            var res = await Result
               .Create(1)
               .MapAsync(x => Task.FromResult(new { Status = x, Value = x }))
               .ValidateAsync(
                    x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with status: {x.Status}");

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(1, res.Messages.Count);
            Assert.AreEqual(res.Messages.First(), "Failed with status: 1");
        }

        [TestMethod]
        public async Task ValidateAsyncMessageDelegateMultiFail()
        {
            var res = await Result
                .Create(1)
                .MapAsync(x => Task.FromResult(new { Status = x, Value = x }))
                .ValidateAsync(
                    x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with status: {x.Status}")
                .ValidateAsync(
                    x => Task.FromResult(x.Value == 0),
                    ResultComplete.InvalidArgument,
                    x => $"Failed with value: {x.Value}");

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(2, res.Messages.Count);
            Assert.AreEqual(res.Messages.First(), "Failed with status: 1");
            Assert.AreEqual(res.Messages.Skip(1).First(), "Failed with value: 1");
        }

        [TestMethod]
        public async Task ValidateAsyncMessageDelegateMultiFailWithSkip()
        {
            var res = await Result
                .Create(1)
                .MapAsync(x => Task.FromResult(new { Status = x, Value = x }))
                .ValidateAsync(
                   x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Failed with status: {x.Status}")
                .ValidateAsync(
                   x => x.Status == 0,
                    ResultComplete.InvalidArgument,
                    x => $"Skip Failed with status: {x.Status}",
                    skipOnInvalidResult: true)
                .ValidateAsync(
                    x => Task.FromResult(x.Value == 0),
                    ResultComplete.InvalidArgument,
                    x => $"Failed with value: {x.Value}")
                .ValidateAsync(
                    x => Task.FromResult(x.Value == 0),
                    ResultComplete.InvalidArgument,
                    x => $"Skip Failed with value: {x.Value}",
                    skipOnInvalidResult: true);

            Assert.IsFalse(res.IsSuccessfulStatus());
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(2, res.Messages.Count);
            Assert.AreEqual(res.Messages.First(), "Failed with status: 1");
            Assert.AreEqual(res.Messages.Skip(1).First(), "Failed with value: 1");
        }

        [TestMethod]
        public void ValidateNotNull_ShouldSkip()
        {
            var res = Result.Create<string>(null)
                .Validate(false, ResultComplete.InvalidArgument, "A")
                .ValidateNotNull(
                    ResultComplete.Conflict,
                    "B",
                    skipOnInvalidResult: true);

            Assert.AreEqual(1, res.Messages.Count);
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(null, res.Data);
        }

        [TestMethod]
        public void ValidateNotNull_ShouldCheck()
        {
            var res = Result.Create<string>(null)
                .Validate(false, ResultComplete.InvalidArgument, "A")
                .ValidateNotNull(ResultComplete.Conflict, "B", skipOnInvalidResult: false);

            Assert.AreEqual(2, res.Messages.Count);
            Assert.AreEqual(ResultComplete.Conflict, res.Status);
        }

        [TestMethod]
        public void ValidateNotNull_ShouldPass()
        {
            var res = Result.Create("TEST").ValidateNotNull(ResultComplete.Conflict, "B");

            Assert.AreEqual(null, res.Messages);
            Assert.AreEqual(ResultComplete.Success, res.Status);
            Assert.AreEqual("TEST", res.Data);
        }

        [TestMethod]
        public async Task ValidateNotNullAsync_ShouldSkip()
        {
            var res = await Result.Create<string>(null)
                .ValidateAsync(arg => Task.Run(() => false), ResultComplete.InvalidArgument, "A")
                .ValidateNotNullAsync(
                    ResultComplete.Conflict,
                    "B",
                    skipOnInvalidResult: true);

            Assert.AreEqual(1, res.Messages.Count);
            Assert.AreEqual(ResultComplete.InvalidArgument, res.Status);
            Assert.AreEqual(null, res.Data);
        }

        [TestMethod]
        public async Task ValidateNotNullAsync_ShouldCheck()
        {
            var res = await Result.Create<string>(null)
                .ValidateAsync(arg => Task.Run(() => false), ResultComplete.InvalidArgument, "A")
                .ValidateNotNullAsync(ResultComplete.Conflict, "B", skipOnInvalidResult: false);

            Assert.AreEqual(2, res.Messages.Count);
            Assert.AreEqual(ResultComplete.Conflict, res.Status);
        }

        [TestMethod]
        public async Task ValidateNotNullAsync_ShouldPass()
        {
            var res = await Result.Create("TEST")
                .ValidateAsync(arg => Task.Run(() => true), ResultComplete.InvalidArgument, "A")
                .ValidateNotNullAsync(ResultComplete.Conflict, "B");

            Assert.AreEqual(null, res.Messages);
            Assert.AreEqual(ResultComplete.Success, res.Status);
            Assert.AreEqual("TEST", res.Data);
        }
    }
}

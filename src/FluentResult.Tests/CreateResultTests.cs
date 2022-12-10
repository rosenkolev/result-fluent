using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentResult.Tests
{
    [TestClass]
    public class CreateResultTests
    {
        [TestMethod]
        public void CreateConstructor()
        {
            var model = TestModel.Generate();
            var res = new Result<TestModel>(model, ResultComplete.OperationFailed, new [] { "MyMessage" });
            Assert.AreEqual(model, res.Data);
            Assert.AreEqual(ResultComplete.OperationFailed, res.Status);
            Assert.AreEqual(1, res.Messages.Count);
            Assert.IsTrue(res.Messages.Contains("MyMessage"));
        }

        [TestMethod]
        public void CreateConstructorNulls()
        {
            var res = new Result<TestModel>(null, 0, null);
            Assert.IsNull(res.Data);
            Assert.IsNull(res.Messages);
            Assert.AreEqual(ResultComplete.Success, res.Status);
        }

        [TestMethod]
        public void CreateConstructorSuccess()
        {
            var model = TestModel.Generate();
            var res = new Result<TestModel>(model);
            Assert.AreEqual(model, res.Data);
            Assert.IsNull(res.Messages);
            Assert.IsTrue(res.IsSuccessfulStatus());
        }

        [TestMethod]
        public void CreateStaticSuccess()
        {
            var model = TestModel.Generate();
            var res = Result.Create(model);
            Assert.AreEqual(model, res.Data);
            Assert.IsNull(res.Messages);
            Assert.IsTrue(res.IsSuccessfulStatus());
        }

        [TestMethod]
        public void CreateStaticFail()
        {
            var res = Result.CreateResultWithError<TestModel>(ResultComplete.NotFound, "Model not found");
            Assert.IsNull(res.Data);
            Assert.AreEqual(ResultComplete.NotFound, res.Status);
            Assert.IsTrue(res.Messages.Contains("Model not found"));
        }

        [TestMethod]
        public void ConvertResultToOtherData()
        {
            var result = new Result<int>(1, ResultComplete.OperationFailed, new [] { "ERR" });
            var other = result.To("R");
            Assert.AreEqual("R", other.Data);
            Assert.AreEqual(ResultComplete.OperationFailed, other.Status);
            Assert.AreEqual("ERR", other.Messages.First());
        }

        [TestMethod]
        public void ConvertWithMessage()
        {
            var result = Result.Create(2, "TEST");
            Assert.AreEqual(2, result.Data);
            Assert.AreEqual(ResultComplete.Success, result.Status);
            Assert.AreEqual("TEST", result.Messages.First());
        }
    }
}

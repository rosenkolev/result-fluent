using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentResult.Tests
{
    [TestClass]
    public class ResultOfItemsTests
    {
        [TestMethod]
        public void CreateConstructor()
        {
            var models = new[] { TestModel.Generate(), TestModel.Generate() };
            var res = new ResultOfItems<TestModel>(models, ResultComplete.Success, null, 3, 2, 1, null);
            Assert.AreEqual(models, res.Data);
            Assert.AreEqual(ResultComplete.Success, res.Status);
            Assert.IsNull(res.Messages);
            Assert.AreEqual(2, res.Metadata.Count);
            Assert.AreEqual(3, res.Metadata.Total);
            Assert.AreEqual(2, res.Metadata.PageSize);
            Assert.AreEqual(1, res.Metadata.PageIndex);
        }

        [TestMethod]
        public async Task MapItemsAsync()
        {
            var models = new[] { TestModel.Generate(), TestModel.Generate() };
            var result = await Result.Create(models)
                .MapAsync(it => Task.FromResult((IReadOnlyList<TestModel>)it))
                .ToResultOfItemsAsync(it => Task.FromResult(
                    Result.CreateResultOfItems(it, 10)));

            Assert.AreEqual(2, result.Metadata.Count);
            Assert.AreEqual(10, result.Metadata.Total);
        }
    }
}

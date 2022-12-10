using System.Collections.Generic;
using System.Linq;
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
        public void CreateConstructorWithoutMetadata()
        {
            var models = new[] { TestModel.Generate(), TestModel.Generate() };
            var res = new ResultOfItems<TestModel>(models, ResultComplete.Success, null);
            Assert.AreEqual(models, res.Data);
            Assert.AreEqual(ResultComplete.Success, res.Status);
            Assert.IsNull(res.Messages);
            Assert.IsNull(res.Metadata);
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


        [TestMethod]
        public void ToResultOfItems_Create()
        {
            var model = TestModel.Generate();
            var result = Result.Create(new[] { model })
                .ToResultOfItems(data => Result.CreateResultOfItems(data, 1));

            Assert.AreEqual(model, result.Data.First());
        }

        [TestMethod]
        public async Task ToResultOfItemsAsync_Create()
        {
            var model = TestModel.Generate();
            var result = await Helper.Async(new[] { model })
                .ToResultOfItemsAsync(data => Result.CreateResultOfItems(data, 1));

            Assert.AreEqual(model, result.Data.First());
        }

        [TestMethod]
        public async Task ToResultOfItemsAsync_CreateAsync()
        {
            var model = TestModel.Generate();
            var result = await Helper.Async(new[] { model })
                .ToResultOfItemsAsync(data =>
                    Task.Run(() => Result.CreateResultOfItems(data, 1)));

            Assert.AreEqual(model, result.Data.First());
        }

        [TestMethod]
        public async Task ToResultOfItems_CreateAsync()
        {
            var model = TestModel.Generate();
            var result = await Result.Create(new[] { model })
                .ToResultOfItemsAsync(data =>
                    Task.Run(() => Result.CreateResultOfItems(data, 1)));

            Assert.AreEqual(model, result.Data.First());
        }

        [TestMethod]
        public async Task ToResultOfItemsAsync_CreateNoMapCollection()
        {
            var model = TestModel.Generate();
            var result = await Helper.Async<IReadOnlyCollection<TestModel>>(new[] { model })
                .ToResultOfItemsAsync();

            Assert.AreEqual(model, result.Data.First());
        }

        [TestMethod]
        public async Task ToResultOfItemsAsync_CreateNoMapList()
        {
            var model = TestModel.Generate();
            var result = await Helper.Async<IReadOnlyList<TestModel>>(new[] { model })
                .ToResultOfItemsAsync();

            Assert.AreEqual(model, result.Data.First());
        }
    }
}

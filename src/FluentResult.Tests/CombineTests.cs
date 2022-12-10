using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FluentResult.Tests
{
    [TestClass]
    public sealed class CombineTests
    {
        [TestMethod]
        public void Combine3()
        {
            var result = Result.Create(2)
                .Combine(_ => (
                    Result.Create(true),
                    Result.Create("3")),
                   (a, b, c) => $"{a}_{b}_{c}");

            Assert.AreEqual("2_True_3", result.Data);
        }

        [TestMethod]
        public void Combine4()
        {
            var result = Result.Create(2)
                .Combine(_ => (
                    Result.Create(true),
                    Result.Create("3"),
                    Result.Create("4")),
                   (a, b, c, d) => $"{a}_{b}_{c}_{d}");

            Assert.AreEqual("2_True_3_4", result.Data);
        }

        [TestMethod]
        public void Combine5()
        {
            var result = Result.Create(2)
                .Combine(_ => (
                    Result.Create(true),
                    Result.Create("3"),
                    Result.Create("4"),
                    Result.Create("5")),
                   (a, b, c, d, e) => $"{a}_{b}_{c}_{d}_{e}");

            Assert.AreEqual("2_True_3_4_5", result.Data);
        }

        [TestMethod]
        public void Combine6()
        {
            var result = Result.Create(2)
                .Combine(_ => (
                    Result.Create(true),
                    Result.Create("3"),
                    Result.Create("4"),
                    Result.Create("5"),
                    Result.Create("6")),
                   (a, b, c, d, e, f) => $"{a}_{b}_{c}_{d}_{e}_{f}");

            Assert.AreEqual("2_True_3_4_5_6", result.Data);
        }

        [TestMethod]
        public async Task Combine3Async()
        {
            var result = await Helper.Async(1).CombineAsync(
                _ => (
                    Helper.Async("2"),
                    Helper.Async("3")),
                (a, b, c) => $"{a}_{b}_{c}");

            Assert.AreEqual("1_2_3", result.Data);
        }

        [TestMethod]
        public async Task Combine4Async()
        {
            var result = await Helper.Async(1).CombineAsync(
                _ => (
                    Helper.Async("2"),
                    Helper.Async("3"),
                    Helper.Async("4")),
                (a, b, c, d) => $"{a}_{b}_{c}_{d}");

            Assert.AreEqual("1_2_3_4", result.Data);
        }

        [TestMethod]
        public async Task Combine5Async()
        {
            var result = await Helper.Async(1).CombineAsync(
                _ => (
                    Helper.Async("2"),
                    Helper.Async("3"),
                    Helper.Async("4"),
                    Helper.Async("5")),
                (a, b, c, d, e) => $"{a}_{b}_{c}_{d}_{e}");

            Assert.AreEqual("1_2_3_4_5", result.Data);
        }

        [TestMethod]
        public async Task Combine6Async()
        {
            var result = await Helper.Async(1).CombineAsync(
                _ => (
                    Helper.Async("2"),
                    Helper.Async("3"),
                    Helper.Async("4"),
                    Helper.Async("5"),
                    Helper.Async("6")),
                (a, b, c, d, e, f) => $"{a}_{b}_{c}_{d}_{e}_{f}");

            Assert.AreEqual("1_2_3_4_5_6", result.Data);
        }
    }
}

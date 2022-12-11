using System.Linq;
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

        [TestMethod]
        public void Combine3_FailWhenFirstFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "A"),
                Result.Create("3")),
                (a, b, c) => $"{a}_{b}_{c}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("A", result.Messages.First());
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void Combine3_FailWhenSecondFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("3"),
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "B")),
                (a, b, c) => $"{a}_{b}_{c}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("B", result.Messages.First());
            Assert.IsNull(result.Data);
        }

        [TestMethod]
        public void Combine4_FailWhenFirstFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "A"),
                Result.Create("3"),
                Result.Create("4")),
                (a, b, c, d) => $"{a}_{b}_{c}_{d}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("A", result.Messages.First());
        }

        [TestMethod]
        public void Combine4_FailWhenSecondFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.CreateResultWithError<int>(ResultComplete.NotFound, "B"),
                Result.Create("4")),
                (a, b, c, d) => $"{a}_{b}_{c}_{d}");

            Assert.AreEqual(ResultComplete.NotFound, result.Status);
            Assert.AreEqual("B", result.Messages.First());
        }

        [TestMethod]
        public void Combine4_FailWhenThirdFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.Create("3"),
                Result.CreateResultWithError<int>(ResultComplete.InvalidArgument, "C")),
                (a, b, c, d) => $"{a}_{b}_{c}_{d}");

            Assert.AreEqual(ResultComplete.InvalidArgument, result.Status);
            Assert.AreEqual("C", result.Messages.First());
        }

        [TestMethod]
        public void Combine5_FailWhenFirstFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.CreateResultWithError<int>(ResultComplete.OperationFailed, "A"),
                Result.Create("3"),
                Result.Create("4"),
                Result.Create("5")),
                (a, b, c, d, e) => $"{a}_{b}_{c}_{d}_{e}");

            Assert.AreEqual(ResultComplete.OperationFailed, result.Status);
            Assert.AreEqual("A", result.Messages.First());
        }

        [TestMethod]
        public void Combine5_FailWhenSecondFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "B"),
                Result.Create("4"),
                Result.Create("5")),
                (a, b, c, d, e) => $"{a}_{b}_{c}_{d}_{e}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("B", result.Messages.First());
        }

        [TestMethod]
        public void Combine5_FailWhenThirdFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.Create("3"),
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "C"),
                Result.Create("5")),
                (a, b, c, d, e) => $"{a}_{b}_{c}_{d}_{e}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("C", result.Messages.First());
        }

        [TestMethod]
        public void Combine5_FailWhenForthFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.Create("3"),
                Result.Create("4"),
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "D")),
                (a, b, c, d, e) => $"{a}_{b}_{c}_{d}_{e}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("D", result.Messages.First());
        }

        [TestMethod]
        public void Combine6_FailWhenFirstFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "A"),
                Result.Create("3"),
                Result.Create("4"),
                Result.Create("5"),
                Result.Create("6")),
                (a, b, c, d, e, f) => $"{a}_{b}_{c}_{d}_{e}_{f}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("A", result.Messages.First());
        }

        [TestMethod]
        public void Combine6_FailWhenSecondFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "B"),
                Result.Create("4"),
                Result.Create("5"),
                Result.Create("6")),
                (a, b, c, d, e, f) => $"{a}_{b}_{c}_{d}_{e}_{f}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("B", result.Messages.First());
        }

        [TestMethod]
        public void Combine6_FailWhenThirdFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.Create("3"),
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "C"),
                Result.Create("5"),
                Result.Create("6")),
                (a, b, c, d, e, f) => $"{a}_{b}_{c}_{d}_{e}_{f}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("C", result.Messages.First());
        }

        [TestMethod]
        public void Combine6_FailWhenFourthFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.Create("3"),
                Result.Create("4"),
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "D"),
                Result.Create("6")),
                (a, b, c, d, e, f) => $"{a}_{b}_{c}_{d}_{e}_{f}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("D", result.Messages.First());
        }

        [TestMethod]
        public void Combine6_FailWhenFifthFail()
        {
            var result = Result.Create(2).Combine(_ => (
                Result.Create("2"),
                Result.Create("3"),
                Result.Create("4"),
                Result.Create("6"),
                Result.CreateResultWithError<int>(ResultComplete.Conflict, "E")),
                (a, b, c, d, e, f) => $"{a}_{b}_{c}_{d}_{e}_{f}");

            Assert.AreEqual(ResultComplete.Conflict, result.Status);
            Assert.AreEqual("E", result.Messages.First());
        }
    }
}

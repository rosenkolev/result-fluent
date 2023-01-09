using System.Threading.Tasks;

namespace FluentResult.Tests
{
    public static class Helper
    {
        public static Task<Result<T>> Async<T>(T data) =>
            Task.Run(() => Result.Create(data));

        public static Task<Result<T>> Async<T>(T data, ResultComplete status, params string[] messages) =>
            Task.Run(() => new Result<T>(data, status, messages));
    }
}

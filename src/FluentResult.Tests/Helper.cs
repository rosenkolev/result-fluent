using System.Threading.Tasks;

namespace FluentResult.Tests
{
    public static class Helper
    {
        public static Task<Result<T>> Async<T>(T data) =>
            Task.Run(() => Result.Create(data));
    }
}

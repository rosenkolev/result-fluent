using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FluentResult;

/// <summary>
/// A short version of Task&lt;Result&lt;TResult&gt;&gt;.
/// Short for AsynchronousResult.
/// </summary>
/// <typeparam name="TResult">The type of the result.</typeparam>
public struct Async<TResult>
{
    /// <summary>Gets the configured awaiter.</summary>
    public ConfiguredTaskAwaitable<Result<TResult>>.ConfiguredTaskAwaiter GetAwaiter() =>
        Awaitable.GetAwaiter();

    /// <summary>Initializes a new instance of the <see cref="Async{TResult}"/> struct.</summary>
    public Async(Task<Result<TResult>> result)
    {
        Awaitable = result.ConfigureAwait(false);
    }

    /// <summary>Gets the inner task.</summary>
    public ConfiguredTaskAwaitable<Result<TResult>> Awaitable { get; private set; }
}

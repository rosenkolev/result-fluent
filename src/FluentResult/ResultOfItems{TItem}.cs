using System.Collections.Generic;
using System.Linq;

namespace FluentResult
{
    /// <summary>The result as collection of items.</summary>
    /// <typeparam name="TItem">The type of the query/collection item.</typeparam>
    public class ResultOfItems<TItem> : Result<IEnumerable<TItem>>
    {
        /// <summary>Initializes a new instance of the <see cref="ResultOfItems{TItem}"/> class.</summary>
        public ResultOfItems(
            IEnumerable<TItem> items,
            ResultComplete status,
            ICollection<string>? messages,
            int? totalCount,
            int? pageSize,
            int? pageIndex,
            int? count)
            : base(items, status, messages)
        {
            Metadata = new ResultMetadata
            {
                Count = count ?? items.Count(),
                Total = totalCount,
                PageSize = pageSize,
                PageIndex = pageIndex,
            };
        }

        /// <summary>Initializes a new instance of the <see cref="ResultOfItems{TItem}"/> class.</summary>
        public ResultOfItems(IEnumerable<TItem> data, ResultComplete status, ICollection<string>? messages)
            : base(data, status, messages)
        {
        }

        /// <summary>Gets the metadata.</summary>
        public ResultMetadata? Metadata { get; private set; }
    }
}

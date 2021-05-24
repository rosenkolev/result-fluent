namespace FluentResult
{
    /// <summary>A result metadata.</summary>
    public class ResultMetadata
    {
        /// <summary>Gets or sets the count.</summary>
        public int Count { get; set; }

        /// <summary>Gets or sets the total records.</summary>
        public int? Total { get; set; }

        /// <summary>Gets or sets the index of the page.</summary>
        public int? PageIndex { get; set; }

        /// <summary>Gets or sets the size of the page.</summary>
        public int? PageSize { get; set; }
    }
}

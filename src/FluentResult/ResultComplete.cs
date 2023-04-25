namespace FluentResult
{
    /// <summary>The operation status types.</summary>
    public enum ResultComplete : byte
    {
        /// <summary>The success status</summary>
        Success,

        /// <summary>One or more object where no found</summary>
        NotFound,

        /// <summary>When one or more argument was invalid.</summary>
        InvalidArgument,

        /// <summary>When the operation failed.</summary>
        OperationFailed,

        /// <summary>When the operation is in conflict with another running operation.</summary>
        Conflict,
    }
}

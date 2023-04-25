using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace FluentResult;

[Serializable]
public class ResultValidationException : Exception
{
    private static readonly Type ValidationErrorsType = typeof(IReadOnlyCollection<string>);
    private static readonly Type ResultCompleteType = typeof(ResultComplete);

    /// <summary>Initializes a new instance of the <see cref="ResultValidationException"/> class.</summary>
    public ResultValidationException(
        ResultComplete status,
        IReadOnlyCollection<string> validationErrors)
        : base(
            string.Concat(
                $"Validation failed with status {Enum.GetName(ResultCompleteType, status)}. ",
                string.Join(". ", validationErrors),
                '.'))
    {
        Status = status;
        ValidationErrors = validationErrors;
    }

    /// <summary>Initializes a new instance of the <see cref="ResultValidationException"/> class.</summary>
    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    protected ResultValidationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        Status = (ResultComplete)info.GetByte("status");
        ValidationErrors = (IReadOnlyCollection<string>)info.GetValue("errors", ValidationErrorsType);
    }

    public ResultComplete Status { get; private set; }

    public IReadOnlyCollection<string> ValidationErrors { get; private set; }

    [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info?.AddValue("status", Status);
        info?.AddValue("errors", ValidationErrors, ValidationErrorsType);
        base.GetObjectData(info, context);
    }
}

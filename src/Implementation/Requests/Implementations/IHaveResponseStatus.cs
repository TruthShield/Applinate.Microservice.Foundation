// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    /// <summary>
    /// This is a strutural constraint for <see cref="IReturn{TResponse}"/> results.
    ///
    /// There must ALWAYS be a status for both commands and queries.
    /// </summary>
    public interface IHaveResponseStatus
    {
        ResponseStatus Status { get; }
    }
}
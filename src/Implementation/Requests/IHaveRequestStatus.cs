// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    /// <summary>
    /// This is a strutural constraint for <see cref="IReturn{TResult}"/> results.
    ///
    /// There must ALWAYS be a status for commands or queries.
    /// </summary>
    public interface IHaveRequestStatus
    {
        RequestStatus Status { get; }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public interface ICommandHandler<TRequest> : IRequestHandler<TRequest, CommandResponse>
    where TRequest : class, IReturn<CommandResponse>
    { }
}
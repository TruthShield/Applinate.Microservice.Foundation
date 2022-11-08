// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public interface ICommandHandler<TArg> : IRequestHandler<TArg, CommandResponse>
    where TArg : class, IReturn<CommandResponse>
    { }
}
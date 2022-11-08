// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    public interface IHandleCommand<TArg> : IHandleRequest<TArg, CommandResponse>
    where TArg : class, IReturn<CommandResponse>
    { }
}
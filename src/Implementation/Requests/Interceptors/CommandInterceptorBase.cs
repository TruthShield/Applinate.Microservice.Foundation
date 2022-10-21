// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Threading;

    /// <summary>
    /// Class DispatchedCommandInterceptorBase.
    /// Implements the <see cref="TS.Utility.CommandInterceptorBase{TArg, TS.Utility.DispatchedCommandResponse}" />
    /// </summary>
    /// <typeparam name="TArg">The type of the t argument.</typeparam>
    /// <seealso cref="TS.Utility.CommandInterceptorBase{TArg, TS.Utility.DispatchedCommandResponse}" />
    public class CommandInterceptorBase<TArg> : RequestInterceptorBase<TArg, CommandResponse>
    where TArg : class, IReturn<CommandResponse>
    {
        public CommandInterceptorBase(ExecuteDelegate<TArg, CommandResponse> core) : base(core)
        {
        }

        public override Task<CommandResponse> ExecuteAsync(TArg arg, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(arg, cancellationToken);
        }

        protected override Task<CommandResponse> PostProcessAsync(CommandResponse result)
        {
            return base.PostProcessAsync(result);
        }

        protected override Task<TArg> PreProcessAsync(TArg arg)
        {
            return base.PreProcessAsync(arg);
        }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    using System.Threading;

    /// <summary>
    /// Class DispatchedCommandInterceptorBase.
    /// Implements the <see cref="TS.Utility.CommandInterceptorBase{TRequest, TS.Utility.DispatchedCommandResponse}" />
    /// </summary>
    /// <typeparam name="TRequest">The type of the t argument.</typeparam>
    /// <seealso cref="TS.Utility.CommandInterceptorBase{TRequest, TS.Utility.DispatchedCommandResponse}" />
    public class CommandInterceptorBase<TRequest> : RequestInterceptorBase<TRequest, CommandResponse>
    where TRequest : class, IReturn<CommandResponse>
    {
        public CommandInterceptorBase(ExecuteDelegate<TRequest, CommandResponse> core) : base(core)
        {
        }

        public override Task<CommandResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken)
        {
            return base.ExecuteAsync(request, cancellationToken);
        }

        protected override Task<CommandResponse> PostProcessAsync(CommandResponse response)
        {
            return base.PostProcessAsync(response);
        }

        protected override Task<TRequest> PreProcessAsync(TRequest request)
        {
            return base.PreProcessAsync(request);
        }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [BypassSafetyChecks]
    internal sealed class FaultGeneratingCommandExecutor<TArg, TResult> : IRequestHandler<TArg, TResult>
        where TArg : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
    {
        private readonly Func<Exception> _ExceptionBuilder;

        public FaultGeneratingCommandExecutor(Func<Exception> exceptionBuilder)
        {
            Assert.IsNotNull(exceptionBuilder, nameof(exceptionBuilder));

            _ExceptionBuilder = exceptionBuilder;
        }

        public Task<TResult> ExecuteAsync(TArg arg, CancellationToken cancellationToken = default)
        {
            throw _ExceptionBuilder();
        }
    }
}
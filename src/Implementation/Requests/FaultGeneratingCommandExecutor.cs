// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [BypassSafetyChecks]
    internal sealed class FaultGeneratingCommandExecutor<TRequest, TResult> : IRequestHandler<TRequest, TResult>
        where TRequest : class, IReturn<TResult>
        where TResult : class, IHaveRequestStatus
    {
        private readonly Func<Exception> _ExceptionBuilder;

        public FaultGeneratingCommandExecutor(Func<Exception> exceptionBuilder)
        {
            Assert.IsNotNull(exceptionBuilder, nameof(exceptionBuilder));

            _ExceptionBuilder = exceptionBuilder;
        }

        public Task<TResult> ExecuteAsync(TRequest arg, CancellationToken cancellationToken = default)
        {
            throw _ExceptionBuilder();
        }
    }
}
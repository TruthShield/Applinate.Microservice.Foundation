// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate
{
    [BypassSafetyChecks]
    internal sealed class FaultGeneratingRequestExecutor<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : class, IReturn<TResponse>
        where TResponse : class, IHaveResponseStatus
    {
        private readonly Func<Exception> _ExceptionBuilder;

        public FaultGeneratingRequestExecutor(Func<Exception> exceptionBuilder)
        {
            Assert.IsNotNull(exceptionBuilder, nameof(exceptionBuilder));

            _ExceptionBuilder = exceptionBuilder;
        }

        public Task<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            throw _ExceptionBuilder();
        }
    }
}
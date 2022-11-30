// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Commands.Interceptors
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    [Intercept(-1999999999)]
    internal class RequestContextUpdaterInterceptorFactory : InterceptorFactoryBase
    {
        public override async Task<TResponse> ExecuteAsync<TRequest, TResponse>(
            ExecuteDelegate<TRequest, TResponse> next, 
            TRequest request,
            CancellationToken cancellationToken) 
        {
            var currentServiceType = RequestContextProvider.Instance.ServiceType;
            var attribute          = typeof(TRequest).GetCustomAttribute<ServiceRequestAttribute>();
            var nextServiceType    = attribute?.ServiceType ?? ServiceType.None;
            int nextCallCount      = RequestContextProvider.Instance.RequestCallCount + 1;

            var entry = RequestContextProvider.Instance = RequestContextProvider.Instance with
            {                
                ServiceType = nextServiceType,
                RequestCallCount = nextCallCount
            };

            try
            {
                InfrastructureEventSink.For.ScopedContextChange().Fire(
                    RequestContextChange.Entry<TRequest, TResponse>(
                        RequestContextProvider.Instance.RequestCallCount));

                InfrastructureEventSink.For.AnyContextChange().Fire(
                    RequestContextChange.Entry<TRequest, TResponse>(
                        RequestContextProvider.Instance.RequestCallCount));

                var result = await base.ExecuteAsync(next, request, cancellationToken).ConfigureAwait(false);
                return result ?? throw ExceptionFactory.UnexpectedNull();
            }
            catch
            {
                throw;
            }
            finally
            {
                int exitCallCount = RequestContextProvider.Instance.RequestCallCount + 1;

                RequestContextProvider.Instance = RequestContextProvider.Instance with
                {
                    ServiceType = currentServiceType,
                    RequestCallCount = exitCallCount
                };

                InfrastructureEventSink.For.ScopedContextChange().Fire(
                    RequestContextChange.Exit<TRequest, TResponse>(
                        exitCallCount));

                InfrastructureEventSink.For.AnyContextChange().Fire(
                    RequestContextChange.Exit<TRequest, TResponse>(
                        exitCallCount));
            }
        }
    }
}
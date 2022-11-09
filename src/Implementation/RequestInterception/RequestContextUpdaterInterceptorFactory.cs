// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Commands.Interceptors
{
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    [Intercept(-1999999999)]
    internal class RequestContextUpdaterInterceptorFactory : InterceptorFactoryBase
    {
        public override async Task<TResult> ExecuteAsync<TArg, TResult>(
            ExecuteDelegate<TArg, TResult> next, 
            TArg arg,
            CancellationToken cancellationToken) 
        {
            var currentServiceType = RequestContext.Current.ServiceType;
            var attribute          = typeof(TArg).GetCustomAttribute<ServiceRequestAttribute>();
            var nextServiceType    = attribute?.CommandType ?? ServiceType.None;


            int nextCallCount = RequestContext.Current.RequestCallCount + 1;
            var entry = RequestContext.Current = RequestContext.Current with
            {
                
                ServiceType = nextServiceType,
                RequestCallCount = nextCallCount
            };

            try
            {
                InfrastructureEventSink.For.ScopedContextChange().Fire(
                    RequestContextChange.Entry<TArg, TResult>(
                        RequestContext.Current.RequestCallCount));

                InfrastructureEventSink.For.AnyContextChange().Fire(
                    RequestContextChange.Entry<TArg, TResult>(
                        RequestContext.Current.RequestCallCount));

                var result = await base.ExecuteAsync(next, arg, cancellationToken).ConfigureAwait(false);
                return result ?? throw ExceptionFactory.UnexpectedNull();
            }
            catch
            {
                throw;
            }
            finally
            {
                int exitCallCount = RequestContext.Current.RequestCallCount + 1;

                RequestContext.Current = RequestContext.Current with
                {
                    ServiceType = currentServiceType,
                    RequestCallCount = exitCallCount
                };

                InfrastructureEventSink.For.ScopedContextChange().Fire(
                    RequestContextChange.Exit<TArg, TResult>(
                        exitCallCount));


                InfrastructureEventSink.For.AnyContextChange().Fire(
                    RequestContextChange.Exit<TArg, TResult>(
                        exitCallCount));
            }

            
        }
    }


}
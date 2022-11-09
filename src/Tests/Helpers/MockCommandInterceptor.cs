namespace Applinate.Test
{
    using Applinate;
    using System.Threading;
    using System.Threading.Tasks;
    /// <summary>
    /// Class MockCommandInterceptor. This class cannot be inherited.
    /// Implements the <see cref="Applinate.InterceptorFactoryBase" />
    /// </summary>
    /// <seealso cref="Applinate.InterceptorFactoryBase" />
    [Intercept(-2100000000)]
    internal sealed class MockCommandInterceptor : InterceptorFactoryBase
    {
        public override Task<TResponse> ExecuteAsync<TRequest, TResponse>(ExecuteDelegate<TRequest, TResponse> next, TRequest request, CancellationToken cancellationToken)
        {
            if (MockRequest<TRequest, TResponse>.IsSet)
            {
                return MockRequest<TRequest, TResponse>.Execute(request, cancellationToken);
            }

            return next(request, cancellationToken);
        }
    }
}

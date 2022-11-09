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
        public override Task<TResult> ExecuteAsync<TRequest, TResult>(ExecuteDelegate<TRequest, TResult> next, TRequest arg, CancellationToken cancellationToken)
        {
            if (MockRequest<TRequest, TResult>.IsSet)
            {
                return MockRequest<TRequest, TResult>.Execute(arg, cancellationToken);
            }

            return next(arg, cancellationToken);
        }
    }
}

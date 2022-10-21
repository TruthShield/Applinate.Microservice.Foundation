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
        public override Task<TResult> ExecuteAsync<TArg, TResult>(ExecuteDelegate<TArg, TResult> next, TArg arg, CancellationToken cancellationToken)
        {
            if (MockRequest<TArg, TResult>.IsSet)
            {
                return MockRequest<TArg, TResult>.Execute(arg, cancellationToken);
            }

            return next(arg, cancellationToken);
        }
    }
}

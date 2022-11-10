// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_008_InterceptorFactory : InterceptorFactoryBase
    {
        public override Task<TResponse> ExecuteAsync<TRequest, TResponse>(
            ExecuteDelegate<TRequest, TResponse> next, 
            TRequest request, 
            CancellationToken cancellationToken)
        {
            var newArg = Update(request);
            return base.ExecuteAsync(next, newArg, cancellationToken);
        }

        private static My_008_UnitTestCommand Update(My_008_UnitTestCommand val)
        {
            return new My_008_UnitTestCommand
            {
                PostNumber = val.PostNumber + 1,
                PreNumber = val.PreNumber + 1
            };
        }

        private static TRequest Update<TRequest>(TRequest request)
        {
            if (request is My_008_UnitTestCommand z)
            {
                return Update(z).As<TRequest>();
            }

            return request;
        }
    }
}
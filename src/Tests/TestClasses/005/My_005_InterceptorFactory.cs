// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_005_InterceptorFactory : InterceptorFactoryBase
    {
        public override Task<TResult> ExecuteAsync<TRequest, TResult>(ExecuteDelegate<TRequest, TResult> next, TRequest arg, CancellationToken cancellationToken)
        {
            var newArg = Update(arg);
            return base.ExecuteAsync(next, newArg, cancellationToken);
        }

        private static My_005_UnitTestCommand Update(My_005_UnitTestCommand val)
        {
            return new My_005_UnitTestCommand
            {
                PostNumber = val.PostNumber + 1,
                PreNumber = val.PreNumber + 1
            };
        }

        private static TRequest Update<TRequest>(TRequest arg)
        {
            if (arg is My_005_UnitTestCommand z)
            {
                return Update(z).As<TRequest>();
            }

            return arg;
        }
    }
}
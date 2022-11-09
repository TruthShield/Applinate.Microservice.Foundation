// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_007_InterceptorFactory : InterceptorFactoryBase
    {
        public override Task<TResult> ExecuteAsync<TRequest, TResult>(ExecuteDelegate<TRequest, TResult> next, TRequest arg, CancellationToken cancellationToken)
        {
            var newArg = Update(arg);
            return base.ExecuteAsync(next, newArg, cancellationToken);
        }

        private static My_007_UnitTestCommand Update(My_007_UnitTestCommand val)
        {
            return new My_007_UnitTestCommand
            {
                PostNumber = val.PostNumber + 1,
                PreNumber = val.PreNumber + 1
            };
        }

        private static TRequest Update<TRequest>(TRequest arg)
        {
            if (arg is My_007_UnitTestCommand z)
            {
                return Update(z).As<TRequest>();
            }

            return arg;
        }
    }
}
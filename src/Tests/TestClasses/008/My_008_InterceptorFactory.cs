// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_008_InterceptorFactory : InterceptorFactoryBase
    {
        public override Task<TResult> ExecuteAsync<TArg, TResult>(ExecuteDelegate<TArg, TResult> next, TArg arg, CancellationToken cancellationToken)
        {
            var newArg = Update(arg);
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

        private static TArg Update<TArg>(TArg arg)
        {
            if (arg is My_008_UnitTestCommand z)
            {
                return Update(z).As<TArg>();
            }

            return arg;
        }
    }
}
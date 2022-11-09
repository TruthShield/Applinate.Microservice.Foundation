// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [Intercept(0)]
    public class My_004A_Interceptor : RequestInterceptorBase<My_004_UnitTestCommand, My_004_UnitTestCommandResult>
    {
        public My_004A_Interceptor(ExecuteDelegate<My_004_UnitTestCommand, My_004_UnitTestCommandResult> core) : base(core)
        {
        }

        protected override Task<My_004_UnitTestCommand> PreProcessAsync(My_004_UnitTestCommand arg)
        {
            var newArg = new My_004_UnitTestCommand
            {
                PreNumber = arg.PreNumber + 1,
                PostNumber = arg.PostNumber + 1
            };

            return base.PreProcessAsync(newArg);
        }
    }
}
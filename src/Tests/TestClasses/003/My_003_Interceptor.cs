// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_003_Interceptor : RequestInterceptorBase<My_003_UnitTestCommand, My_003_UnitTestCommandResult>
    {
        public My_003_Interceptor(ExecuteDelegate<My_003_UnitTestCommand, My_003_UnitTestCommandResult> core) : base(core)
        {
        }

        protected override Task<My_003_UnitTestCommand> PreProcessAsync(My_003_UnitTestCommand arg)
        {
            var newArg = new My_003_UnitTestCommand
            {
                PreNumber = arg.PreNumber + 1,
                PostNumber = arg.PostNumber + 1
            };

            return base.PreProcessAsync(newArg);
        }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_002_Interceptor : RequestInterceptorBase<My_002_UnitTestCommand, My_002_UnitTestCommandResult>
    {
        public My_002_Interceptor(ExecuteDelegate<My_002_UnitTestCommand, My_002_UnitTestCommandResult> core) : base(core)
        {
        }

        protected override Task<My_002_UnitTestCommandResult> PostProcessAsync(My_002_UnitTestCommandResult result)
        {
            return Task.FromResult(new My_002_UnitTestCommandResult()
            {
                PreNumber = result.PreNumber + 1,
                PostNumber = result.PostNumber + 1
            });
        }
    }
}
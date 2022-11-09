// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Foundation.Test
{
    public class My_005_UntiTestCommandExecutor : IRequestHandler<My_005_UnitTestCommand, My_005_UnitTestCommandResult>
    {
        public Task<My_005_UnitTestCommandResult> ExecuteAsync(
            My_005_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_005_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
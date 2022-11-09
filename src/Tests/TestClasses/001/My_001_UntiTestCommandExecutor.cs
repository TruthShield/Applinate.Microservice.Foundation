// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Foundation.Test
{
    public class My_001_UntiTestCommandExecutor : IRequestHandler<My_001_UnitTestCommand, My_001_UnitTestCommandResult>
    {
        public Task<My_001_UnitTestCommandResult> ExecuteAsync(
            My_001_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_001_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
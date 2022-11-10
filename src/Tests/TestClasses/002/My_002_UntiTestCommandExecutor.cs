// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_002_UntiTestCommandExecutor : IRequestHandler<My_002_UnitTestCommand, My_002_UnitTestCommandResult>
    {
        public Task<My_002_UnitTestCommandResult> ExecuteAsync(
            My_002_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_002_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
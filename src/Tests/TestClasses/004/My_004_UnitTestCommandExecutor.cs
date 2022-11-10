// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_004_UnitTestCommandExecutor : IRequestHandler<My_004_UnitTestCommand, My_004_UnitTestCommandResult>
    {
        public Task<My_004_UnitTestCommandResult> ExecuteAsync(
            My_004_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_004_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
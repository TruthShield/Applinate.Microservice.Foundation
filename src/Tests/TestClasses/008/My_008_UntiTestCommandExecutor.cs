// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_008_UntiTestCommandExecutor : IRequestHandler<My_008_UnitTestCommand, My_008_UnitTestCommandResult>
    {
        public Task<My_008_UnitTestCommandResult> ExecuteAsync(
            My_008_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_008_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{

    [ServiceRequest(ServiceType.Integration)]
    public class My_009_UntiTestCommandExecutor : IRequestHandler<My_009_UnitTestCommand, My_009_UnitTestCommandResult>
    {
        public Task<My_009_UnitTestCommandResult> ExecuteAsync(
            My_009_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_009_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
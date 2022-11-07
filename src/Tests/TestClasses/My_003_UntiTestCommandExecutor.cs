// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [ServiceRequest(ServiceType.Orchestration)]
    public class My_003_UntiTestCommandExecutor : IHandleRequest<My_003_UnitTestCommand, My_003_UnitTestCommandResult>
    {
        public Task<My_003_UnitTestCommandResult> ExecuteAsync(
            My_003_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_003_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{

    [ServiceRequest(ServiceType.Calculation)]
    public class My_007_UntiTestCommandExecutor : IRequestHandler<My_007_UnitTestCommand, My_007_UnitTestCommandResult>
    {
        public Task<My_007_UnitTestCommandResult> ExecuteAsync(
            My_007_UnitTestCommand arg,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new My_007_UnitTestCommandResult
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
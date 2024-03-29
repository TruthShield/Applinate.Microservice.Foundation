// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_006_UnitTestIntegratorCommandExecutor : IRequestHandler<My_006_UnitTestIntegratorCommand, My_006_UnitTestIntegratorCommandResult>
    {
        public Task<My_006_UnitTestIntegratorCommandResult> ExecuteAsync(
            My_006_UnitTestIntegratorCommand arg,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new My_006_UnitTestIntegratorCommandResult());
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [Service(ServiceType.Orchestration)]
    public class My_006_UnitTestOrchestratorCommandExecutor : IHandleRequest<My_006_UnitTestOrchestratorCommand, My_006_UnitTestOrchestratorCommandResult>
    {
        public async Task<My_006_UnitTestOrchestratorCommandResult> ExecuteAsync(
            My_006_UnitTestOrchestratorCommand arg,
            CancellationToken cancellationToken = default)
        {
            var command = new My_006_UnitTestCalculatorCommand() { };
            _ = await command.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            return new My_006_UnitTestOrchestratorCommandResult();
        }
    }
}
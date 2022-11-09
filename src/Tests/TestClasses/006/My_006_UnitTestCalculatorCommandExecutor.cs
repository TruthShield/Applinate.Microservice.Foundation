// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_006_UnitTestCalculatorCommandExecutor : IRequestHandler<My_006_UnitTestCalculatorCommand, My_006_UnitTestCalculatorCommandResult>
    {
        public async Task<My_006_UnitTestCalculatorCommandResult> ExecuteAsync(
            My_006_UnitTestCalculatorCommand arg,
            CancellationToken cancellationToken = default)
        {
            var command = new My_006_UnitTestIntegratorCommand();
            _ = await command.ExecuteAsync(cancellationToken).ConfigureAwait(false);
            return new My_006_UnitTestCalculatorCommandResult();
        }
    }
}
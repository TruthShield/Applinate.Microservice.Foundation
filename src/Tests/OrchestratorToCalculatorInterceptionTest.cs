// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Applinate;
    using Applinate.Test;
    using FluentAssertions;
    using System.Threading.Tasks;
    using Xunit;

    public class OrchestratorToCalculatorInterceptionTest : TestBase
    {
        public OrchestratorToCalculatorInterceptionTest() : base(ServiceType.Orchestration) { }

        [Fact]
        public async Task Interception_Factory_Works_For_Orchestrator()
        {
            var result = await new My_007_UnitTestCommand
            {
                PostNumber = 1,
                PreNumber = 2,
            }
            .ExecuteAsync()
            .ConfigureAwait(false);

            result.PreNumber.Should().Be(3);

            result.PostNumber.Should().Be(2);
        }
    }
}
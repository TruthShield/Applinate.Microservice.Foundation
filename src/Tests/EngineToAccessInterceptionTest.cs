// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Applinate;
    using Applinate.Test;
    using FluentAssertions;
    using System.Threading.Tasks;
    using Xunit;

    public class CalculatorToIntegratorInterceptionTest : TestBase
    {
        public CalculatorToIntegratorInterceptionTest() : base(ServiceType.Calculation)
        {
        }

        [Fact]
        public async Task InterceptionFactoryWorksForCalculator()
        {
            var result = await new My_009_UnitTestCommand
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
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Applinate;
    using Applinate.Test;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class ClientTests : TestBase
    {
        [Fact()]
        public async Task ClientTest()
        {
            ServiceClient c = new ServiceClient(Guid.NewGuid(), new(Guid.NewGuid(), 1,2,3));

            c.StartNewConversation();

            var command = new My_001_UnitTestCommand
            {
                PostNumber = 1,
                PreNumber = 2,
            };

            var result = await command.ExecuteAsync();

            result.PreNumber.Should().Be(2);

            result.PostNumber.Should().Be(1);
        }
    }
}
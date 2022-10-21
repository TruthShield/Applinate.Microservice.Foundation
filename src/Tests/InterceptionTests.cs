// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Applinate;
    using Applinate.Test;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using Xunit;
    using Polly;

    public class InterceptionTests : TestBase
    {
        private static readonly Policy _Policy = Policy
            .Handle<Exception>()
            .WaitAndRetry(new[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) });
            

        [Fact] 
        [Trait("Category", "LocalOnly")] // TODO: work out server failure issue
        public Task InterceptionFactoryWorksForClient()
        {
            return _Policy.Execute(async () =>
            {
                ServiceClient c = new ServiceClient(Guid.NewGuid(), new(Guid.NewGuid(), 1, 2, 3));

                c.StartNewConversation();

                var command = new My_005_UnitTestCommand
                {
                    PostNumber = 1,
                    PreNumber = 2,
                };
                var result = await command.ExecuteAsync().ConfigureAwait(false);

                result.PreNumber.Should().Be(3);

                result.PostNumber.Should().Be(2);
            });
        }

        [Fact]
        public async Task MultipleTypedInterceptionOrderingWorks()
        {
            ServiceClient c = new ServiceClient(Guid.NewGuid(), new(Guid.NewGuid(), 1,2,3));

            c.StartNewConversation();

            var command = new My_004_UnitTestCommand
            {
                PostNumber = 1,
                PreNumber = 2,
            };

            var result = await command.ExecuteAsync();

            result.PreNumber.Should().Be(30);

            result.PostNumber.Should().Be(20);
        }

        [Fact]
        public async Task TypedInterceptionPostProcessingWorks()
        {
            ServiceClient c = new ServiceClient(Guid.NewGuid(), new(Guid.NewGuid(),1,2,3));

            c.StartNewConversation();

            var result = await new My_002_UnitTestCommand
            {
                PostNumber = 1,
                PreNumber = 2,
            }.ExecuteAsync();

            result.PreNumber.Should().Be(3);

            result.PostNumber.Should().Be(2);
        }

        [Fact]
        public async Task TypedInterceptionPreProcessingWorks()
        {
            ServiceClient c = new ServiceClient(Guid.NewGuid(), new(Guid.NewGuid(),1,2,3));
            c.StartNewConversation();

            var result = await new My_003_UnitTestCommand
            {
                PostNumber = 1,
                PreNumber = 2,
            }.ExecuteAsync();

            result.PreNumber.Should().Be(3);

            result.PostNumber.Should().Be(2);
        }
    }
}
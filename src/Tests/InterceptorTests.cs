// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Applinate;
    using Applinate.Test;
    using FluentAssertions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    public class InterceptorTests : TestBase
    {
        [Fact()]
        public async Task ContextChangesUpdate()
        {
            var queue = new Queue<RequestContextChange>();

            InfrastructureEventSink.For.ScopedContextChange().Changed += (sender, args) =>
            {
                queue.Enqueue(args);
            };

            ServiceClient c = new ServiceClient(Guid.NewGuid(), new(Guid.NewGuid(), 1,2,3));
            c.StartNewConversation();
            RequestContext.Current.ServiceType.Should().Be(ServiceType.Client);

            var result = await new My_006_UnitTestOrchestratorCommand().ExecuteAsync();

            RequestContext.Current.ServiceType.Should().Be(ServiceType.Client);

            queue.Count().Should().Be(6);
            queue.Where(x => x.ContextChangeType == RequestContextChangeType.Entry).Count().Should().Be(3, "three service entries");
            queue.Where(x => x.ContextChangeType == RequestContextChangeType.Exit).Count().Should().Be(3, "three service exits");

            var changes = queue.ToArray();

            changes[0].Should().Be(RequestContextChange.Entry<My_006_UnitTestOrchestratorCommand , My_006_UnitTestOrchestratorCommandResult>(1));
            changes[1].Should().Be(RequestContextChange.Entry<My_006_UnitTestCalculatorCommand  , My_006_UnitTestCalculatorCommandResult>(2));
            changes[2].Should().Be(RequestContextChange.Entry<My_006_UnitTestIntegratorCommand  , My_006_UnitTestIntegratorCommandResult>(3));
            changes[3].Should().Be(RequestContextChange.Exit<My_006_UnitTestIntegratorCommand   , My_006_UnitTestIntegratorCommandResult>(4));
            changes[4].Should().Be(RequestContextChange.Exit<My_006_UnitTestCalculatorCommand   , My_006_UnitTestCalculatorCommandResult>(5));
            changes[5].Should().Be(RequestContextChange.Exit<My_006_UnitTestOrchestratorCommand  , My_006_UnitTestOrchestratorCommandResult>(6));
        }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Applinate;
    using Applinate.Test;
    using FluentAssertions;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class ContextChangeTests: TestBase
    {
        public ContextChangeTests() : base(ServiceType.Orchestration) { }

        [Fact()]
        public async Task Orchestrator_To_Integrator_Context_Is_Reset_Correctly()
        {

            var command = new BasicIntegrationRequest
            {
                PostNumber = 1,
                PreNumber = 2,
            };

            RequestContextProvider.Instance.ServiceType.Should().Be(ServiceType.Orchestration);

            var contextChangeHit = 0;


            InfrastructureEventSink.For.ScopedContextChange().Changed += (s, a) =>
            {
                contextChangeHit++;

                switch (contextChangeHit)
                {
                    case 1:
                        RequestContextProvider.Instance.ServiceType.Should().Be(ServiceType.Integration, "access entry");
                        break;
                    case 2:
                        RequestContextProvider.Instance.ServiceType.Should().Be(ServiceType.Orchestration, "manager exit");
                        break;
                    default:
                        true.Should().BeFalse("this should not get hit... there are only 2 changes ");
                        break;
                }

            };

            _ = await command.ExecuteAsync();


            contextChangeHit.Should().Be(2);

            RequestContextProvider.Instance.ServiceType.Should().Be(ServiceType.Orchestration);
        }

        [Fact()]
        public async Task Orchestrator_To_Integrator_Context_Is_Reset_Correctly_When_Faulted()
        {

            var command = new BasicIntegrationRequest
            {
                PostNumber = 1,
                PreNumber = 2,
            };

            RequestContextProvider.Instance.ServiceType.Should().Be(ServiceType.Orchestration);

            var contextChangeHit = 0;


            InfrastructureEventSink.For.ScopedContextChange().Changed += (s, a) =>
            {
                contextChangeHit++;

                switch (contextChangeHit)
                {
                    case 1:
                        RequestContextProvider.Instance.ServiceType.Should().Be(ServiceType.Integration, "access entry");
                        throw new InvalidOperationException("test exception to check flow when a fault occurrs in the child");
                    default:
                        break;
                }

            };

            try
            {
                _ = await command.ExecuteAsync();
            }
            catch { }

            RequestContextProvider.Instance.ServiceType.Should().Be(ServiceType.Orchestration);
        }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using FluentAssertions;
    using Xunit;

    public class DependencyInversionTests
    {
        [Fact(DisplayName = "When a registered service provider is requested the service registry shall return the registered provider.")]
        public void Rgistration_Works()
        {
            ServiceProvider.RegisterSingleton<IRegistrationTestService, RegistrationTestService>();

            var service = ServiceProvider.Locate<IRegistrationTestService>();

            service.Should().NotBeNull();
        }
    }
}
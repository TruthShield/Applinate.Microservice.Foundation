// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Applinate.Test;

    public sealed class ServiceProxyTest: TestBase
    {
        [Fact]
        public async Task DispatchRequestFromInterface()
        {
            var proxy = ServiceProvider.Locate<IMy_0010_Service>();
            var response = await proxy.HandleRequestAsync(new());

            _ = response;
        }

        [Fact()]
        public async Task HandleRequestFromInterfaceImplementation()
        {
            var proxy = ServiceProvider.Locate<IMy_011_Service>();
            var response = await proxy.HandleRequestAsync(new()
            {
                Value = 123,
            });

            response.Value.Should().Be(123);
        }

    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class BasicIntegrationTestExecutor : IRequestHandler<BasicIntegrationRequest, BasicIntegrationResponse>
    {
        public Task<BasicIntegrationResponse> ExecuteAsync(BasicIntegrationRequest arg, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new BasicIntegrationResponse
            {
                PreNumber = arg.PreNumber,
                PostNumber = arg.PostNumber
            });
        }
    }
}
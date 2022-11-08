// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [ServiceRequest(ServiceType.Integration)]
    public class BasicIntegrationRequest : IReturn<BasicIntegrationResponse>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }

    public class BasicIntegrationResponse : IHaveRequestStatus
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }

        public RequestStatus Status => throw new NotImplementedException();
    }

    [ServiceRequest(ServiceType.Integration)]
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
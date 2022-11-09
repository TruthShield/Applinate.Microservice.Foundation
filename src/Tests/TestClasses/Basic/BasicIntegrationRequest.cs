// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [ServiceRequest(ServiceType.Integration)]
    public class BasicIntegrationRequest : IReturn<BasicIntegrationResponse>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }
}
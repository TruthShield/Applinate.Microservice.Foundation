// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [Service(ServiceType.Orchestration)]
    public interface IMy_0010_Service
    {
        // note: signature discovery
        Task<My_010_UnitTestResponse> HandleRequestAsync(
            My_010_UnitTestRequestParams arg,
            CancellationToken cancellationToken = default);


        // note: signature discovery
        //Task<RequestStatus> HandleCommandAsync(
        //    My_010_UnitTestCommandParams arg,
        //    CancellationToken cancellationToken = default);
    }
}
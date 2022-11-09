// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [Service(ServiceType.Orchestration)]
    public interface IMy_011_Service
    {
        Task<My_011_UnitTestResponse> HandleRequestAsync(
            My_011_UnitTestRequestParams arg,
            CancellationToken cancellationToken = default);
    }
}
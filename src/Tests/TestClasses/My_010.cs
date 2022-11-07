// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Microsoft.Extensions.DependencyInjection;

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

    internal sealed class My_010_Service : IHandleRequest<My_010_UnitTestRequestParams, My_010_UnitTestResponse>
    {
        public Task<My_010_UnitTestResponse> ExecuteAsync(My_010_UnitTestRequestParams arg, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }

    [ServiceRequest(ServiceType.Orchestration)]
    public class My_010_UnitTestRequestParams : IReturn<My_010_UnitTestResponse>
    {
    }

    public class My_010_UnitTestResponse : IHaveRequestStatus
    {
        public RequestStatus Status => throw new NotImplementedException();
    }
}
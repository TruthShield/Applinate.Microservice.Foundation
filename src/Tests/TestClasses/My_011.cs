//// Copyright (c) TruthShield, LLC. All rights reserved.
//namespace Applinate.Foundation.Test
//{
//    using Namotion.Reflection;

//    [Service(ServiceType.Orchestration)]
//    public interface IMy_011_Service
//    {
//        Task<My_011_UnitTestResponse> HandleRequestAsync(
//            My_011_UnitTestRequestParams arg,
//            CancellationToken cancellationToken = default);
//    }

//    internal sealed class My_011_Service : IMy_011_Service
//    {
//        public Task<My_011_UnitTestResponse> HandleRequestAsync(My_011_UnitTestRequestParams arg, CancellationToken cancellationToken = default)
//        {
//            return Task.FromResult(new My_011_UnitTestResponse()
//            {
//                Value = arg.Value
//            });
//        }
//    }

//    [ServiceRequest(ServiceType.Orchestration)] // TODO: TO SERIVER REQUEST!!!!!!!!!!!!!!
//    public class My_011_UnitTestRequestParams : IReturn<My_011_UnitTestResponse>
//    {
//        public int Value { get; set; }
//    }

//    public class My_011_UnitTestResponse : IHaveRequestStatus
//    {
//        public RequestStatus Status => RequestStatus.Success;

//        public int Value { get; set; }
//    }
//}
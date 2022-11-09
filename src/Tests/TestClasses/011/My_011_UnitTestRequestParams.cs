// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [ServiceRequest(ServiceType.Orchestration)]
    public class My_011_UnitTestRequestParams : IReturn<My_011_UnitTestResponse>
    {
        public int Value { get; set; }
    }
}
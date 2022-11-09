// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Foundation.Test
{
    [ServiceRequest(ServiceType.Orchestration)]
    public class My_001_UnitTestCommand : IReturn<My_001_UnitTestCommandResult>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [ServiceRequest(ServiceType.Integration)]
    public class My_008_UnitTestCommand : IReturn<My_008_UnitTestCommandResult>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }
}
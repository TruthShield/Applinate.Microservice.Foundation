// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [ServiceRequest(ServiceType.Calculation)]
    public class My_007_UnitTestCommand : IReturn<My_007_UnitTestCommandResult>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }
}
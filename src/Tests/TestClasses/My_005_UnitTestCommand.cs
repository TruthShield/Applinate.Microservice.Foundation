// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [Service(ServiceType.Orchestration)]
    public class My_005_UnitTestCommand : IReturn<My_005_UnitTestCommandResult>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }
}
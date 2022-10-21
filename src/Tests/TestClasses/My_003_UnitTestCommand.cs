// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    [Service(ServiceType.Orchestration)]
    public class My_003_UnitTestCommand : IReturn<My_003_UnitTestCommandResult>
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }
    }
}
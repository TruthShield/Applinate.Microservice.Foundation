// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Foundation.Test
{
    public class My_005_UnitTestCommandResult : IHaveRequestStatus
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }

        public RequestStatus Status => throw new NotImplementedException();
    }
}
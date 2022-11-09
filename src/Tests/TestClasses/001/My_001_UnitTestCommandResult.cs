// Copyright (c) TruthShield, LLC. All rights reserved.

namespace Applinate.Foundation.Test
{

    public class My_001_UnitTestCommandResult : IHaveResponseStatus
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }

        public ResponseStatus Status => ResponseStatus.Success;
    }
}
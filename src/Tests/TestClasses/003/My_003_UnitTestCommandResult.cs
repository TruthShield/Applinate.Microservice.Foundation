// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_003_UnitTestCommandResult : IHaveResponseStatus
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }

        public ResponseStatus Status => throw new NotImplementedException();
    }
}
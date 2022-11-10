// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    public class My_007_UnitTestCommandResult:IHaveResponseStatus
    {
        public int PostNumber { get; set; }
        public int PreNumber { get; set; }

        public ResponseStatus Status => throw new System.NotImplementedException();
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{

    public class My_011_UnitTestResponse : IHaveResponseStatus
    {
        public ResponseStatus Status => ResponseStatus.Success;

        public int Value { get; set; }
    }
}
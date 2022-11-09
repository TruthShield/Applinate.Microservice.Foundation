// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{

    public class My_011_UnitTestResponse : IHaveRequestStatus
    {
        public RequestStatus Status => RequestStatus.Success;

        public int Value { get; set; }
    }
}
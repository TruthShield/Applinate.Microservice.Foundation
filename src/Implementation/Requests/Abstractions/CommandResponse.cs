// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public sealed class CommandResponse: IHaveRequestStatus
    {
        public static readonly CommandResponse Success = new(RequestStatus.Success);

        public CommandResponse(RequestStatus status)
        {
            Status = status; 
        }

        public RequestStatus Status { get; init; }

        public static CommandResponse Failure(string str) => new(RequestStatus.Failure(str));
    }
}
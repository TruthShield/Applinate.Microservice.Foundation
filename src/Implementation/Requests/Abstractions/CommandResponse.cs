// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    public sealed class CommandResponse: IHaveResponseStatus
    {
        public static readonly CommandResponse Success = new(ResponseStatus.Success);

        public CommandResponse(ResponseStatus status)
        {
            Status = status; 
        }

        public ResponseStatus Status { get; init; }

        public static CommandResponse Failure(string str) => new(ResponseStatus.Failure(str));
    }
}
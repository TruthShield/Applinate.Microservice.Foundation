// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Collections.Immutable;

    public record ResponseStatus
    {
        public ResponseStatus(RequestStatusMessage[]?messages = null)
        {
            Messages = (messages ?? Array.Empty<RequestStatusMessage>()).ToImmutableArray();
        }

        public bool IsSuccess => 
            !Messages.Any(x => x.MessageType == RequestStatusMessageType.Error);

        public ImmutableArray<RequestStatusMessage> Messages { get; init; }

        public static readonly ResponseStatus Success = new();

        public static ResponseStatus Build(
            string[]? information = null,
            string[]? warnings = null,
            string[]? errors = null) => new(
                    (information?.Select(RequestStatusMessage.Information) ?? Enumerable.Empty<RequestStatusMessage>())
                    .Union(warnings?.Select(RequestStatusMessage.Warning) ?? Enumerable.Empty<RequestStatusMessage>())
                    .Union(errors?.Select(RequestStatusMessage.Error) ?? Enumerable.Empty<RequestStatusMessage>())
                    .ToArray());

        public static ResponseStatus Failure(params string[] errorMessages) => Build(null, null, errorMessages);
    }
}
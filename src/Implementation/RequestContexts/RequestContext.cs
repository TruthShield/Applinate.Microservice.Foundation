// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class RequestContext. This class cannot be inherited.
    ///
    /// This is the ambient context for the request that holds all the contextual information
    /// about the originating call and execution state.
    /// </summary>
    [DataContract]
    public sealed record RequestContext
    {
        public static readonly RequestContext Empty = new(
            currentServiceType: ServiceType.None,
            sessionId: SequentialGuid.Empty,
            conversationId: SequentialGuid.Empty,
            appContext: AppContextKey.Empty,
            requestCallCount: 0,
            decoratorCallCount: 0,
            metadata: BuildMetadata());

        private static ImmutableDictionary<string, string> BuildMetadata(
            IDictionary<string, string>? metadata = null) =>
                metadata?.ToImmutableDictionary() ??
                new Dictionary<string, string>(StringComparer.Ordinal).ToImmutableDictionary();

        [System.Text.Json.Serialization.JsonConstructor]
        [Newtonsoft.Json.JsonConstructor]
        public RequestContext(
            ServiceType currentServiceType,
            SequentialGuid sessionId,
            SequentialGuid conversationId,
            AppContextKey appContext,
            int requestCallCount = 0,
            int decoratorCallCount = 0,
            IImmutableDictionary<string, string>? metadata = null,
            SequentialGuid? userProfileId = null)
        {
            ServiceType = currentServiceType;
            SessionId = sessionId;
            ConversationId = conversationId;
            AppContextKey = appContext;
            RequestCallCount = requestCallCount;
            DecoratorCallCount = decoratorCallCount;
            Metadata = metadata ?? BuildMetadata();
            UserProfileId = userProfileId ?? SequentialGuid.Empty;
        }

        [DataMember] public AppContextKey AppContextKey { get; init; }
        [DataMember] public SequentialGuid ConversationId { get; init; }
        [DataMember] public int DecoratorCallCount { get; init; }
        [DataMember] public ServiceType ServiceType { get; init; }
        [DataMember] public IImmutableDictionary<string, string> Metadata { get; init; }
        [DataMember] public int RequestCallCount { get; init; }
        [DataMember] public SequentialGuid SessionId { get; init; }
        [DataMember] public SequentialGuid UserProfileId { get; init; }
    }
}
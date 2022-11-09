// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using Microsoft.Extensions.Primitives;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Class RequestContext. This class cannot be inherited.
    ///
    /// This is the ambient context for the request that holds all the contextual information
    /// about the originating call and execution state.
    ///
    /// Implements the <see cref="System.IEquatable{Applinate.RequestContext}" />
    /// </summary>
    /// <seealso cref="System.IEquatable{Applinate.RequestContext}" />
    public sealed record RequestContext
    {
        public static readonly RequestContext Empty = new(
            currentServiceType : ServiceType.None,
            sessionId          : SequentialGuid.Empty,
            conversationId     : SequentialGuid.Empty,
            appContext         : AppContextKey.Empty,
            requestCallCount   : 0,
            decoratorCallCount : 0,
            metadata           : BuildMetadata());

        private static ReadOnlyDictionary<string, StringValues> BuildMetadata(IDictionary<string, StringValues>? metadata = null)
        {
            return new ReadOnlyDictionary<string, StringValues>(metadata ?? new Dictionary<string, StringValues>(StringComparer.Ordinal));
        }

        private class ServiceContextWrapper { public RequestContext RequestContext; }

        /// wrapping the Immutable ServiceContext value in a mutable wrapper class instance which internal AsyncLocal stores,
        /// so changes are done to the wrapper, not the immutable Stext.  This workaround is for maintaining a reference
        /// to the same ServiceContext on both async context entry and exit.
        /// NOTE: This structure only works with single linear async strand(flow) like a "logical thread"
        /// as it uses the mutable value, it should not be transacted from multi-threaded child callers, such as
        /// forking sub-tasks
        private static readonly AsyncLocal<ServiceContextWrapper> _Current = new AsyncLocal<ServiceContextWrapper>();

        public RequestContext(
            ServiceType currentServiceType,
            SequentialGuid sessionId,
            SequentialGuid conversationId,
            AppContextKey appContext,
            int requestCallCount                                = 0,
            int decoratorCallCount                              = 0,
            IReadOnlyDictionary<string, StringValues>? metadata = null,
            SequentialGuid? userProfileId                       = null)
        {
            ServiceType               = currentServiceType;

            SessionId                 = sessionId;
            ConversationId            = conversationId;
            AppContextKey             = appContext;
            RequestCallCount          = requestCallCount;
            DecoratorCallCount        = decoratorCallCount;
            Metadata                  = metadata              ?? BuildMetadata();
            UserProfileId             = userProfileId         ?? SequentialGuid.Empty;
        }

        public static RequestContext Current
        {
            get => _Current?.Value?.RequestContext ?? RequestContext.Empty;
            internal set
            {
                if (_Current.Value is null)
                {
                    _Current.Value = new ServiceContextWrapper() { RequestContext = value };
                    return;
                }

                _Current.Value.RequestContext = value;
            }
        }

        public AppContextKey AppContextKey                         { get; init; }
        public SequentialGuid ConversationId                       { get; init; }
        public int DecoratorCallCount                              { get; init; }
        public ServiceType ServiceType                             { get; init; }
        public IReadOnlyDictionary<string, StringValues> Metadata  { get; init; }
        public int RequestCallCount                                { get; init; }
        public SequentialGuid SessionId { get; init; }
        public SequentialGuid UserProfileId { get; init; }

        public static void SetAsClient() =>
            Current = Current with { ServiceType = ServiceType.Client };
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
using Microsoft.Extensions.Primitives;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Applinate
{
    /// <summary>
    /// Class ServiceClient. This class cannot be inherited.
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <example>
    /// // Step 1: Build the client
    /// ServiceClient c = new ServiceClient(
    ///     userId, // from header
    ///     Config.ApplicationId, // from appid
    ///     LanguageCodeKey.Build(ddl_Language.SelectedValue?.ToString() ?? LanguageCodeKey.EN.Root));
    /// 
    /// // Step 2: Start the conversation
    /// c.StartNewConversation();
    /// 
    /// // Step 3: Execute the request
    /// var result = await new ContentConfigurationQuery().ExecuteAsync();
    /// </example>
    public sealed class ServiceClient
    {
        public ServiceClient(
            Guid userProfileId,
            AppContextKey appContext, 
            IDictionary<string, StringValues>? metadata = null)
        {
            /* todo; get all ambient context stuff necessasry for dispatching */

            Context = new RequestContext(
                currentServiceType : ServiceType.Client,
                sessionId          : Guid.NewGuid(),
                conversationId     : Guid.NewGuid(),
                appContext         : appContext,
                requestCallCount   : 0,
                decoratorCallCount : 0,
                userProfileId      : userProfileId,
                metadata           : BuildMetadata(metadata));               
        }

        /// <summary>
        /// Sets the context needed to send a command and starts a new conversation.
        /// </summary>
        public void StartNewConversation(IDictionary<string, StringValues>? metadata = null) => 
            RequestContext.Current =
                Context
                with
                { ConversationId = Guid.NewGuid() }
                with
                { Metadata = BuildMetadata(metadata) };

        private static IReadOnlyDictionary<string, StringValues> BuildMetadata(IDictionary<string, StringValues>? metadata) =>
            new ReadOnlyDictionary<string, StringValues>(
                metadata ?? 
                new Dictionary<string, StringValues>(StringComparer.Ordinal));

        public RequestContext Context { get; }
    }
}
// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// Class RequestContextProvider. This class cannot be inherited.
    ///
    /// This is the ambient context for the request that holds all the contextual information
    /// about the originating call and execution state.
    ///
    /// Implements the <see cref="System.IEquatable{Applinate.RequestContext}" />
    /// </summary>
    /// <seealso cref="System.IEquatable{Applinate.RequestContext}" />
    public static class RequestContextProvider
    {
        public static AppContextKey AppContextKey                         => RequestContext.Current.AppContextKey;
        public static SequentialGuid ConversationId                       => RequestContext.Current.ConversationId;
        public static int DecoratorCallCount                              => RequestContext.Current.DecoratorCallCount;
        public static ServiceType ServiceType                             => RequestContext.Current.ServiceType;
        public static IImmutableDictionary<string, StringValues> Metadata => RequestContext.Current.Metadata;
        public static int RequestCallCount                                => RequestContext.Current.RequestCallCount;
        public static SequentialGuid SessionId                            => RequestContext.Current.SessionId;
        public static SequentialGuid UserProfileId                        => RequestContext.Current.UserProfileId;

        public static bool TryGetMetadata<T>(string key, out T value) 
        {
            var result = Metadata.TryGetValue(key, out var v);

            if(! result)
            {
                value = default;
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(v);

            return true;
        }

        public static bool TryGetMetadata<T>(out T value)
            where T:class, new() => 
            TryGetMetadata<T>(nameof(T), out value);

        public static bool ContainsMetadata<T>(string key)
            where T : class, new() => 
            Metadata.ContainsKey(key);

        public static bool ContainsMetadata<T>()
            where T : class, new() => 
            ContainsMetadata<T>(nameof(T));

        public static void SetMetadata<T>(string key, T value)
            where T : class, new() =>
            RequestContext.Current = Metadata.ContainsKey(key)
                ? (RequestContext.Current with
                {
                    Metadata = Metadata.SetItem(key, JsonConvert.SerializeObject(value))
                })
                : (RequestContext.Current with
                {
                    Metadata = Metadata.Add(key, JsonConvert.SerializeObject(value))
                });

        public static void SetMetadata<T>(T value)
            where T : class, new() =>
            SetMetadata<T>(nameof(T), value);

        public static void RemoveMetadata<T>(string key)
            where T : class, new() =>
            RequestContext.Current = Metadata.ContainsKey(key) ?
                RequestContext.Current with
                {
                    Metadata = Metadata.Remove(key)
                } :
                RequestContext.Current;

        public static void RemoveMetadata<T>()
            where T : class, new() =>
            RemoveMetadata<T>(nameof(T));

    }
}
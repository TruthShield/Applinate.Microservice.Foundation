// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate
{
    using Newtonsoft.Json;
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
        public static AppContextKey AppContextKey                   => Instance.AppContextKey;
        public static SequentialGuid ConversationId                 => Instance.ConversationId;
        public static int DecoratorCallCount                        => Instance.DecoratorCallCount;
        public static IImmutableDictionary<string, string> Metadata => Instance.Metadata;
        public static int RequestCallCount                          => Instance.RequestCallCount;
        public static ServiceType ServiceType                       => Instance.ServiceType;
        public static SequentialGuid SessionId                      => Instance.SessionId;
        public static SequentialGuid UserProfileId                  => Instance.UserProfileId;



        private class ServiceContextWrapper { public RequestContext RequestContext; }

        /// wrapping the Immutable ServiceContext value in a mutable wrapper class instance which internal AsyncLocal stores,
        /// so changes are done to the wrapper, not the immutable Stext.  This workaround is for maintaining a reference
        /// to the same ServiceContext on both async context entry and exit.
        /// NOTE: This structure only works with single linear async strand(flow) like a "logical thread"
        /// as it uses the mutable value, it should not be transacted from multi-threaded child callers, such as
        /// forking sub-tasks
        private static readonly AsyncLocal<ServiceContextWrapper> _Instance = new AsyncLocal<ServiceContextWrapper>();

        internal static RequestContext Instance
        {
            get => _Instance?.Value?.RequestContext ?? RequestContext.Empty;
            set
            {
                if (_Instance.Value is null)
                {
                    _Instance.Value = new ServiceContextWrapper() { RequestContext = value };
                    return;
                }

                _Instance.Value.RequestContext = value;
            }
        }

        public static bool ContainsMetadata<T>(string key)
            where T : class, new() =>
            Metadata.ContainsKey(key);

        public static bool ContainsMetadata<T>()
            where T : class, new() =>
            ContainsMetadata<T>(nameof(T));

        public static void RemoveMetadata<T>(string key)
            where T : class, new() =>
            Instance = Metadata.ContainsKey(key) ?
                Instance with
                {
                    Metadata = Metadata.Remove(key)
                } :
                Instance;

        public static void RemoveMetadata<T>()
            where T : class, new() =>
            RemoveMetadata<T>(nameof(T));

        public static void SetMetadata<T>(string key, T value)
            where T : class, new() =>
            Instance = Metadata.ContainsKey(key)
                ? (Instance with
                {
                    Metadata = Metadata.SetItem(key, JsonConvert.SerializeObject(value))
                })
                : (Instance with
                {
                    Metadata = Metadata.Add(key, JsonConvert.SerializeObject(value))
                });

        public static void SetMetadata<T>(T value)
            where T : class, new() =>
            SetMetadata<T>(nameof(T), value);

        public static bool TryGetMetadata<T>(string key, out T value)
        {
            var result = Metadata.TryGetValue(key, out var v);

            if (!result)
            {
                value = default;
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(v);

            return true;
        }

        public static bool TryGetMetadata<T>(out T value)
            where T : class, new() =>
            TryGetMetadata<T>(nameof(T), out value);
    }
}
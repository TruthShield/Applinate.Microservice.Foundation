// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Xunit;

    public class RequestContextSerializationTests
    {

        [Fact]
        public void SequentialGuidSerialization_Works()
        {
            var x = SequentialGuid.NewGuid();


            var ser = JsonConvert.SerializeObject(x);

            var deser = JsonConvert.DeserializeObject<SequentialGuid>(ser);

            deser.Should().Be(x);

        }

        [Fact]
        public void AppContextKeySerialization_Works()
        {
            var x = new AppContextKey(
                    SequentialGuid.NewGuid(), 1, 0, 2);


            var ser = JsonConvert.SerializeObject(x);

            var deser = JsonConvert.DeserializeObject<AppContextKey>(ser);

            deser.Should().Be(x);

        }
        [Fact]
        public void ContextSerialization_Works()
        {
            var x = new RequestContext(ServiceType.Orchestration,
                SequentialGuid.NewGuid(),
                SequentialGuid.NewGuid(),
                new AppContextKey(
                    SequentialGuid.NewGuid(),
                    1, 0, 2),
                2,
                3,
                new Dictionary<string, string>()
                {
                    {"key1", "value1"},
                }.ToImmutableDictionary(),
                SequentialGuid.NewGuid());

            var ser = JsonConvert.SerializeObject(x);

            var deser = JsonConvert.DeserializeObject<RequestContext>(ser);

            deser.AppContextKey.Should().Be(x.AppContextKey);
            deser.ConversationId.Should().Be(x.ConversationId);
            deser.DecoratorCallCount.Should().Be(x.DecoratorCallCount);
            deser.RequestCallCount.Should().Be(x.RequestCallCount);
            deser.ServiceType.Should().Be(x.ServiceType);
            deser.SessionId.Should().Be(x.SessionId);
            deser.UserProfileId.Should().Be(x.UserProfileId);
            
            deser.Metadata.Count.Should().Be(x.Metadata.Count);

            foreach(var kvp in x.Metadata)
            {
                deser.Metadata[kvp.Key].Should().Be(kvp.Value);
            }
        }
    }
}
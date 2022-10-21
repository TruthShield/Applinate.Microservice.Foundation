// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using FluentAssertions;
    using Xunit;
    public class SequentialGuidTests
    {
        [Fact]
        public void CompactAndUncompact()
        {
            SequentialGuid g = SequentialGuid.NewGuid();

            var str = g.ToCompactString();

            var uncompacted = str.FromCompactStringToSequentialGuid();

            g.Should().Be(uncompacted);
        }
    }
}
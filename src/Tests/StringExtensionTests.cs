// Copyright (c) TruthShield, LLC. All rights reserved.
namespace Applinate.Foundation.Test
{
    using FluentAssertions;
    using Xunit;

    public class StringExtensionTests
    {
        [Theory]
        [InlineData("TheQuickBrownFox", "the-quick-brown-fox")]
        [InlineData("The quick Brown Fox", "the-quick-brown-fox")]
        [InlineData("The quick Brown Fox ", "the-quick-brown-fox")]
        [InlineData(" The quick Brown Fox", "the-quick-brown-fox")]
        public void KebabCase(string input, string expected)
        {
            input.ToKebabCase().Should().Be(expected);
        }
    }
}
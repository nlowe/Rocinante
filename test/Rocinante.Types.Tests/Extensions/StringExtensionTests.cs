using Xunit;
using Rocinante.Types.Extensions;

namespace Rocinante.Types.Tests.Extensions
{
    public class StringExtensionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void IsNullOrEmpty_Valid(string str)
        {
            Assert.True(str.IsNullOrEmpty());
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData("aa")]
        [InlineData("  aaa  ")]
        public void IsNullOrEmpty_FalseForString(string str)
        {
            Assert.False(str.IsNullOrEmpty());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData(" \t\n")]
        public void IsNullOrWhitespace_Valid(string str)
        {
            Assert.True(str.IsNullOrWhiteSpace());
        }

        [Theory]
        [InlineData("foo")]
        [InlineData(" foo")]
        [InlineData("foo ")]
        [InlineData("\t \nfoo\n \t")]
        public void IsNullOrWhiteSpace_FalseForString(string str)
        {
            Assert.False(str.IsNullOrWhiteSpace());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void OrNull_Valid(string str)
        {
            Assert.Null(str.OrNull());
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("a")]
        [InlineData("aa")]
        [InlineData("  aaa  ")]
        public void OrNull_ReturnsStringIfPresent(string str)
        {
            Assert.Equal(str, str.OrNull());
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData(" \t\n")]
        public void OrBlank_Valid(string str)
        {
            Assert.Null(str.OrBlank());
        }

        [Theory]
        [InlineData("foo")]
        [InlineData(" foo")]
        [InlineData("foo ")]
        [InlineData("\t \nfoo\n \t")]
        public void OrBlank_ReturnsStringIfPresent(string str)
        {
            Assert.Equal(str, str.OrBlank());
        }

        [Theory]
        [InlineData("a/", "/")]
        [InlineData("ab", "b")]
        [InlineData("a/b/c/", "/")]
        public void AppendIfMissing_AlreadyPresent(string str, string suffix)
        {
            Assert.Equal(str, str.AppendIfMissing(suffix));
        }

        [Theory]
        [InlineData("a", "/")]
        [InlineData("a", "b")]
        [InlineData("a/b/c", "/")]
        public void AppendIfMissing_Missing(string str, string suffix)
        {
            Assert.Equal(str + suffix, str.AppendIfMissing(suffix));
        }
    }
}
using System;
using Rocinante.Types.Extensions;
using Xunit;

namespace Rocinante.Types.Tests.Extensions
{
    public class ExceptionExtensionTests
    {
        [Fact]
        public void EmptyStringForNullException()
        {
            Exception ex = null;
            Assert.Equal(string.Empty, ex.AllInnerMessages());
        }

        [Fact]
        public void NoSeperatorForSingleException()
        {
            Assert.Equal("foo", new Exception("foo").AllInnerMessages());
        }

        [Fact]
        public void MultipleExceptionsValid()
        {
            var sut = new Exception("foo", new Exception("bar", new Exception("baz")));

            Assert.Equal("foo\n-----\nbar\n-----\nbaz", sut.AllInnerMessages());
        }

        [Fact]
        public void CustomSeperator()
        {
            var sut = new Exception("foo", new Exception("bar", new Exception("baz")));

            Assert.Equal("foo/bar/baz", sut.AllInnerMessages("/"));
        }

        [Fact]
        public void ThrowsArgumentExceptionIfSeperatorNull()
        {
            try
            {
                var sut = new Exception("foo", new Exception("bar", new Exception("baz")));
                sut.AllInnerMessages(null);

                Assert.True(false, "Expected ArgumentException");            
            }
            catch(Exception ex)
            {
                Assert.IsAssignableFrom(typeof(ArgumentException), ex);
                Assert.Equal("The seperator cannot be null or empty\nParameter name: seperator", ex.Message);
            }
        }

        [Fact]
        public void ThrowsArgumentExceptionIfSeperatorEmpty()
        {
            try
            {
                var sut = new Exception("foo", new Exception("bar", new Exception("baz")));
                sut.AllInnerMessages(string.Empty);

                Assert.True(false, "Expected ArgumentException");            
            }
            catch(Exception ex)
            {
                Assert.IsAssignableFrom(typeof(ArgumentException), ex);
                Assert.Equal("The seperator cannot be null or empty\nParameter name: seperator", ex.Message);
            }
        }
    }
}
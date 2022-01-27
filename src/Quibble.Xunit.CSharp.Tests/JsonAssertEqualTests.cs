using Xunit;

namespace Quibble.Xunit.CSharp.Tests
{
    public class JsonAssertEqualTests
    {
        [Fact]
        public void BooksExampleTest()
        {
            var expected =
                @"{
        ""books"": [{
            ""title"": ""Data and Reality"",
            ""author"": ""William Kent"" 
        }, {
            ""title"": ""Thinking Forth"",
            ""author"": ""Leo Brodie""
        }]
    }";

            var actual =
                @"{
        ""books"": [{
            ""title"": ""Data and Reality"",
            ""author"": ""William Kent"",
            ""edition"": ""2nd""
        }, {
            ""title"": ""Thinking Forth"",
            ""author"": ""Chuck Moore""
        }]
    }";
            var ex = Assert.Throws<JsonAssertException>(() => JsonAssert.Equal(expected, actual));
        }

        [Fact]
        public void BooksExampleTestWithExtraField()
        {
            var expected =
                @"{
        ""books"": [{
            ""title"": ""Data and Reality"",
            ""author"": ""William Kent"" 
        }, {
            ""title"": ""Thinking Forth"",
            ""author"": ""Leo Brodie""
        }]
    }";

            var actual =
                @"{
        ""books"": [{
            ""title"": ""Data and Reality"",
            ""author"": ""William Kent"",
            ""edition"": ""2nd""
        }, {
            ""title"": ""Thinking Forth"",
            ""author"": ""Leo Brodie""
        }]
    }";
            JsonAssert.EqualOverrideDefault(expected, actual, new JsonDiffConfig(true, ""));
        }

        [Fact]
        public void BooksExampleTestWithDifferentValue()
        {
            var expected =
                @"{
        ""books"": [{
            ""title"": ""Data and Reality"",
            ""author"": ""William Kent"" 
        }, {
            ""title"": ""Thinking Forth"",
            ""author"": ""Leo Brodie""
        }]
    }";

            var actual =
                @"{
        ""books"": [{
            ""title"": ""Data and Reality"",
            ""author"": ""William Kent"",
            ""edition"": ""2nd""
        }, {
            ""title"": ""Thinking Forth"",
            ""author"": ""Chuck Moore""
        }]
    }";
            var ex = Assert.Throws<JsonAssertException>(() => JsonAssert.Equal(expected, actual));
            JsonAssert.EqualOverrideDefault(expected, actual, new JsonDiffConfig(true, "$.books[1].author"));
        }
    }
}
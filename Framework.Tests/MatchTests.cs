using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Framework.Tests
{
    public class MatchTests
    {
        [Fact]
        public void MatchStatementMatchesRuntimeType()
        {
            Base value = new Derived2();
            var matched = false;
            Match.On(value).With(m => m
                .Is<Derived1>(_ => Assert.True(false, "Matched Derived1"))
                .Is<Derived2>(s =>
                {
                    matched = true;
                    Derived2 d = s; // Prove that `s` must be of type Derived2
                    Assert.Equal(value, s);
                })
                .Is<Derived3>(_ => Assert.True(false, "Matched Derived3")));
            Assert.True(matched, "Did not match at all");
        }

        [Fact]
        public void MatchExpressionMatchesRuntimeType()
        {
            Base value = new Derived2();
            var result = MatchInto<int>.On(value).With(m => m
                .Is<Derived1>(_ => 1)
                .Is<Derived2>(s =>
                {
                    Derived2 d = s; // Prove that `s` must be of type Derived2
                    Assert.Equal(value, s);
                    return 2;
                })
                .Is<Derived3>(_ => 3));
            Assert.Equal(2, result);
        }

        private class Base { }
        private class Derived1 : Base { }
        private class Derived2 : Base { }
        private class Derived3 : Base { }
    }
}

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit
{
    namespace OuterNamespace
    {
        public class TestClass { }
        public class TestClass<T> { }

        namespace InnerNamespace
        {
            public class TestClass
            {
            }

            public static class Tester
            {
                public static void TestMethod()
                {
                    InnerNamespace.TestClass t1 = new TestClass();
                    OuterNamespace.TestClass<int> t2 = new TestClass<int>();
                }

            }
        }
    }
}

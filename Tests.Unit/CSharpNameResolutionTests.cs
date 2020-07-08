namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit
{
    namespace OuterNamespace
    {
        public class TestClass1 { }
        public class TestClass1<T> { }

        namespace InnerNamespace
        {
            public class TestClass1
            {
            }

            public static class Tester
            {
                public static void TestMethod()
                {
                    InnerNamespace.TestClass1 t1 = new TestClass1();
                    OuterNamespace.TestClass1<int> t2 = new TestClass1<int>();
                }
            }
        }
    }
}

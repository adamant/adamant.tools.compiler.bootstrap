using System.Diagnostics.CodeAnalysis;

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
                [SuppressMessage("Style", "IDE0059:Unnecessary assignment of a value", Justification = "<Pending>")]
                public static void TestMethod()
                {
                    TestClass1 t1 = new TestClass1();
                    TestClass1<int> t2 = new TestClass1<int>();
                }
            }
        }
    }
}

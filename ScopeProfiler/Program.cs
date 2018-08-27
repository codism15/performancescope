using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ScopeProfiler
{
    class Program
    {
        static void Main(string[] args)
        {
            PerformanceScope rootScope = null;
            using (rootScope = new PerformanceScope(nameof(Main)))
            {
                Console.WriteLine("Hello World!");
                for (int i = 0; i < 10; i++)
                {
                    TestMethod1();
                }

                for (int i = 0; i < 50; i++)
                {
                    TestMethod2();
                    Thread.Sleep(10);
                }
            }

            var outs = new StringWriter();
            rootScope.Report(outs);

            Debug.WriteLine(outs.ToString());
        }

        static void TestMethod1()
        {
            using (new PerformanceScope(nameof(TestMethod1)))
            {
                Thread.Sleep(100);
                TestMethod2();
            }
        }

        static void TestMethod2()
        {
            using (new PerformanceScope("Method 2"))
            {
                Thread.Sleep(50);
            }
        }
    }
}

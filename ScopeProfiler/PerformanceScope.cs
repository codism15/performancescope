using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ScopeProfiler
{
    public class PerformanceScope
        : IDisposable
    {
        static Stack<PerformanceScope> stack = new Stack<PerformanceScope>();

        PerformanceScope instance;

        public string Name { get; set; }

        public int CallCount { get; set; }

        public Stopwatch Stopwatch { get; set; }

        public List<PerformanceScope> Children { get; set; }

        public PerformanceScope(string name)
        {
            if (stack.Count == 0)
            {
                Name = name;
                Stopwatch = new Stopwatch();
                instance = this;
            }
            else
            {
                var currentCounter = stack.Peek();
                if (currentCounter.Children == null)
                {
                    currentCounter.Children = new List<PerformanceScope>();
                }
                instance = currentCounter.Children.FirstOrDefault(c => c.Name == name);
                if (instance == null)
                {
                    Name = name;
                    Stopwatch = new Stopwatch();
                    instance = this;
                    currentCounter.Children.Add(instance);
                }
            }
            stack.Push(instance);
            instance.CallCount++;
            instance.Stopwatch.Start();
        }

        public void Dispose()
        {
            instance.Stopwatch.Stop();
            stack.Pop();            
        }

        public void Report(TextWriter outs)
        {
            Report(outs, instance);
        }

        private void Report(TextWriter outs, PerformanceScope node, string indent = "")
        {
            outs.WriteLine($"{indent}{node.Name}:");
            outs.WriteLine($"{indent}  CallCount: {node.CallCount}");
            outs.WriteLine($"{indent}  TotalSeconds: {node.Stopwatch.Elapsed.TotalSeconds:0.0}");
            if (node.Children != null)
            {
                if (node.Children.Count > 1)
                {
                    double totalSeconds = node.Children.Aggregate(0D, (s, ps) => s + ps.Stopwatch.Elapsed.TotalSeconds);
                    outs.WriteLine($"{indent}  ChildTotalSeconds: {totalSeconds:0.0}");
                }
                var childIndent = indent + "  ";
                foreach (var child in node.Children)
                {
                    Report(outs, child, childIndent);
                }
            }
        }
    }

}

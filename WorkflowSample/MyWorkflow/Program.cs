using System;
using System.Activities;
using System.Activities.Statements;
using System.Dynamic;
using System.Linq;

namespace MyWorkflow
{
    class Program
    {
        static void Main(string[] args)
        {
            Activity workflow1 = new Activity1();
            WorkflowInvoker.Invoke(workflow1);
            Console.WriteLine("enter to end..");
            Console.ReadLine();

            
        }
    }

    public class A
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public B b { get; set; }
    }

    public class B
    {
        public int Count { get; set; }
    }
}

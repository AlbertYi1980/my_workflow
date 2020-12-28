using Microsoft.CSharp.Activities;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Linq.Expressions;

namespace DynamicFlow
{
    public class ActivityGenerator
    {
        public Activity Generate()
        {
            //var value = new CSharpValue<string>("\"hello\"");
            //var tree = value.GetExpressionTree();
            //var expression = (Expression<Func<ActivityContext, string>>)tree;            
            return new Sequence
            {
                Activities =
                {
                    new WriteLine
                    {
                        Text = new InArgument<string>(new CSharpValue<string>(@"new Xxx() { Yyy = ""sonsgon"" }.Yyy"))
                    }
                }

            };
        }
    }
}

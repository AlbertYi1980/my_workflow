using System;
using System.Activities;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using IronPython.Modules;
using Xunit;
using Xunit.Abstractions;

namespace Mirror.Workflows.Tests
{
    public class MirrorFilterExpressionTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public MirrorFilterExpressionTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void BuildCSharpExpression()
        {
            var regex = new Regex("\\$\\{\\s*(?<vn>[^\\{\\}\\$]+)\\s*\\}", RegexOptions.Compiled);

            var expression = "${kk}bbbb${abb}bbbbbbbbb${ccc}ddd${xxx}d";

            var strings = regex.Split(input: expression);
            var sb = new StringBuilder();
            _testOutputHelper.WriteLine("-------------");
            for (var index = 0; index < strings.Length; index++)
            {
                var s = strings[index];
                if (index % 2 == 0)
                {
                    sb.Append(JsonSerializer.Serialize(s));
                }
                else
                {
                    sb.AppendFormat(" + JsonSerializer.Serialize({0}) + ", s);
                }
            }

            _testOutputHelper.WriteLine(sb.ToString());
        }
    }
}
using System.Activities;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Mirror.Workflows.CustomActivities.MirrorCrud
{
    public static class MirrorExpressionHelper
    {
        private static readonly Regex Regex = new Regex("\\$\\{\\s*(?<vn>[^\\{\\}\\$]+)\\s*\\}", RegexOptions.Compiled);

        public static  string BuildCSharpExpression( string expression)
        {
           
            var strings = Regex.Split(input: expression);
            var sb = new StringBuilder();
            for (var index = 0; index < strings.Length; index++)
            {
                var s = strings[index];
                if (index % 2 == 0)
                {
                    sb.Append(JsonSerializer.Serialize(s));
                }
                else
                {
                    sb.AppendFormat(" + JsonSerializer.Serialize({0}, null) + ", s);
                }
            }

            return sb.ToString();
        }
    }
}
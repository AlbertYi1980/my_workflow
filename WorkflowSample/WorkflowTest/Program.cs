using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.IO;
using WorkflowCore;

namespace WorkflowTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var convertor = new Json2Xaml();
            var json = "{\"$type\":\"sequence\",\"variables\":[{\"type\":\"int\",\"name\":\"a\",\"default\":\"0\"}],\"activities\":[{\"$type\":\"writeLine\",\"text\":\"a.ToString()\"},{\"$type\":\"assign\",\"assignType\":\"int\",\"to\":\"a\",\"value\":\"1\"},{\"$type\":\"writeLine\",\"text\":\"a.ToString()\"}]}";
            var xaml = convertor.Convert("aaFirst", json);
            Console.WriteLine(xaml);
            var settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true,


            };
            var activity = ActivityXamlServices.Load(new StringReader(xaml), settings);


            var a = new WorkflowApplication(activity);
            a.Run();

            Console.ReadLine();
        }
    }
}

using Newtonsoft.Json.Linq;
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
            var json = "{\"$type\":\"sequence\",\"variables\":[{\"type\":\"JObject\",\"name\":\"a\"}],\"activities\":[{\"$type\":\"mirrorCreate\",\"mirrorBase\":\"\\\"http://localhost:5000/\\\"\",\"tenantId\":\"\\\"a\\\"\",\"modelKey\":\"\\\"kkk\\\"\",\"args\":\"JToken.Parse(\\\"{\\\\\\\"model\\\\\\\":{\\\\\\\"data\\\\\\\":{\\\\\\\"a\\\\\\\":1,\\\\\\\"b\\\\\\\":\\\\\\\"hello\\\\\\\"}}}\\\",null)\",\"result\":\"a\"},{\"$type\":\"writeLine\",\"text\":\"a.ToString()\"}]}";
          
            var xaml = convertor.Convert("aaFirst", json);
            Console.WriteLine(xaml);
            var settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true,


            };
            try
            {  
                var activity = ActivityXamlServices.Load(new StringReader(xaml), settings);
                var a = new WorkflowApplication(activity);
                a.Run();

            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            
            }
           

            Console.ReadLine();
        }
    }
}

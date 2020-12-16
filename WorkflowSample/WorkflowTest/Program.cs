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
            var json = LoadJsonFromFile("temp.json");
          
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

        private static string LoadJsonFromFile(string fileName)
        {
            var dir = @"C:\Users\yicheng\Downloads\OZtree-master\OZprivate\Trees";
            var path = Path.Combine(dir, fileName);
            return File.ReadAllText(path);
        }
    }
}

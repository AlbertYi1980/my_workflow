using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Mirror.Workflows.Activities;
using Mirror.Workflows.Activities.Parsers;
using Mirror.Workflows.Activities.Parsers.Descriptors;
using Mirror.Workflows.CustomActivities.Descriptors;
using Mirror.Workflows.InstanceStoring;
using Mirror.Workflows.Repositories;
using Mirror.Workflows.Tests.Common;
using Mirror.Workflows.Types;
using Xunit;
using Xunit.Abstractions;

namespace Mirror.Workflows.Tests
{
    public class ActivityExecutingTest
    {
        public ActivityExecutingTest(ITestOutputHelper outputHelper)
        {
            System.Diagnostics.Trace.Listeners.Add(new TestTraceTraceListener(outputHelper));
        }

        [Fact]
        public void RunIf()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("if");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }

        [Fact]
        public void RunSequence()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("sequence");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }
        
        
        
        [Fact]
        public void RunCSharp()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var descriptorContainer = new DescriptorContainer(new WellKnownDescriptorContainer());
            descriptorContainer.Add(new CSharpDescriptor());
            var customActivityParser = new CustomActivityParser(descriptorContainer, wellKnownTypeContainer);
            var definition = LoadDefinition("csharp");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }

        [Fact]
        public void RunSwitch()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("switch");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }

        [Fact]
        public void RunForeach()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("foreach");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }

        [Fact]
        public void RunStateMachine()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("stateMachine");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }

        [Fact]
        public void RunAssign()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("assign");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }

        [Fact]
        public void RunCustom()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("custom");

            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);

            var executor = new ActivityExecutor(null);
            executor.Run(activity, new Dictionary<string, object>() {{"content1", "we win"}});
        }


        [Fact]
        public void RunTrace()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("trace");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }   
        
        
        [Fact]
        public void RunArguments()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("arguments");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            var appId = executor.Run(activity, new Dictionary<string, object>{ {"a","kknd"}});

        }    

        
        
        [Fact]
        public void RunVariables()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("variables");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            var appId = executor.Run(activity);

        }    


        
        
        [Fact]
        public void RunTemp()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("temp");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var store = new DefaultInstanceStore(new StrongTypeJsonFileRepository("e:\\workflow-instances"));
            var executor = new ActivityExecutor(null);
            var appId = executor.Run(activity, new Dictionary<string, object>{ {"v","kknd"}});

            // var resumeSuccess = executor.Resume(activity, appId, "input", "aaaaa");
        }    


        [Fact]
        public void  RunUserTask()
        {
            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var customActivityParser =
                new CustomActivityParser(new WellKnownDescriptorContainer(), wellKnownTypeContainer);
            var definition = LoadDefinition("userTask");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(wellKnownTypeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var store = new DefaultInstanceStore(new StrongTypeJsonFileRepository("e:\\workflow-instances"));
            // var store = new FileInstanceStore("e:\\workflow-instances");
            var executor = new ActivityExecutor(null);
            var appId = executor.Run(activity);
            
            var resumeSuccess = executor.Resume(activity, appId, "input", "aaaaa");
           
        }

        [Fact]
        public void RunDynamic()
        {
            var code = @"
    using System;
namespace MyModel
{
    public class Xxx
    {
        public string Yyy { get; set; }
    }
}
";
            var assemblyName = "t_123_v_1.1";

            var wellKnownTypeContainer = new WellKnownTypeContainer();
            var typeContainer = new TypeContainer(wellKnownTypeContainer);
            var assembly = DynamicModelLoader.Load(assemblyName, code);
            foreach (var type in assembly.GetTypes())
            {
                typeContainer.Add(type);
            }

            var customActivityParser = new CustomActivityParser(new WellKnownDescriptorContainer(), typeContainer);
            var definition = LoadDefinition("dynamic");
            var activity = customActivityParser.Parse(JsonSerializer.Deserialize<JsonElement>(definition));
            var compiler = new ActivityCompiler(typeContainer);
            compiler.Compile("nnn", "mmmm", activity);
            var executor = new ActivityExecutor(null);
            executor.Run(activity);
        }


        private string LoadDefinition(string name)
        {
            var dir = @"C:\Users\yicheng\source\repos\my_workflow\Mirror.Workflows\Mirror.Workflows.Tests\cases";
            var path = Path.Combine(dir, $"{name}.json");
            return File.ReadAllText(path);
        }
    }
}
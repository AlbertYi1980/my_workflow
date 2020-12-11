using System;
using System.Activities;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xaml;
using Microsoft.CSharp.Activities;
using WorkflowCore.Activities;
using System.Reflection;
using System.Activities.Expressions;

namespace WorkflowCore
{
    public class Json2Xaml
    {
        public string Convert(string name, string json)
        {
       
            var refTypes = new[] { typeof(JsonElement), typeof(JsonSerializer), typeof(ReadOnlySpan<>), typeof(Enumerable) };

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
            var root = ParseActivity(jsonElement);

           
            var ab = new ActivityBuilder
            {
                Name = name,
                Implementation = root,
            };


            TextExpression.SetReferencesForImplementation(ab, refTypes.Select(t => new AssemblyReference() {Assembly = t.Assembly }).ToList());
            TextExpression.SetNamespacesForImplementation(ab, refTypes.Select(t => t.Namespace).ToList());
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            var context = new XamlSchemaContext();
           
            var xw = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(tw, context));
          
            XamlServices.Save(xw, ab);
            
            return sb.ToString();
        }

        public Activity ConvertToActivity(string name, string json)
        {

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);


            return ParseActivity(jsonElement);
          
          
        }

        private Activity ParseActivity(JsonElement jsonElement)
        {
         
            var type = jsonElement.GetProperty("$type").GetString();
            switch (type)
            {
                case "sequence":
                    return ParseSequence(jsonElement);
                case "if":
                        return ParseIf(jsonElement);
                case "while":
                    return ParseWhile(jsonElement);
                case "doWhile":
                    return ParseDoWhile(jsonElement);
                case "writeLine":
                    return ParseWriteLine(jsonElement);
                case "userTask":
                    return ParseUserTask(jsonElement);
                case "switch":
                    return ParseSwitch(jsonElement);
                case "foreach":
                    return ParseForeach(jsonElement);
                case "assign":
                    return ParseAssign(jsonElement);
                default:
                    throw new NotSupportedException($"type {type} not been supported.");
            }
        }

        private Activity ParseUserTask(JsonElement jsonElement)
        {
            var resultExists = jsonElement.TryGetProperty("result", out var resultElement); 
            var userTask = new UserTask() {
                DisplayName = GetDisplayName(jsonElement),
                Name = jsonElement.GetProperty("name").GetString(),       
            };
            if (resultExists)
            {
                 userTask.Result = new Variable<JsonElement>(resultElement.GetString());
            }
      
            return userTask;
        }

        private Activity ParseSequence(JsonElement jsonElement)
        {
            var activitiesExists = jsonElement.TryGetProperty("activities", out var activitiesElement);
            var variablesExists = jsonElement.TryGetProperty("variables", out var variablesElement);
            var sequence = new Sequence()
            {
                DisplayName = GetDisplayName(jsonElement),
            };
            if (activitiesExists)
            {
                var activities =activitiesElement.EnumerateArray().Select(ParseActivity);
                foreach (var activity in activities)
                {
                    sequence.Activities.Add(activity);
                }
            }
            foreach(var v in ParseVariables(jsonElement)){
                sequence.Variables.Add(v);
            }
            return sequence;
        }


        private IEnumerable<Variable> ParseVariables(JsonElement jsonElement)
        {
            var variablesExists = jsonElement.TryGetProperty("variables", out var variablesElement);
            if (!variablesExists) yield break;
            foreach (var variableElement in variablesElement.EnumerateArray())
            {
                var typeText = variableElement.GetProperty("type").GetString();
                var type = MapType(typeText);
                var coreMethod = typeof(Json2Xaml).GetMethod(nameof(ParseVariableCore), BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
                coreMethod = coreMethod.MakeGenericMethod(type);
                yield return   (Variable)coreMethod.Invoke(this, new object[] { variableElement });
            }   
        }

        private Variable ParseVariableCore<T>(JsonElement variableElement)
        {
            var name = variableElement.GetProperty("name").GetString();
  
            var defaultExists = variableElement.TryGetProperty("default", out var defaultElment);
            var variable = Variable.Create(name, typeof(T), VariableModifiers.None);
            if (defaultExists)
            {
                variable.Default = new CSharpValue<T>(defaultElment.GetString());
            };
            return variable;

        }


        private Activity ParseIf(JsonElement jsonElement)
        {
            var thenExists = jsonElement.TryGetProperty("then", out var thenElement);
            var elseExists  = jsonElement.TryGetProperty("else", out var esleElement);
          
            var @if = new If
            {
                DisplayName = GetDisplayName(jsonElement),
                Condition = new CSharpValue<bool>(jsonElement.GetProperty("condition").GetString()),    
            };

            if (thenExists)
            {
                @if.Then = ParseActivity(thenElement);
            }


            if (elseExists)
            {
                @if.Else = ParseActivity(esleElement);
            }

          
            return @if;
        }


        private Activity ParseWhile(JsonElement jsonElement)
        {
            var @while = new While()
            {
                DisplayName = GetDisplayName(jsonElement)
            };
            @while.Condition = new CSharpValue<bool>(jsonElement.GetProperty("condition").GetString());
            var bodyExists = jsonElement.TryGetProperty("body", out var bodyElement);
            if (bodyExists)
            {
                @while.Body = ParseActivity(bodyElement);
            }
            foreach (var v in ParseVariables(jsonElement))
            {
                @while.Variables.Add(v);
            }
            return @while;
        }

        private Activity ParseDoWhile(JsonElement jsonElement)
        {
            var @doWhile = new DoWhile()
            {
                DisplayName = GetDisplayName(jsonElement)
            };
            @doWhile.Condition = new CSharpValue<bool>(jsonElement.GetProperty("condition").GetString());
            var bodyExists = jsonElement.TryGetProperty("body", out var bodyElement);
            if (bodyExists)
            {
                @doWhile.Body = ParseActivity(bodyElement);
            }
            foreach (var v in ParseVariables(jsonElement))
            {
                doWhile.Variables.Add(v);
            }
            return @doWhile;
        }

        private Type MapType(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));
            switch (type)
            {
                case "int":
                    return typeof(int);
                case "string":
                    return typeof(string);
                default:
                    throw new NotSupportedException($"can not support type {type}");
            }
        }

         private Activity ParseSwitch(JsonElement jsonElement)
        {
            var typeArguments = jsonElement.GetProperty("switchType").GetString();
            var type = MapType(typeArguments);
            var coreMethod = typeof(Json2Xaml).GetMethod(nameof(ParseSwitchCore), BindingFlags.Default |BindingFlags.Instance | BindingFlags.NonPublic );
            coreMethod = coreMethod.MakeGenericMethod(type);
            return (Activity)coreMethod.Invoke(this, new object[] { jsonElement });
        }

        private T ParseCaseKey<T>(string key)
        {
            var type = typeof(T);
            if (type == typeof(int)) return (T) (object)int.Parse(key);
            if (type == typeof(string)) return (T)(object)(key);
            throw new NotSupportedException($"not support type {type.Name}");
        }

        private Activity ParseSwitchCore<T>(JsonElement jsonElement)
        {

            var expressionElement = jsonElement.GetProperty("expression");
            var defaultExists = jsonElement.TryGetProperty("default", out var defaultElment);
            var casesExist = jsonElement.TryGetProperty("cases", out var casesElment);
            var @switch = new Switch<T>()
            {
                DisplayName = GetDisplayName(jsonElement)
            };

            @switch.Expression = new CSharpValue<T>(expressionElement.GetString());
            if (defaultExists)
            {
                @switch.Default = ParseActivity(defaultElment);
            }
            if (casesExist)
            {
                foreach(var caseElement in casesElment.EnumerateArray())
                {
                    var key = caseElement.GetProperty("key").GetString();
                    var valueElement = caseElement.GetProperty("value");
                    @switch.Cases.Add(new KeyValuePair<T, Activity>(ParseCaseKey<T>(key), ParseActivity(valueElement) ));
                }
               ;
            }
            return @switch;
        }

        private Activity ParseForeach(JsonElement jsonElement)
        {

            var values = jsonElement.GetProperty("values").GetString();
            var valueName = jsonElement.GetProperty("valueName").GetString();
            var bodyExists = jsonElement.TryGetProperty("body", out var bodyElment);

            var @foreach = new ForEach<JsonElement>()
            {
                DisplayName = GetDisplayName(jsonElement)
            };

            @foreach.Values = new CSharpValue<IEnumerable<JsonElement>>(values);
            var activityAction = new ActivityAction<JsonElement>();
            activityAction.Argument = new DelegateInArgument<JsonElement>(valueName);
            if (bodyExists)
            {
                activityAction.Handler = ParseActivity(bodyElment);
            }
            @foreach.Body = activityAction;
            return @foreach;
        }

        private Activity ParseAssign(JsonElement jsonElement)
        {
            var typeArguments = jsonElement.GetProperty("assignType").GetString();
            var type = MapType(typeArguments);
            var coreMethod = typeof(Json2Xaml).GetMethod(nameof(ParseAssignCore), BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
            coreMethod = coreMethod.MakeGenericMethod(type);
            return (Activity)coreMethod.Invoke(this, new object[] { jsonElement });
        }

        private Activity ParseAssignCore<T>(JsonElement jsonElement)
        {
            var assign = new Assign<T>()
            {
                DisplayName = GetDisplayName(jsonElement)
            };
            var to = jsonElement.GetProperty("to").GetString();
            assign.To = new OutArgument<T>(new CSharpReference<T>(to));
            var value = jsonElement.GetProperty("value").GetString();
            assign.Value = new CSharpValue<T>(value);
            return assign;
        }

        private Activity ParseWriteLine(JsonElement jsonElement)
        {
            var writeLine = new WriteLine()
            {
                DisplayName = GetDisplayName(jsonElement)
            };
            writeLine.Text = new CSharpValue<string>(jsonElement.GetProperty("text").GetString());
            return writeLine;
        }

        private string GetDisplayName(JsonElement jsonElement)
        {
            var displayNameExists = jsonElement.TryGetProperty("displayName", out var displayNameElement);
            return  displayNameExists ? displayNameElement.GetString() :null ;
        }



    }
}

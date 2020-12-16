using System;
using System.Activities;
using System.Activities.Statements;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xaml;
using Microsoft.CSharp.Activities;
using WorkflowCore.Activities;
using System.Reflection;
using System.Activities.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WorkflowCore
{
    public class Json2Xaml
    {
        public string Convert(string name, string json)
        {
       
            var refTypes = new[] { 
          
                typeof(ReadOnlySpan<>), 
                typeof(Enumerable),
                typeof(JToken)
            };

            var jsonElement = JsonConvert.DeserializeObject<JObject>(json);
            var root = ParseActivity(jsonElement);

           
            var ab = new ActivityBuilder
            {
                Name = name,
                Implementation = root,
            };


            TextExpression.SetReferencesForImplementation(ab, refTypes.Select(t => new AssemblyReference() {Assembly = t.Assembly }).ToList());
            TextExpression.SetNamespacesForImplementation(ab, refTypes.Select(t => t.Namespace).Distinct().ToList());
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            var context = new XamlSchemaContext();
           
            var xw = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(tw, context));
          
            XamlServices.Save(xw, ab);
            
            return sb.ToString();
        }

        public Activity ConvertToActivity(string name, string json)
        {

            var jsonElement = JsonConvert.DeserializeObject<JObject>(json);


            return ParseActivity(jsonElement);
          
          
        }

        private Activity ParseActivity(JObject jsonElement)
        {
         
            var type = jsonElement["$type"].ToString();
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
                case "mirrorCreate":
                   return  ParseMirrorCreate(jsonElement);
                case "mirrorDelete":
                    return ParseMirrorDelete(jsonElement);
                case "mirrorUpdate":
                    return ParseMirrorUpdate(jsonElement);
                case "mirrorQuery":
                    return ParseMirrorQuery(jsonElement);
                case "mirrorDetail":
                    return ParseMirrorDetail(jsonElement);
                default:
                    throw new NotSupportedException($"type {type} not been supported.");
            }
        }

        private Activity ParseMirrorCreate(JObject jsonElement)
        {
            var crud = new MirrorCreate();
            crud.DisplayName = GetDisplayName(jsonElement);
            crud.MirrorBase = new CSharpValue<string>(jsonElement["mirrorBase"].ToString());
            crud.TenantId = new CSharpValue<string>(jsonElement["tenantId"].ToString());
            crud.ModelKey = new CSharpValue<string>(jsonElement["modelKey"].ToString());
            crud.Model = new CSharpValue<string>(jsonElement["model"].ToString());
            var resultElement = jsonElement["result"]?.ToString();
            if (resultElement != null)
            {
                crud.Result = new OutArgument<string>(new CSharpReference<string>(resultElement));
            }
            return crud;
        }

        private Activity ParseMirrorDelete(JObject jsonElement)
        {
            var crud = new MirrorDelete();
            crud.DisplayName = GetDisplayName(jsonElement);
            crud.MirrorBase = new CSharpValue<string>(jsonElement["mirrorBase"].ToString());
            crud.TenantId = new CSharpValue<string>(jsonElement["tenantId"].ToString());
            crud.ModelKey = new CSharpValue<string>(jsonElement["modelKey"].ToString());
            crud.Filter = new CSharpValue<string>(jsonElement["filter"].ToString());
            var resultElement = jsonElement["result"]?.ToString();
            if (resultElement != null)
            {
                crud.Result = new OutArgument<bool>(new CSharpReference<bool>(resultElement));
            }
            return crud;
        }

        private Activity ParseMirrorUpdate(JObject jsonElement)
        {
            var crud = new MirrorUpdate();
            crud.DisplayName = GetDisplayName(jsonElement);
            crud.MirrorBase = new CSharpValue<string>(jsonElement["mirrorBase"].ToString());
            crud.TenantId = new CSharpValue<string>(jsonElement["tenantId"].ToString());
            crud.ModelKey = new CSharpValue<string>(jsonElement["modelKey"].ToString());
            crud.Filter = new CSharpValue<string>(jsonElement["filter"].ToString());
            crud.Model = new CSharpValue<string>(jsonElement["model"].ToString());
            var resultElement = jsonElement["result"]?.ToString();
            if (resultElement != null)
            {
                crud.Result = new OutArgument<bool>(new CSharpReference<bool>(resultElement));
            }
            return crud;
        }

        private Activity ParseMirrorQuery(JObject jsonElement)
        {
            var crud = new MirrorQuery();
            crud.DisplayName = GetDisplayName(jsonElement);
            crud.MirrorBase = new CSharpValue<string>(jsonElement["mirrorBase"].ToString());
            crud.TenantId = new CSharpValue<string>(jsonElement["tenantId"].ToString());
            crud.ModelKey = new CSharpValue<string>(jsonElement["modelKey"].ToString());
            crud.Filter = new CSharpValue<string>(jsonElement["filter"].ToString());
            crud.Sort = new CSharpValue<string>(jsonElement["sort"].ToString());
            var resultElement = jsonElement["result"]?.ToString();
            if (resultElement != null)
            {
                crud.Result = new OutArgument<string>(new CSharpReference<string>(resultElement));
            }
            return crud;
        }

        private Activity ParseMirrorDetail(JObject jsonElement)
        {
            var crud = new MirrorDetail();
            crud.DisplayName = GetDisplayName(jsonElement);
            crud.MirrorBase = new CSharpValue<string>(jsonElement["mirrorBase"].ToString());
            crud.TenantId = new CSharpValue<string>(jsonElement["tenantId"].ToString());
            crud.ModelKey = new CSharpValue<string>(jsonElement["modelKey"].ToString());
            crud.Filter = new CSharpValue<string>(jsonElement["filter"].ToString());
            var resultElement = jsonElement["result"]?.ToString();
            if (resultElement != null)
            {
                crud.Result = new OutArgument<string>(new CSharpReference<string>(resultElement));
            }
            return crud;
        }

    

        private Activity ParseUserTask(JObject jsonElement)
        {
            var resultElement = jsonElement["result"]?.ToString(); 
            var userTask = new UserTask() {
                DisplayName = GetDisplayName(jsonElement),
                Name = jsonElement["name"].ToString(),       
            };
            if (resultElement != null)
            {
                userTask.Result = new OutArgument<string>(new CSharpReference<string>(resultElement));
            }
      
            return userTask;
        }

        private Activity ParseSequence(JObject jsonElement)
        {
            var activitiesElement = (JArray)jsonElement["activities"];
            var sequence = new Sequence()
            {
                DisplayName = GetDisplayName(jsonElement),
            };
            if (activitiesElement != null)
            {
                var activities =activitiesElement.Select(a =>  ParseActivity((JObject)a));
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


        private IEnumerable<Variable> ParseVariables(JObject jsonElement)
        {
            var variables = (JArray)jsonElement["variables"];
            if (variables == null) yield break;
            foreach (JObject variableElement in variables)
            {
                var typeText = variableElement["type"].ToString();
                var type = MapType(typeText);
                var coreMethod = typeof(Json2Xaml).GetMethod(nameof(ParseVariableCore), BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
                coreMethod = coreMethod.MakeGenericMethod(type);
                yield return   (Variable)coreMethod.Invoke(this, new object[] { variableElement });
            }   
        }

        private Variable ParseVariableCore<T>(JObject variableElement)
        {
            var name = variableElement["name"].ToString();
  
            var defaultExpression = variableElement["default"]?.ToString();
            var variable = Variable.Create(name, typeof(T), VariableModifiers.None);
            if (defaultExpression != null)
            {
                variable.Default = new CSharpValue<T>(defaultExpression);
            };
            return variable;

        }


        private Activity ParseIf(JObject jsonElement)
        {
            var thenElement = (JObject)jsonElement["then"];
            var esleElement =(JObject) jsonElement["else"];
          
            var @if = new If
            {
                DisplayName = GetDisplayName(jsonElement),
                Condition = new CSharpValue<bool>(jsonElement["condition"].ToString()),    
            };

            if (thenElement != null)
            {
                @if.Then = ParseActivity(thenElement);
            }


            if (esleElement != null)
            {
                @if.Else = ParseActivity(esleElement);
            }

          
            return @if;
        }


        private Activity ParseWhile(JObject jsonElement)
        {
            var @while = new While()
            {
                DisplayName = GetDisplayName(jsonElement)
            };
            @while.Condition = new CSharpValue<bool>(jsonElement["condition"].ToString());
            var body = (JObject)jsonElement["body"];
            if (body != null)
            {
                @while.Body = ParseActivity(body);
            }
            foreach (var v in ParseVariables(jsonElement))
            {
                @while.Variables.Add(v);
            }
            return @while;
        }

        private Activity ParseDoWhile(JObject jsonElement)
        {
            var @doWhile = new DoWhile()
            {
                DisplayName = GetDisplayName(jsonElement)
            };
            @doWhile.Condition = new CSharpValue<bool>(jsonElement["condition"].ToString());
            var bodyElement = (JObject)jsonElement["body"];
            if (bodyElement != null)
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
                case "json":
                    return typeof(string);
            
                default:
                    throw new NotSupportedException($"can not support type {type}");
            }
        }

         private Activity ParseSwitch(JObject jsonElement)
        {
            var typeArguments = jsonElement["switchType"].ToString();
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

        private Activity ParseSwitchCore<T>(JObject jsonElement)
        {

            var expressionElement = jsonElement["expression"].ToString();
            var defaultElment = (JObject)jsonElement["default"];
            var casesElment = (JArray)jsonElement["cases"];
            var @switch = new Switch<T>()
            {
                DisplayName = GetDisplayName(jsonElement)
            };

            @switch.Expression = new CSharpValue<T>(expressionElement);
            if (defaultElment != null)
            {
                @switch.Default = ParseActivity(defaultElment);
            }
            if (casesElment !=  null)
            {
                foreach(JObject caseElement in casesElment)
                {
                    var key = caseElement["key"].ToString();
                    var valueElement = (JObject)caseElement["value"];
                    @switch.Cases.Add(new KeyValuePair<T, Activity>(ParseCaseKey<T>(key), ParseActivity(valueElement) ));
                }
               ;
            }
            return @switch;
        }

        private Activity ParseForeach(JObject jsonElement)
        {

            var values = jsonElement["values"].ToString();
            var valueName = jsonElement["valueName"].ToString();
            var body = (JObject)jsonElement["body"];

            var @foreach = new ForEach<JObject>()
            {
                DisplayName = GetDisplayName(jsonElement)
            };

            @foreach.Values = new CSharpValue<IEnumerable<JObject>>(values);
            var activityAction = new ActivityAction<JObject>();
            activityAction.Argument = new DelegateInArgument<JObject>(valueName);
            if (body  != null)
            {
                activityAction.Handler = ParseActivity(body);
            }
            @foreach.Body = activityAction;
            return @foreach;
        }

        private Activity ParseAssign(JObject jsonElement)
        {
            var typeArguments = jsonElement["assignType"].ToString();
            var type = MapType(typeArguments);
            var coreMethod = typeof(Json2Xaml).GetMethod(nameof(ParseAssignCore), BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
            coreMethod = coreMethod.MakeGenericMethod(type);
            return (Activity)coreMethod.Invoke(this, new object[] { jsonElement });
        }

        private Activity ParseAssignCore<T>(JObject jsonElement)
        {
            var assign = new Assign<T>()
            {
                DisplayName = GetDisplayName(jsonElement)
            };
            var to = jsonElement["to"].ToString();
            assign.To = new OutArgument<T>(new CSharpReference<T>(to));
            var value = jsonElement["value"].ToString();
            assign.Value = new CSharpValue<T>(value);
            return assign;
        }

        private Activity ParseWriteLine(JObject jsonElement)
        {
            var writeLine = new WriteLine()
            {
                DisplayName = GetDisplayName(jsonElement)
            };
            writeLine.Text = new CSharpValue<string>(jsonElement["text"].ToString());
            return writeLine;
        }

        private string GetDisplayName(JObject jsonElement)
        {
            return jsonElement["displayName"]?.ToString();
        }



    }
}

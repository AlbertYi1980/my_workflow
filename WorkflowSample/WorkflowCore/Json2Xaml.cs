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


            TextExpression.SetReferencesForImplementation(ab, refTypes.Select(t => new AssemblyReference() { Assembly = t.Assembly }).ToList());
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

            var node = JsonConvert.DeserializeObject<JObject>(json);


            return ParseActivity(node);


        }

        private Activity ParseActivity(JObject node)
        {

            var type = node["$type"].ToString();
            switch (type)
            {
                case "sequence":
                    return ParseSequence(node);
                case "if":
                    return ParseIf(node);
                case "while":
                    return ParseWhile(node);
                case "doWhile":
                    return ParseDoWhile(node);
                case "writeLine":
                    return ParseWriteLine(node);
                case "userTask":
                    return ParseUserTask(node);
                case "switch":
                    return ParseSwitch(node);
                case "foreach":
                    return ParseForeach(node);
                case "assign":
                    return ParseAssign(node);
                case "mirrorCreate":
                    return ParseMirrorCreate(node);
                case "mirrorDelete":
                    return ParseMirrorDelete(node);
                case "mirrorUpdate":
                    return ParseMirrorUpdate(node);
                case "mirrorQuery":
                    return ParseMirrorQuery(node);
                case "mirrorDetail":
                    return ParseMirrorDetail(node);
                case "stateMachine":
                    return ParseMachineState(node);
                default:
                    throw new NotSupportedException($"type {type} not been supported.");
            }
        }

        private Activity ParseMirrorCreate(JObject node)
        {
            var create = new MirrorCreate();
            create.DisplayName = GetDisplayName(node);
            create.MirrorBase = new CSharpValue<string>(node["mirrorBase"].ToString());
            create.TenantId = new CSharpValue<string>(node["tenantId"].ToString());
            create.ModelKey = new CSharpValue<string>(node["modelKey"].ToString());
            create.Model = new CSharpValue<string>(node["model"].ToString());
            var resultNode = node["result"]?.ToString();
            if (resultNode != null)
            {
                create.Result = new OutArgument<string>(new CSharpReference<string>(resultNode));
            }
            return create;
        }

        private Activity ParseMirrorDelete(JObject node)
        {
            var delete = new MirrorDelete();
            delete.DisplayName = GetDisplayName(node);
            delete.MirrorBase = new CSharpValue<string>(node["mirrorBase"].ToString());
            delete.TenantId = new CSharpValue<string>(node["tenantId"].ToString());
            delete.ModelKey = new CSharpValue<string>(node["modelKey"].ToString());
            delete.Filter = new CSharpValue<string>(node["filter"].ToString());
            var resultNode = node["result"]?.ToString();
            if (resultNode != null)
            {
                delete.Result = new OutArgument<bool>(new CSharpReference<bool>(resultNode));
            }
            return delete;
        }

        private Activity ParseMirrorUpdate(JObject node)
        {
            var update = new MirrorUpdate();
            update.DisplayName = GetDisplayName(node);
            update.MirrorBase = new CSharpValue<string>(node["mirrorBase"].ToString());
            update.TenantId = new CSharpValue<string>(node["tenantId"].ToString());
            update.ModelKey = new CSharpValue<string>(node["modelKey"].ToString());
            update.Filter = new CSharpValue<string>(node["filter"].ToString());
            update.Model = new CSharpValue<string>(node["model"].ToString());
            var resultNode = node["result"]?.ToString();
            if (resultNode != null)
            {
                update.Result = new OutArgument<bool>(new CSharpReference<bool>(resultNode));
            }
            return update;
        }

        private Activity ParseMirrorQuery(JObject node)
        {
            var query = new MirrorQuery();
            query.DisplayName = GetDisplayName(node);
            query.MirrorBase = new CSharpValue<string>(node["mirrorBase"].ToString());
            query.TenantId = new CSharpValue<string>(node["tenantId"].ToString());
            query.ModelKey = new CSharpValue<string>(node["modelKey"].ToString());
            query.Filter = new CSharpValue<string>(node["filter"].ToString());
            query.Sort = new CSharpValue<string>(node["sort"].ToString());
            var resultNode = node["result"]?.ToString();
            if (resultNode != null)
            {
                query.Result = new OutArgument<string>(new CSharpReference<string>(resultNode));
            }
            return query;
        }

        private Activity ParseMirrorDetail(JObject node)
        {
            var detail = new MirrorDetail();
            detail.DisplayName = GetDisplayName(node);
            detail.MirrorBase = new CSharpValue<string>(node["mirrorBase"].ToString());
            detail.TenantId = new CSharpValue<string>(node["tenantId"].ToString());
            detail.ModelKey = new CSharpValue<string>(node["modelKey"].ToString());
            detail.Filter = new CSharpValue<string>(node["filter"].ToString());
            var resultNode = node["result"]?.ToString();
            if (resultNode != null)
            {
                detail.Result = new OutArgument<string>(new CSharpReference<string>(resultNode));
            }
            return detail;
        }



        private Activity ParseUserTask(JObject node)
        {
            var result = node["result"]?.ToString();
            var userTask = new UserTask()
            {
                DisplayName = GetDisplayName(node),
                Name = node["name"].ToString(),
            };
            if (result != null)
            {
                userTask.Result = new OutArgument<string>(new CSharpReference<string>(result));
            }

            return userTask;
        }

        private Activity ParseSequence(JObject node)
        {
            var activitiesElement = (JArray)node["activities"];
            var sequence = new Sequence()
            {
                DisplayName = GetDisplayName(node),
            };
            if (activitiesElement != null)
            {
                var activities = activitiesElement.Select(a => ParseActivity((JObject)a));
                foreach (var activity in activities)
                {
                    sequence.Activities.Add(activity);
                }
            }
            foreach (var v in ParseVariables(node))
            {
                sequence.Variables.Add(v);
            }
            return sequence;
        }


        private IEnumerable<Variable> ParseVariables(JObject node)
        {
            var variables = (JArray)node["variables"];
            if (variables == null) yield break;
            foreach (JObject variableElement in variables)
            {
                var typeText = variableElement["type"].ToString();
                var type = MapType(typeText);
                var coreMethod = typeof(Json2Xaml).GetMethod(nameof(ParseVariableCore), BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
                coreMethod = coreMethod.MakeGenericMethod(type);
                yield return (Variable)coreMethod.Invoke(this, new object[] { variableElement });
            }
        }

        private Variable ParseVariableCore<T>(JObject variableNode)
        {
            var name = variableNode["name"].ToString();

            var defaultExpression = variableNode["default"]?.ToString();
            var variable = Variable.Create(name, typeof(T), VariableModifiers.None);
            if (defaultExpression != null)
            {
                variable.Default = new CSharpValue<T>(defaultExpression);
            };
            return variable;

        }


        private Activity ParseIf(JObject node)
        {
            var thenNode = (JObject)node["then"];
            var esleNode = (JObject)node["else"];

            var @if = new If
            {
                DisplayName = GetDisplayName(node),
                Condition = new CSharpValue<bool>(node["condition"].ToString()),
            };

            if (thenNode != null)
            {
                @if.Then = ParseActivity(thenNode);
            }


            if (esleNode != null)
            {
                @if.Else = ParseActivity(esleNode);
            }


            return @if;
        }


        private Activity ParseWhile(JObject node)
        {
            var @while = new While()
            {
                DisplayName = GetDisplayName(node)
            };
            @while.Condition = new CSharpValue<bool>(node["condition"].ToString());
            var body = node["body"]?.Value<JObject>();
            if (body != null)
            {
                @while.Body = ParseActivity(body);
            }
            foreach (var v in ParseVariables(node))
            {
                @while.Variables.Add(v);
            }
            return @while;
        }

        private Activity ParseDoWhile(JObject node)
        {
            var @doWhile = new DoWhile()
            {
                DisplayName = GetDisplayName(node)
            };
            @doWhile.Condition = new CSharpValue<bool>(node["condition"].ToString());
            var bodyElement = (JObject)node["body"];
            if (bodyElement != null)
            {
                @doWhile.Body = ParseActivity(bodyElement);
            }
            foreach (var v in ParseVariables(node))
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

        private Activity ParseSwitch(JObject node)
        {
            var typeArguments = node["switchType"].ToString();
            var type = MapType(typeArguments);
            var coreMethod = typeof(Json2Xaml).GetMethod(nameof(ParseSwitchCore), BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
            coreMethod = coreMethod.MakeGenericMethod(type);
            return (Activity)coreMethod.Invoke(this, new object[] { node });
        }

        private T ParseCaseKey<T>(string key)
        {
            var type = typeof(T);
            if (type == typeof(int)) return (T)(object)int.Parse(key);
            if (type == typeof(string)) return (T)(object)(key);
            throw new NotSupportedException($"not support type {type.Name}");
        }

        private Activity ParseSwitchCore<T>(JObject node)
        {

            var expressionElement = node["expression"].ToString();
            var defaultNode = (JObject)node["default"];
            var casesElment = (JArray)node["cases"];
            var @switch = new Switch<T>()
            {
                DisplayName = GetDisplayName(node)
            };

            @switch.Expression = new CSharpValue<T>(expressionElement);
            if (defaultNode != null)
            {
                @switch.Default = ParseActivity(defaultNode);
            }
            if (casesElment != null)
            {
                foreach (JObject caseNode in casesElment)
                {
                    var key = caseNode["key"].ToString();
                    var valueNode = (JObject)caseNode["value"];
                    @switch.Cases.Add(new KeyValuePair<T, Activity>(ParseCaseKey<T>(key), ParseActivity(valueNode)));
                }
               ;
            }
            return @switch;
        }

        private Activity ParseForeach(JObject node)
        {

            var values = node["values"].ToString();
            var valueName = node["valueName"].ToString();
            var body = (JObject)node["body"];

            var @foreach = new ForEach<JObject>()
            {
                DisplayName = GetDisplayName(node)
            };

            @foreach.Values = new CSharpValue<IEnumerable<JObject>>(values);
            var activityAction = new ActivityAction<JObject>();
            activityAction.Argument = new DelegateInArgument<JObject>(valueName);
            if (body != null)
            {
                activityAction.Handler = ParseActivity(body);
            }
            @foreach.Body = activityAction;
            return @foreach;
        }

        private Activity ParseAssign(JObject node)
        {
            var typeArguments = node["assignType"].ToString();
            var type = MapType(typeArguments);
            var coreMethod = typeof(Json2Xaml).GetMethod(nameof(ParseAssignCore), BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
            coreMethod = coreMethod.MakeGenericMethod(type);
            return (Activity)coreMethod.Invoke(this, new object[] { node });
        }

        private Activity ParseAssignCore<T>(JObject node)
        {
            var assign = new Assign<T>()
            {
                DisplayName = GetDisplayName(node)
            };
            var to = node["to"].ToString();
            assign.To = new OutArgument<T>(new CSharpReference<T>(to));
            var value = node["value"].ToString();
            assign.Value = new CSharpValue<T>(value);
            return assign;
        }

        private Activity ParseWriteLine(JObject node)
        {
            var writeLine = new WriteLine()
            {
                DisplayName = GetDisplayName(node)
            };
            writeLine.Text = new CSharpValue<string>(node["text"].ToString());
            return writeLine;
        }

        private Activity ParseMachineState(JObject machineNode)
        {
            var machine = new StateMachine();
            var results = machineNode["states"].Value<JArray>().Select(s => ParseState((JObject)s)).ToDictionary(p => p.Id, p => p);
            var intialState = machineNode["initialState"].Value<string>();

            machine.InitialState = results[intialState].State;
            foreach (var variable in ParseVariables(machineNode))
            {
                machine.Variables.Add(variable);
            }
            machine.DisplayName = GetDisplayName(machineNode);
            foreach (var result in results.Values)
            {

                var id = result.Id;
                var state = result.State;
                if (result.TransitionsNode != null)
                {
                    foreach (JObject transitionNode in result.TransitionsNode)
                    {
                        state.Transitions.Add(ParseTransition(transitionNode, results));
                    }
                }
               
                machine.States.Add(state);
            }
            return machine;
        }

        private class StateParseResult
        {
            public string Id { get; set; }
            public State State { get; set; }
            public JArray TransitionsNode { get; set; }
        }

        private StateParseResult ParseState(JObject stateNode)
        {
            var id = stateNode["id"].Value<string>();
            var isFinal = stateNode["isFinal"]?.Value<bool?>() ?? false;
            var entryNode = stateNode["entry"]?.Value<JObject>();
            var exitNode = stateNode["exit"]?.Value<JObject>();
            var state = new State
            {
                DisplayName = GetDisplayName(stateNode),
                Entry = entryNode == null ? null : ParseActivity(entryNode),
                Exit = exitNode == null ? null : ParseActivity(exitNode),
                IsFinal = isFinal
            };
            foreach (var variable in ParseVariables(stateNode))
            {
                state.Variables.Add(variable);
            }

            return new StateParseResult
            {
                Id = id,
                State = state,
                TransitionsNode = stateNode["transitions"]?.Value<JArray>()
            };
        }

        private Transition ParseTransition(JObject transitionNode, Dictionary<string, StateParseResult> results)
        {
            var triggerNode = transitionNode["trigger"]?.Value<JObject>();
            var actionNode = transitionNode["action"]?.Value<JObject>();
            var condition = transitionNode["condition"]?.Value<string>();
            var to = transitionNode["to"]?.Value<string>();
            var transition = new Transition
            {
                DisplayName = GetDisplayName(transitionNode),
                Trigger = triggerNode == null ? null : ParseActivity(triggerNode),
                Action = actionNode == null ? null : ParseActivity(actionNode),
                Condition = condition == null ? null: new CSharpValue<bool>(condition),
                To = results[to].State
            };

            return transition;
        }


        private string GetDisplayName(JObject node)
        {
            return node["displayName"]?.ToString();
        }



    }
}

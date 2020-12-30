using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using Microsoft.CSharp.Activities;
using Mirror.Workflows.Activities.Special;

namespace Mirror.Workflows.Activities
{
    public class ActivityParser
    {
        private readonly Dictionary<string,Type> _types;
        private readonly Dictionary<string, Func<JsonElement, Activity>> _parsers;
        public ActivityParser(IEnumerable<ActivityDescriptor> descriptors, IEnumerable<Type> types)
        {
            _types = types.ToDictionary(t => t.Name, t => t);
            var primitiveParsers = new Dictionary<string, Func<JsonElement, Activity>>
            {
                {"sequence", ParseSequence},
                {"if", ParseIf},
                {"while", ParseWhile},
                {"doWhile", ParseDoWhile},
                {"writeLine", ParseWriteLine},
                {"trace", ParseTrace},
                {"userTask", ParseUserTask},
                {"switch", ParseSwitch},
                {"foreach", ParseForeach},
                {"assign", ParseAssign},
                {"pick", ParsePick},
                {"parallel", ParseParallel},
                {"stateMachine", ParseStateMachine},
            };
            _parsers = new Dictionary<string, Func<JsonElement, Activity>>(primitiveParsers);
            if (descriptors != null)
            {
                foreach (var descriptor in descriptors)
                {
                    var key = descriptor.Name;
                    var parser = descriptor.Parser;
                    if (_parsers.ContainsKey(key))
                    {
                        throw new Exception($"Parser {key} already exists.");
                    }

                    _parsers.Add(key, parser);
                }
            }
        }

        public Activity Parse(string definition)
        {
            return ParseActivity(JsonSerializer.Deserialize<JsonElement>(definition));
        }

        private Activity ParseActivity(JsonElement node)
        {
            var type = node.GetProperty("$type").GetString();
            if (!_parsers.ContainsKey(type!))
            {
                throw new Exception($"can not find parser for activity type {type}");
            }

            var parser = _parsers[type];
            return parser(node);
        }


        #region control flow

        private Activity ParseSequence(JsonElement node)
        {
            var activitiesNodeExists = node.TryGetProperty("activities", out var activitiesNode);

            var sequence = new Sequence
            {
                DisplayName = GetDisplayName(node),
            };
            if (activitiesNodeExists)
            {
                var activities = activitiesNode.EnumerateArray()
                    .Select(ParseActivity);
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


        private Activity ParseIf(JsonElement node)
        {
            var thenNodeExists = node.TryGetProperty("then", out var thenNode);
            var elseNodeExists = node.TryGetProperty("else", out var elseNode);

            var @if = new If
            {
                DisplayName = GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString()),
            };

            if (thenNodeExists)
            {
                @if.Then = ParseActivity(thenNode);
            }


            if (elseNodeExists)
            {
                @if.Else = ParseActivity(elseNode);
            }


            return @if;
        }

        private Activity ParseWhile(JsonElement node)
        {
            var @while = new While
            {
                DisplayName = GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString())
            };
            var bodyExists = node.TryGetProperty("body", out var bodyNode);
            if (bodyExists)
            {
                @while.Body = ParseActivity(bodyNode);
            }

            foreach (var v in ParseVariables(node))
            {
                @while.Variables.Add(v);
            }

            return @while;
        }

        private Activity ParseDoWhile(JsonElement node)
        {
            var doWhile = new DoWhile
            {
                DisplayName = GetDisplayName(node),
                Condition = new CSharpValue<bool>(node.GetProperty("condition").GetString())
            };
            var bodyExists = node.TryGetProperty("body", out var bodyNode);
            if (bodyExists)
            {
                doWhile.Body = ParseActivity(bodyNode);
            }

            foreach (var v in ParseVariables(node))
            {
                doWhile.Variables.Add(v);
            }

            return doWhile;
        }


        private Activity ParseSwitch(JsonElement node)
        {
            var typeArguments = node.GetProperty("switchType").GetString();
            var type = MapType(typeArguments);
            var coreMethod = typeof(ActivityParser).GetMethod(nameof(ParseSwitchCore),
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
            coreMethod = coreMethod!.MakeGenericMethod(type);
            return (Activity) coreMethod.Invoke(this, new object[] {node});
        }

        private T ParseCaseKey<T>(string key)
        {
            var type = typeof(T);
            if (type == typeof(int)) return (T) (object) int.Parse(key);
            if (type == typeof(string)) return (T) (object) (key);
            throw new NotSupportedException($"not support type {type.Name}");
        }

        private Activity ParseSwitchCore<T>(JsonElement node)
        {
            var defaultNodeExists = node.TryGetProperty("default", out var defaultNode);
            var casesNodeExists = node.TryGetProperty("cases", out var casesNode);
            var @switch = new Switch<T>
            {
                DisplayName = GetDisplayName(node),
                Expression = new CSharpValue<T>(node.GetProperty("expression").GetString())
            };

            if (defaultNodeExists)
            {
                @switch.Default = ParseActivity(defaultNode);
            }

            if (casesNodeExists)
            {
                foreach (JsonElement caseNode in casesNode.EnumerateArray())
                {
                    var key = caseNode.GetProperty("key").GetString();
                    var valueNode = caseNode.GetProperty("value");
                    @switch.Cases.Add(new KeyValuePair<T, Activity>(ParseCaseKey<T>(key), ParseActivity(valueNode)));
                }
            }

            return @switch;
        }

        private Activity ParseForeach(JsonElement node)
        {
            var values = node.GetProperty("values").GetString();
            var valueName = node.GetProperty("valueName").GetString();
            var bodyExists = node.TryGetProperty("body", out var bodyNode);

            var @foreach = new ForEach<JsonElement>
            {
                DisplayName = GetDisplayName(node), Values = new CSharpValue<IEnumerable<JsonElement>>(values)
            };

            var activityAction = new ActivityAction<JsonElement>
            {
                Argument = new DelegateInArgument<JsonElement>(valueName)
            };
            if (bodyExists)
            {
                activityAction.Handler = ParseActivity(bodyNode);
            }

            @foreach.Body = activityAction;
            return @foreach;
        }

        private Activity ParsePick(JsonElement node)
        {
            var pick = new Pick {DisplayName = GetDisplayName(node)};
            var branchesNodeExists = node.TryGetProperty("branches", out var branchesNode);
            if (branchesNodeExists)
            {
                foreach (var branchNode in branchesNode.EnumerateArray())
                {
                    pick.Branches.Add(ParsePickBranch(branchNode));
                }
            }

            return pick;
        }

        private PickBranch ParsePickBranch(JsonElement branchNode)
        {
            var displayName = GetDisplayName(branchNode);
            var triggerNode = branchNode.GetProperty("trigger");
            var actionNodeExists = branchNode.TryGetProperty("action", out var actionNode);
            var branch = new PickBranch
            {
                DisplayName = displayName,
                Trigger = ParseActivity(triggerNode),
                Action = actionNodeExists ? null : ParseActivity(actionNode),
            };
            foreach (var variable in ParseVariables(branchNode))
            {
                branch.Variables.Add(variable);
            }

            return branch;
        }

        private Activity ParseParallel(JsonElement parallelNode)
        {
            var displayName = GetDisplayName(parallelNode);
            var branchesNodeExists = parallelNode.TryGetProperty("branches", out var branchesNode);
            parallelNode.TryGetProperty("completionCondition", out var completionConditionProperty);
            var parallel = new Parallel()
            {
                DisplayName = displayName,
                CompletionCondition = new CSharpValue<bool>(completionConditionProperty.GetString()),
            };
            foreach (var variable in ParseVariables(parallelNode))
            {
                parallel.Variables.Add(variable);
            }

            if (branchesNodeExists)
            {
                foreach (var branchNode in branchesNode.EnumerateArray())
                {
                    parallel.Branches.Add(ParseActivity(branchNode));
                }
            }

            return parallel;
        }

        #endregion

        #region manul

        private Activity ParseUserTask(JsonElement node)
        {
            var resultExists = node.TryGetProperty("result", out var resultNode);
            var userTask = new UserTask()
            {
                DisplayName = GetDisplayName(node),
                Name = node.GetProperty("name").GetString(),
            };
            if (resultExists)
            {
                userTask.Result = new OutArgument<string>(new CSharpReference<string>(resultNode.GetString()));
            }

            return userTask;
        }

        #endregion

        #region basic

        private Activity ParseAssign(JsonElement node)
        {
            var typeArguments = node.GetProperty("assignType").GetString();
            var type = MapType(typeArguments);
            var coreMethod = typeof(ActivityParser).GetMethod(nameof(ParseAssignCore),
                BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
            coreMethod = coreMethod!.MakeGenericMethod(type);
            return (Activity) coreMethod.Invoke(this, new object[] {node});
        }

        private Activity ParseAssignCore<T>(JsonElement node)
        {
            var assign = new Assign<T>()
            {
                DisplayName = GetDisplayName(node)
            };
            var to = node.GetProperty("to").GetString();
            assign.To = new OutArgument<T>(new CSharpReference<T>(to));
            var value = node.GetProperty("value").GetString();
            assign.Value = new CSharpValue<T>(value);
            return assign;
        }

        private Activity ParseWriteLine(JsonElement node)
        {
            var writeLine = new WriteLine
            {
                DisplayName = GetDisplayName(node),
                Text = new CSharpValue<string>(node.GetProperty("text").GetString())
            };
            return writeLine;
        }
        
        private Activity ParseTrace(JsonElement node)
        {
            var trace = new Trace
            {
                DisplayName = GetDisplayName(node),
                Text = new CSharpValue<string>(node.GetProperty("text").GetString())
            };
            return trace;
        }

        #endregion

   


        #region state machine

        private Activity ParseStateMachine(JsonElement machineNode)
        {
            var machine = new StateMachine();
            var results =
                machineNode.GetProperty("states").EnumerateArray()
                    .Select(ParseState)
                    .ToDictionary(p => p.Id, p => p);
            var initialState = machineNode.GetProperty("initialState").GetString();

            machine.InitialState = results[initialState!].State;
            foreach (var variable in ParseVariables(machineNode))
            {
                machine.Variables.Add(variable);
            }

            machine.DisplayName = GetDisplayName(machineNode);
            foreach (var result in results.Values)
            {
                var state = result.State;
                if (result.TransitionsNode != null)
                {
                    foreach (JsonElement transitionNode in result.TransitionsNode.Value.EnumerateArray())
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
            public JsonElement? TransitionsNode { get; set; }
        }

        private StateParseResult ParseState(JsonElement stateNode)
        {
            var id = stateNode.GetProperty("id").GetString();
            var isFinal = !stateNode.TryGetProperty("isFinal", out var isFinalProperty) && isFinalProperty.GetBoolean();
            var entryNodeExists = stateNode.TryGetProperty("entry", out var entryNode);
            var exitNodeExists = stateNode.TryGetProperty("exit", out var exitNode);
            var transitionsNodeExists = stateNode.TryGetProperty("transitions", out var transitionsNode);
            var state = new State
            {
                DisplayName = GetDisplayName(stateNode),
                Entry = entryNodeExists ? null : ParseActivity(entryNode),
                Exit = exitNodeExists ? null : ParseActivity(exitNode),
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
                TransitionsNode = transitionsNodeExists ? transitionsNode : (JsonElement?) null
            };
        }

        private Transition ParseTransition(JsonElement transitionNode, Dictionary<string, StateParseResult> results)
        {
            transitionNode.TryGetProperty("trigger", out var triggerNode);
            transitionNode.TryGetProperty("action", out var actionNode);
            transitionNode.TryGetProperty("condition", out var conditionProperty);
            var to = transitionNode.GetProperty("to").GetString();
            var transition = new Transition
            {
                DisplayName = GetDisplayName(transitionNode),
                Trigger = ParseActivity(triggerNode),
                Action = ParseActivity(actionNode),
                Condition = new CSharpValue<bool>(conditionProperty.GetString()),
                To = results[to!].State
            };

            return transition;
        }

        #endregion


        #region utils

        private string GetDisplayName(JsonElement node)
        {
            var exists = node.TryGetProperty("displayName", out var displayNameProperty);
            if (!exists) return null;
            return displayNameProperty.GetString();
        }

        private IEnumerable<Variable> ParseVariables(JsonElement node)
        {
            var exists = node.TryGetProperty("variables", out var variablesNode);
            if (!exists) yield break;
            foreach (var variableNode in variablesNode.EnumerateArray())
            {
                var typeText = variableNode.GetProperty("type").GetString();
                var type = MapType(typeText);
                var coreMethod = typeof(ActivityParser).GetMethod(nameof(ParseVariableCore),
                    BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic);
                coreMethod = coreMethod!.MakeGenericMethod(type);
                yield return (Variable) coreMethod.Invoke(this, new object[] {variableNode});
            }
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
                    if (!_types.ContainsKey(type))
                    {
                        throw new NotSupportedException($"can not support type {type}");
                    }

                    return _types[type];
            }
        }

        private Variable ParseVariableCore<T>(JsonElement variableNode)
        {
            var name = variableNode.GetProperty("name").GetString();
            var defaultNodeExists = variableNode.TryGetProperty("default", out var defaultNode);
            var variable = Variable.Create(name, typeof(T), VariableModifiers.None);
            if (defaultNodeExists)
            {
                var defaultExpression = defaultNode.GetString();
                variable.Default = new CSharpValue<T>(defaultExpression);
            }

            return variable;
        }

        #endregion
    }
}
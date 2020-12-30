using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.CSharp.Activities;

namespace Mirror.Workflows.Activities.Parsers.Descriptors
{
    public class StateMachineDescriptor : IActivityDescriptor
    {
     
        public string Name => "stateMachine";

      
        public Activity Parse(JsonElement machineNode, ParseContext context)
        {
            var machine = new StateMachine();
            var results =
                machineNode.GetProperty("states").EnumerateArray()
                    .Select(s =>ParseState(s, context))
                    .ToDictionary(p => p.Id, p => p);
            var initialState = machineNode.GetProperty("initialState").GetString();

            machine.InitialState = results[initialState!].State;
            foreach (var variable in ParseHelper. ParseVariables(machineNode, context.TypeContainer))
            {
                machine.Variables.Add(variable);
            }

            machine.DisplayName = ParseHelper. GetDisplayName(machineNode);
            foreach (var result in results.Values)
            {
                var state = result.State;
                if (result.TransitionsNode != null)
                {
                    foreach (JsonElement transitionNode in result.TransitionsNode.Value.EnumerateArray())
                    {
                        state.Transitions.Add(ParseTransition(transitionNode, results, context));
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

        private StateParseResult ParseState(JsonElement stateNode, ParseContext context)
        {
            var id = stateNode.GetProperty("id").GetString();
            var isFinal = !stateNode.TryGetProperty("isFinal", out var isFinalProperty) && isFinalProperty.GetBoolean();
            var entryNodeExists = stateNode.TryGetProperty("entry", out var entryNode);
            var exitNodeExists = stateNode.TryGetProperty("exit", out var exitNode);
            var transitionsNodeExists = stateNode.TryGetProperty("transitions", out var transitionsNode);
            var state = new State
            {
                DisplayName = ParseHelper. GetDisplayName(stateNode),
                Entry = entryNodeExists ? null :context.CompositeParser. ParseActivity(entryNode),
                Exit = exitNodeExists ? null : context.CompositeParser.ParseActivity(exitNode),
                IsFinal = isFinal
            };
            foreach (var variable in ParseHelper. ParseVariables(stateNode, context.TypeContainer))
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

        private Transition ParseTransition(JsonElement transitionNode, Dictionary<string, StateParseResult> results, ParseContext context)
        {
            transitionNode.TryGetProperty("trigger", out var triggerNode);
            transitionNode.TryGetProperty("action", out var actionNode);
            transitionNode.TryGetProperty("condition", out var conditionProperty);
            var to = transitionNode.GetProperty("to").GetString();
            var transition = new Transition
            {
                DisplayName = ParseHelper.GetDisplayName(transitionNode),
                Trigger =context.CompositeParser. ParseActivity(triggerNode),
                Action = context.CompositeParser.ParseActivity(actionNode),
                Condition = new CSharpValue<bool>(conditionProperty.GetString()),
                To = results[to!].State
            };

            return transition;
        }
    }
}
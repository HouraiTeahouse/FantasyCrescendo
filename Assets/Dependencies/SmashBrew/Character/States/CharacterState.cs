using HouraiTeahouse.SmashBrew.States;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class CharacterState : State<CharacterStateContext> {

        public string Name { get; private set; }
        public CharacterStateData Data { get; private set; }
        public string AnimatorName { get; private set; }
        public int AnimatorHash { get; private set; }

        public CharacterState(string name,
                              CharacterStateData data) {
            Name = name;
            Data = Argument.NotNull(data);
            AnimatorName = Name.Replace(".", "-");
            AnimatorHash = Animator.StringToHash(AnimatorName);
        }

        public CharacterState AddTransitionTo(CharacterState state, 
                                              Func<CharacterStateContext, bool> extraCheck = null) {
            if (extraCheck != null)
                AddTransition(ctx => ctx.NormalizedAnimationTime >= 1.0f && extraCheck(ctx) ? state : null);
            else
                AddTransition(ctx => ctx.NormalizedAnimationTime >= 1.0f ? state : null);
            return this;
        }

        public override StateEntryPolicy GetEntryPolicy (CharacterStateContext context) {
            return Data.EntryPolicy;
        }

    }

    public static class CharacterStateExtensions {

        public static IEnumerable<CharacterState> AddTransitionTo(this IEnumerable<CharacterState> states,
                                                                State<CharacterStateContext> state) {
            Func<CharacterStateContext, State<CharacterStateContext>> transition =
                ctx => ctx.NormalizedAnimationTime >= 1.0f ? state : null;
            foreach (CharacterState characterState in states)
                characterState.AddTransition(transition);
            return states;
        }

        public static void Chain(this IEnumerable<CharacterState> states) {
            var enumerator = Argument.NotNull(states).GetEnumerator();
            CharacterState last = null;
            while (enumerator.MoveNext()) {
                if (enumerator.Current == null)
                    continue;
                if (last != null)
                    last.AddTransitionTo(enumerator.Current);
                last = enumerator.Current;
            }
        }

    }

}


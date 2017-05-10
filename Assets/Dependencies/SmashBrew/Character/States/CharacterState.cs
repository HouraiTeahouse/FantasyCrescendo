using System.Collections.Generic;
using HouraiTeahouse.SmashBrew.States;

namespace HouraiTeahouse.SmashBrew.Characters {

    public class CharacterState : State<CharacterStateContext> {

        public string Name { get; private set; }
        public CharacterStateData Data { get; private set; }

        public CharacterState(string name,
                              CharacterStateData data) {
            Name = name;
            Data = Argument.NotNull(data);
        }

        public CharacterState AddTransition(CharacterState state) {
            AddTransition(ctx => ctx.NormalizedAnimationTime > 1.0f ? state : null);
            return this;
        }

    }

    public static class CharacterStateExtensions {

        public static void Chain(this IEnumerable<CharacterState> states) {
            var enumerator = Argument.NotNull(states).GetEnumerator();
            CharacterState last = null;
            enumerator.MoveNext();
            while (enumerator.MoveNext()) {
                if (enumerator.Current == null)
                    continue;
                if (last != null)
                    last.AddTransition(enumerator.Current);
                last = enumerator.Current;
            }
        }

    }

}


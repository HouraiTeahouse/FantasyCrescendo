using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public abstract class StateEvent<T> {

        public readonly T State;
        protected StateEvent(T state) { State = state; }

    }

    public class StateInit<T> : StateEvent<T> {

        public StateInit(T state) : base(state) { }

    }

    public class StateEnter<T> : StateEvent<T> {

        public StateEnter(T state) : base(state) { }

    }

    public class StateExit<T> : StateEvent<T> {

        public StateExit(T state) : base(state) { }

    }

    public abstract class CharacterState<T> : BaseAnimationBehaviour<Character> where T : CharacterState<T> {

        /// <summary> Same as Target, renamed for clarity. </summary>
        public Character Character {
            get { return Target; }
        }

        public Mediator CharacterEvents {
            get { return Character != null ? Character.Events : null; }
        }

        public virtual void ProcessInput() { }

        public override void Initialize(GameObject gameObject) {
            base.Initialize(gameObject);
            Dispatch(new StateInit<T>(this as T));
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            Dispatch(new StateExit<T>(this as T));
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
            Dispatch(new StateExit<T>(this as T));
        }

        void Dispatch(object evnt) {
            if (CharacterEvents != null)
                CharacterEvents.Publish(evnt);
        }

    }

}
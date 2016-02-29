using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse {

    /// <summary>
    /// A MonoBehaviour that does only one action, which can be set to do so on a variety of triggers.
    /// </summary>
    public abstract class SingleActionBehaviour : MonoBehaviour, ISubmitHandler {

        private enum Type {
            // Do the action on instantiation 
            Awake = 0,
            // Do the action in response to a a Submit action
            UIEvent = 2,
            // Do the action on the first frame
            Start = 1,
            // Do the action once per frame
            Update = 3,
            // Do the action once per fixed timestep
            FixedUpdate = 4,
            // Do the action on physical collision with an object
            Collision = 5
        }

        [SerializeField]
        private Type _trigger;

        /// <summary>
        /// Unity callback. Called on object instantiation.
        /// </summary>
        protected virtual void Awake() {
            if (_trigger == Type.Awake)
                Action();
            enabled = _trigger >= Type.Start;
        }

        /// <summary>
        /// Unity callback. Called once before the object's first frame.
        /// </summary>
        protected virtual void Start() {
            if (_trigger == Type.Start)
                Action();
            enabled = _trigger > Type.Start;
        }

        /// <summary>
        /// Unity callback. Called on contact with another object with a trigger collider.
        /// </summary>
        protected virtual void OnTriggerEnter(Collider other) {
            if (_trigger == Type.Collision)
                Action();
        }

        /// <summary>
        /// Unity callback. Called once per frame.
        /// </summary>
        protected virtual void Update() {
            if (_trigger == Type.Update)
                Action();
        }

        /// <summary>
        /// Unity callback. Called repeatedly on a fixed timestep.
        /// </summary>
        protected virtual void FixedUpdate() {
            if (_trigger == Type.FixedUpdate)
                Action();
        }

        /// <summary>
        /// <see cref="ISubmitHandler.OnSubmit"/>
        /// </summary>
        public void OnSubmit(BaseEventData eventData) {
            if (_trigger == Type.UIEvent)
                Action();
        }

        /// <summary>
        /// The action to execute.
        /// </summary>
        protected abstract void Action();

    }
}

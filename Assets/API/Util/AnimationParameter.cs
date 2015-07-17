using System;
using UnityEngine;

namespace Genso.API {

    [Serializable]
    public abstract class BaseAnimationParameter {

        [SerializeField]
        private string _name;

        [SerializeField]
        private int _hash;

        private Animator _animator;

        /// <summary>
        /// The parameter name within the Animation Controller that identifies the parameter.
        /// Changing this value will also change the hash property.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _hash = Animator.StringToHash(_name);
            }
        }

        /// <summary>
        /// The corresponding hash value for the parameter.
        /// </summary>
        public int Hash
        {
            get { return _hash; }
        }

        /// <summary>
        /// The target Animator this parameter belongs to. This must be set before using the parameter.
        /// </summary>
        public Animator Animator
        {
            get { return _animator; }
            set { _animator = value; }
        }

        /// <summary>
        /// Creates a unbound Animation parameter.
        /// The Animator property must be set after this to use it.
        /// </summary>
        /// <param name="name">the parameter name</param>
        protected BaseAnimationParameter(string name) {
            _name = name;
        }

        /// <summary>
        /// Creates a bound Animation parameter.
        /// The Animator can be changed using the Animator property.
        /// </summary>
        /// <param name="animator">the Animator to bind to</param>
        /// <param name="name">the parameter name</param>
        protected BaseAnimationParameter(Animator animator, string name)
        {
            _animator = animator;
            Name = name;
        }

    }

    [Serializable]
    public abstract class AnimationParameter<T> : BaseAnimationParameter {

        /// <summary>
        /// Creates a unbound Animation parameter.
        /// The Animator property must be set after this to use it.
        /// </summary>
        /// <param name="name">the parameter name</param>
        protected AnimationParameter(string name) : base(name) { }

        /// <summary>
        /// Creates a bound Animation parameter.
        /// The Animator can be changed using the Animator property.
        /// </summary>
        /// <param name="animator">the Animator to bind to</param>
        /// <param name="name">the parameter name</param>
        protected AnimationParameter(Animator animator, string name) : base(animator, name) { }

        /// <summary>
        /// Gets the value of the Animation parameter from the Animator.
        /// </summary>
        /// <returns>the current value of the parameter</returns>
        public abstract T Get();

        /// <summary>
        /// Sets a new value of the Animation parameter to the Animator.
        /// </summary>
        /// <param name="value">the new value of the parameter</param>
        public abstract void Set(T value);

    }

    [Serializable]
    public sealed class AnimationBool : AnimationParameter<bool> {

        /// <summary>
        /// Creates a unbound Animation parameter.
        /// The Animator property must be set after this to use it.
        /// </summary>
        /// <param name="name">the parameter name</param>
        public AnimationBool(string name) : base(name) { }

        /// <summary>
        /// Creates a bound Animation parameter.
        /// The Animator can be changed using the Animator property.
        /// </summary>
        /// <param name="animator">the Animator to bind to</param>
        /// <param name="name">the parameter name</param>
        public AnimationBool(Animator animator, string name) : base(animator, name) { }

        /// <summary>
        /// Gets the value of the Animation parameter from the Animator.
        /// </summary>
        /// <returns>the current value of the parameter</returns>
        public override bool Get() {
            return Animator.GetBool(Hash);
        }

        /// <summary>
        /// Sets a new value of the Animation parameter to the Animator.
        /// </summary>
        /// <param name="value">the new value of the parameter</param>
        public override void Set(bool value) {
            Animator.SetBool(Hash, value);
        }

        public static implicit operator bool(AnimationBool value) {
            if (value == null)
                throw new NullReferenceException();
            return value.Get();
        }

    }

    [Serializable]
    public class AnimationFloat : AnimationParameter<float> {

        /// <summary>
        /// Creates a unbound Animation parameter.
        /// The Animator property must be set after this to use it.
        /// </summary>
        /// <param name="name">the parameter name</param>
        public AnimationFloat(string name) : base(name) { }

        /// <summary>
        /// Creates a bound Animation parameter.
        /// The Animator can be changed using the Animator property.
        /// </summary>
        /// <param name="animator">the Animator to bind to</param>
        /// <param name="name">the parameter name</param>
        public AnimationFloat(Animator animator, string name) : base(animator, name) {}

        /// <summary>
        /// Gets the value of the Animation parameter from the Animator.
        /// </summary>
        /// <returns>the current value of the parameter</returns>
        public override float Get() {
            return Animator.GetFloat(Hash);
        }

        /// <summary>
        /// Sets a new value of the Animation parameter to the Animator.
        /// </summary>
        /// <param name="value">the new value of the parameter</param>
        public override void Set(float value) {
            Animator.SetFloat(Hash, value);
        }

        public static implicit operator float(AnimationFloat value)
        {
            if (value == null)
                throw new NullReferenceException();
            return value.Get();
        }
    }

    [Serializable]
    public class AnimationInt : AnimationParameter<int> {

        /// <summary>
        /// Creates a unbound Animation parameter.
        /// The Animator property must be set after this to use it.
        /// </summary>
        /// <param name="name">the parameter name</param>
        public AnimationInt(string name) : base(name) { }

        /// <summary>
        /// Creates a bound Animation parameter.
        /// The Animator can be changed using the Animator property.
        /// </summary>
        /// <param name="animator">the Animator to bind to</param>
        /// <param name="name">the parameter name</param>
        public AnimationInt(Animator animator, string name) : base(animator, name) {}

        /// <summary>
        /// Gets the value of the Animation parameter from the Animator.
        /// </summary>
        /// <returns>the current value of the parameter</returns>
        public override int Get() {
            return Animator.GetInteger(Hash);
        }

        /// <summary>
        /// Sets a new value of the Animation parameter to the Animator.
        /// </summary>
        /// <param name="value">the new value of the parameter</param>
        public override void Set(int value) {
            Animator.SetInteger(Hash, value);
        }

        public static implicit operator int(AnimationInt value)
        {
            if (value == null)
                throw new NullReferenceException();
            return value.Get();
        }
    }

    [Serializable]
    public class AnimationTrigger : BaseAnimationParameter {

        /// <summary>
        /// Creates a unbound Animation parameter.
        /// The Animator property must be set after this to use it.
        /// </summary>
        /// <param name="name">the parameter name</param>
        public AnimationTrigger(string name) : base(name) { }

        /// <summary>
        /// Creates a bound Animation parameter.
        /// The Animator can be changed using the Animator property.
        /// </summary>
        /// <param name="animator">the Animator to bind to</param>
        /// <param name="name">the parameter name</param>
        public AnimationTrigger(Animator animator, string name) : base(animator, name) { }

        /// <summary>
        /// Sets the trigger to true to the Animator.
        /// </summary>
        /// <param name="value">the new value of the parameter</param>
        public void Set() {
            Animator.SetTrigger(Hash);
        }

    }

}

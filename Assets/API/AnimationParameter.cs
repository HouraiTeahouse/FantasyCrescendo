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

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _hash = Animator.StringToHash(_name);
            }
        }

        public int Hash
        {
            get { return _hash; }
        }

        public Animator Animator
        {
            get { return _animator; }
            set { _animator = value; }
        }

        protected BaseAnimationParameter(string name) {
            _name = name;
        }

        protected BaseAnimationParameter(Animator animator, string name) {
            Initialize(animator, name);
        }

        public void Initialize(Animator animator, string name) {
            _animator = animator;
            Name = name;
        }

    }

    [Serializable]
    public abstract class AnimationParameter<T> : BaseAnimationParameter {

        protected AnimationParameter(string name) : base(name) { }

        protected AnimationParameter(Animator animator, string name) : base(animator, name) { }

        public abstract T Get();

        public abstract void Set(T value);

    }

    [Serializable]
    public sealed class AnimationBool : AnimationParameter<bool> {

        public AnimationBool(string name) : base(name) { }

        public AnimationBool(Animator animator, string name) : base(animator, name) { }

        public override bool Get() {
            return Animator.GetBool(Hash);
        }

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
        
        public AnimationFloat(string name) : base(name) { }

        public AnimationFloat(Animator animator, string name) : base(animator, name) {}

        public override float Get() {
            return Animator.GetFloat(Hash);
        }

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
        
        public AnimationInt(string name) : base(name) { }

        public AnimationInt(Animator animator, string name) : base(animator, name) {}

        public override int Get() {
            return Animator.GetInteger(Hash);
        }

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
        
        public AnimationTrigger(string name) : base(name) { }

        public AnimationTrigger(Animator animator, string name) : base(animator, name) { }

        public void Set() {
            Animator.SetTrigger(Hash);
        }

    }

}

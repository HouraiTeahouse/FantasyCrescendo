using System;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public abstract class BaseAnimationBehaviour : StateMachineBehaviour {

        public abstract void Initialize(GameObject gameObject);

        public static void InitializeAll(Animator animator) {
            if (!animator)
                return;
            foreach (BaseAnimationBehaviour bab in animator.GetBehaviours<BaseAnimationBehaviour>())
                if (bab)
                    bab.Initialize(animator.gameObject);
        }

    }

    public abstract class BaseAnimationBehaviour<T> : BaseAnimationBehaviour where T : Component
    {
        protected T Target { get; private set; }

        public override void Initialize(GameObject gameObject) {
            Target = gameObject.GetComponent<T>();
            if (!Target)
                Debug.LogError("Expected a component of type" + typeof(T) + " but found none.");
        }

    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse {

    public static class MonoBehaviourExtensions {

        public static T CachedGetComponent<T>(this MonoBehaviour behaviour, T val, Func<T> defaultVal = null) where T : Component{
            Argument.NotNull(behaviour);
            if (val == null)
                return defaultVal != null ? defaultVal() : behaviour.SafeGetComponent<T>();
            return val;
        }

    }

}

using UnityEngine;

namespace HouraiTeahouse {

    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T> {

        /// <summary> The singleton instance of the type. </summary>
        public static T Instance { get; private set; }

        /// <summary> Unity callback. Called object instantiation. </summary>
        protected virtual void Awake() { Instance = this as T; }

    }

}
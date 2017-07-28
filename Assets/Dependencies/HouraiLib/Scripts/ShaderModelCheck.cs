using System;
using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> Disables image effects if the current system does not support them </summary>
    public class ShaderModelCheck : MonoBehaviour {

        [Serializable]
        struct ShaderModelSet {

            public int MinimumShaderModel;
            public Behaviour[] Components;

        }

        [SerializeField]
        ShaderModelSet[] _shaderSets;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            int shaderModel = SystemInfo.graphicsShaderLevel;
            foreach (ShaderModelSet set in _shaderSets)
                foreach (Behaviour behaviour in set.Components)
                    if (behaviour)
                        behaviour.enabled &= shaderModel > set.MinimumShaderModel;
        }

    }

}
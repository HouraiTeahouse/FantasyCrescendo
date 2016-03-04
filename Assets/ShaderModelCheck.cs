using System;
using UnityEngine;

namespace HouraiTeahouse {

    public class ShaderModelCheck : MonoBehaviour {

        [Serializable]
        private struct ShaderModelSet {

            public int MinimumShaderModel;
            public Behaviour[] Components;

        }

        [SerializeField]
        private ShaderModelSet[] _shaderSets;

        void Awake() {
            int shaderModel = SystemInfo.graphicsShaderLevel; 
            foreach(var set in _shaderSets)
                foreach(Behaviour behaviour in set.Components)
                    if (behaviour)
                        behaviour.enabled &= shaderModel > set.MinimumShaderModel;
        }

    }

}

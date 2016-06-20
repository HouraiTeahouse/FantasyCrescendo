// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

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
                        behaviour.enabled &= shaderModel
                            > set.MinimumShaderModel;
        }
    }
}
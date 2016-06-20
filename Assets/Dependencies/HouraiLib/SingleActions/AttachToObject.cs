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

using UnityEngine;

namespace HouraiTeahouse {
    /// <summary> A SingleActionBehaviour that attaches one object to another. </summary>
    public class AttachToObject : SingleActionBehaviour {
        [SerializeField,]
        [Tooltip("The child to attach to the parent.")]
        Transform _child;

        [SerializeField,]
        [Tooltip(
            "Whether to keep the child's world position when attaching to the parent"
            )]
        bool _keepWorldPosition;

        [SerializeField,]
        [Tooltip("The parent object to attach the child to")]
        Transform _parent;

        [SerializeField,]
        [Tooltip(
            "The sibiling index to set the child to. Set as -1 to leave as is.")
        ]
        int _siblingIndex = -1;

        [SerializeField,]
        [Tag,]
        [Tooltip("The tag to search for if a parent is not provided.")]
        string _tag;

        /// <summary> Unity callback. Called when the Editor resets the object. </summary>
        void Reset() {
            _child = transform;
        }

        /// <summary>
        ///     <see cref="SingleActionBehaviour.Action" />
        /// </summary>
        protected override void Action() {
            Transform child = _child;
            Transform parent = _parent;
            if (!parent) {
                GameObject go = GameObject.FindWithTag(_tag);
                if (go)
                    parent = go.transform;
            }
            if (parent) {
                child.SetParent(parent, _keepWorldPosition);
                if (_siblingIndex >= 0)
                    child.SetSiblingIndex(_siblingIndex);
            }
        }
    }
}

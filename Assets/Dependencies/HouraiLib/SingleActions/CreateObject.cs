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
using UnityEngine.UI;

namespace HouraiTeahouse {
    public class CreateObject : SingleActionBehaviour {
        [SerializeField]
        bool _copyPosiiton;

        [SerializeField]
        bool _copyRotation;

        [SerializeField]
        Object _object;

        public System.Action<Object> OnCreate;

        protected override void Action() {
            if (!_object)
                return;
            Object obj = Instantiate(_object);
            if (!_copyPosiiton && !_copyRotation)
                return;
            var go = obj as GameObject;
            var comp = obj as Component;
            Transform objTransform = null;
            if (go != null)
                objTransform = go.transform;
            else if (comp != null)
                objTransform = comp.transform;
            if (!objTransform)
                return;
            var rTransform = objTransform as RectTransform;
            if (rTransform) {
                //if (_copyPosiiton)
                //    rTransform.anchoredPosition3D = transform.position;
                //if (_copyRotation)
                //    rTransform.rotation = transform.rotation;
                LayoutRebuilder.MarkLayoutForRebuild(rTransform);
            }
            else {
                if (_copyPosiiton)
                    objTransform.position = transform.position;
                if (_copyRotation)
                    objTransform.rotation = transform.rotation;
            }
            if (OnCreate != null)
                OnCreate(obj);
        }
    }
}
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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    public abstract class AbstractSelectMenuBuilder<T> : MonoBehaviour
        where T : ScriptableObject, IGameData {
        [SerializeField]
        RectTransform _container;

        [SerializeField]
        RectTransform _prefab;

        protected virtual void Awake() { CreateSelect(); }

        public static void Attach(RectTransform child, Transform parent) {
            child.SetParent(parent, false);
            LayoutRebuilder.MarkLayoutForRebuild(child);
        }

        /// <summary> Construct the select area for characters. </summary>
        void CreateSelect() {
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null || !_container || !_prefab)
                return;

            foreach (T data in GetData()) {
                if (data == null || !CanShowData(data))
                    continue;
                RectTransform character = Instantiate(_prefab);
                Attach(character, _container);
                character.name = data.name;
                character.GetComponentsInChildren<IDataComponent<T>>()
                    .SetData(data);
                LogCreation(data);
            }
        }

        protected abstract IEnumerable<T> GetData();

        protected virtual bool CanShowData(T data) { return true; }

        protected virtual void LogCreation(T data) { }
    }
}
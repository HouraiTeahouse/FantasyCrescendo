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

    /// <summary> An AbstractFactoryEventHandler that creates objects from prefabs in response to events. </summary>
    /// <typeparam name="T"> the type of the object and prefab </typeparam>
    /// <typeparam name="TEvent"> the type of event </typeparam>
    public abstract class PrefabFactoryEventBehaviour<T, TEvent> :
        AbstractFactoryEventBehaviour<T, TEvent> where T : Object {

        [SerializeField]
        T _prefab;

        /// <summary> Gets or sets the prefab to spawn. </summary>
        public T Prefab {
            get { return _prefab; }
            set { _prefab = value; }
        }

        /// <summary>
        ///     <see cref="AbstractFactoryEventBehaviour{T,TEvent}.ShouldCreate" />
        /// </summary>
        protected override bool ShouldCreate(TEvent eventArgs) {
            return base.ShouldCreate(eventArgs) && _prefab != null;
        }

        /// <summary>
        ///     <see cref="AbstractFactoryEventBehaviour{T,TEvent}.Create" />
        /// </summary>
        protected override T Create(TEvent eventArgs) {
            return Instantiate(_prefab);
        }

    }
}

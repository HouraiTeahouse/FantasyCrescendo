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

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary> A PrefabFactoryEventHandler that creates </summary>
    public sealed class PlayerInfoGUI :
        PrefabFactoryEventHandler<RectTransform, PlayerSpawnEvent> {
        /// <summary> The parent RectTransform to attach the spawned objects to. </summary>
        [SerializeField]
        RectTransform _container;

        RectTransform _finalSpace;

        /// <summary> The space prefabs to place before and after all of the elements to keep them centered. </summary>
        [SerializeField]
        RectTransform _spacePrefab;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        protected override void Awake() {
            base.Awake();
            if (!_container) {
                enabled = false;
                return;
            }
            if (!_spacePrefab)
                return;

            RectTransform initialSpace = Instantiate(_spacePrefab);
            _finalSpace = Instantiate(_spacePrefab);

            initialSpace.SetParent(_container.transform);
            _finalSpace.SetParent(_container.transform);

            initialSpace.name = _spacePrefab.name;
            _finalSpace.name = _spacePrefab.name;
        }

        /// <summary>
        ///     <see cref="AbstractFactoryEventHandler{T,TEvent}.ShouldCreate" />
        /// </summary>
        protected override bool ShouldCreate(PlayerSpawnEvent eventArgs) {
            return base.ShouldCreate(eventArgs) && eventArgs.Player != null;
        }

        /// <summary>
        ///     <see cref="AbstractFactoryEventHandler{T,TEvent}.Create" />
        /// </summary>
        protected override RectTransform Create(PlayerSpawnEvent eventArgs) {
            Player player = eventArgs.Player;
            RectTransform display = base.Create(eventArgs);
            display.transform.SetParent(_container.transform, false);
            LayoutRebuilder.MarkLayoutForRebuild(display);
            display.GetComponentsInChildren<IDataComponent<Player>>()
                .SetData(player);
            _finalSpace.transform.SetAsLastSibling();
            return display;
        }
    }
}

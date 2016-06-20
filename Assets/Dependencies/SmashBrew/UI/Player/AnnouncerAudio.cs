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

using HouraiTeahouse.SmashBrew.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> A CharacterUIComponent that creates a playable AudioSOurce that will play the Character's announcer audio
    /// clip </summary>
    public sealed class AnnouncerAudio : CharacterUIComponent<AudioSource>,
                                         ISubmitHandler {
        /// <summary>
        ///     <see cref="ISubmitHandler.OnSubmit" />
        /// </summary>
        public void OnSubmit(BaseEventData eventData) {
            if (Component)
                Component.Play();
        }

        /// <summary>
        ///     <see cref="IDataComponent{T}.SetData" />
        /// </summary>
        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component == null || data == null)
                return;
            data.Announcer.LoadAsync(clip => Component.clip = clip);
        }
    }
}
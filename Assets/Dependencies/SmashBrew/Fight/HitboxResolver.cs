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
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew {
    /// <summary> The global resolver of hitbox collisions. </summary>
    public sealed class HitboxResolver : MonoBehaviour {
        public struct HitboxCollision {
            public Hitbox Source { get; set; }
            public Hitbox Destination { get; set; }

            public void Resolve() { Hitbox.Resolve(Source, Destination); }
        }

        /// <summary> A global list of collisions since the last resolution, sorted by joint priority </summary>
        static readonly PriorityList<HitboxCollision> _collisions =
            new PriorityList<HitboxCollision>();

        [SerializeField]
        [Tooltip("How often to resolve hitbox collsions, in seconds")]
        float _frequency = 1 / 60f;

        Dictionary<IStrikable, HitboxCollision> _targetedCollisions;

        /// <summary> The last time hitbox collisions were resolved </summary>
        float _timer;

        /// <summary> Registers a new collision for resoluiton. </summary>
        /// <param name="src"> the source hitbox </param>
        /// <param name="dst"> the target hitbox </param>
        /// <exception cref="ArgumentNullException"> <paramref name="src" /> or <paramref name="dst" /> are null </exception>
        public static void AddCollision(Hitbox src, Hitbox dst) {
            Check.NotNull(src);
            Check.NotNull(dst);
            // The priority on the collision is the product of the priority on both hitboxes and their 
            _collisions.Add(
                new HitboxCollision {Destination = dst, Source = src},
                (int) src.CurrentType * (int) dst.CurrentType * src.Priority
                    * dst.Priority);
        }

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            _timer = Time.realtimeSinceStartup;
            _targetedCollisions = new Dictionary<IStrikable, HitboxCollision>();
        }

        /// <summary> Unity callback. Called repeatedly on a fixed timestep. </summary>
        void FixedUpdate() {
            float currentTime = Time.realtimeSinceStartup;
            float deltaTime = currentTime - _timer;
            if (deltaTime < _frequency)
                return;
            _timer = currentTime - deltaTime % _frequency;
            if (_collisions.Count <= 0)
                return;
            _targetedCollisions.Clear();
            foreach (HitboxCollision collision in _collisions) {
                AddStrikable(collision.Destination.Damageable, collision);
                AddStrikable(collision.Destination.Knockbackable, collision);
            }
            _collisions.Clear();
            foreach (HitboxCollision collision in _targetedCollisions.Values)
                collision.Resolve();
        }

        void AddStrikable(IStrikable strikable, HitboxCollision collision) {
            if (strikable == null)
                return;
            _targetedCollisions[strikable] = collision;
        }
    }
}

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

namespace HouraiTeahouse.SmashBrew {
    /// <summary> An AnimationBehaviour that causes Characters to constantly move while in the state </summary>
    public class Move : BaseAnimationBehaviour<Character> {
        [SerializeField]
        float _acceleration = 2.2f;

        [SerializeField]
        float _capSpeed = 3.2f;

        [SerializeField]
        [ReadOnly]
        float _currentSpeed;

        [SerializeField]
        [Tooltip(
            "Whether movement in the state ignores or adheres to the difference in state speed"
            )]
        bool _ignoreStateSpeed;

        [SerializeField]
        [Tooltip(
            "The base movement speed that the character will move at while in the state"
            )]
        float _initialSpeed = 2f;

        public override void OnStateEnter(Animator animator,
                                          AnimatorStateInfo stateInfo,
                                          int layerIndex) {
            if (!Target)
                return;
            _currentSpeed = _initialSpeed;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        public override void OnStateUpdate(Animator animator,
                                           AnimatorStateInfo stateInfo,
                                           int layerIndex) {
            if (!Target)
                return;
            Target.Move(_currentSpeed);
            _currentSpeed += _acceleration * Target.FixedDeltaTime;
            if ((_initialSpeed < _capSpeed && _currentSpeed > _capSpeed)
                || (_initialSpeed > _capSpeed && _currentSpeed < _capSpeed))
                _currentSpeed = _capSpeed;
        }
    }
}

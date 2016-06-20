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
    /// <summary> Constants for fast access to Character's animator parameters </summary>
    public static class CharacterAnim {
        // Input
        public static readonly int HorizontalInput =
            Animator.StringToHash("horizontal input");

        public static readonly int VerticalInput =
            Animator.StringToHash("vertical input");

        public static readonly int AttackInput = Animator.StringToHash("attack");

        public static readonly int SpecialInput =
            Animator.StringToHash("special");

        public static readonly int Jump = Animator.StringToHash("jump");
        public static readonly int ShieldInput = Animator.StringToHash("shield");
        public static readonly int Tap = Animator.StringToHash("tap");

        // State Variables
        public static readonly int Grounded = Animator.StringToHash("grounded");

        public static readonly int ShieldHP =
            Animator.StringToHash("shieldHealth");

        public static readonly int Combo = Animator.StringToHash("combo");

        // States
        public static readonly int Run = Animator.StringToHash("run");

        public static bool IsInState(this Animator animator, int stateHash) {
            return animator
                && animator.GetCurrentAnimatorStateInfo(0).shortNameHash
                    == stateHash;
        }
    }
}
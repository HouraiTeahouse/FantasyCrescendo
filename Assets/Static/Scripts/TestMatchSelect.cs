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

namespace HouraiTeahouse.SmashBrew {
    public class TestMatchSelect : MonoBehaviour {
        [Serializable]
        class Selection {
#pragma warning disable 0649
            public CharacterData Data;
            public int Pallete;
#pragma warning restore 0649
        }

        [SerializeField]
        Selection[] testCharacters;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            var index = 0;
            foreach (Player player in Player.AllPlayers) {
                if (index >= testCharacters.Length)
                    break;
                if (player == null || testCharacters[index] == null)
                    continue;
                player.SelectedCharacter = testCharacters[index].Data;
                player.Pallete = testCharacters[index].Pallete;
                player.Type = player.SelectedCharacter
                    ? Player.PlayerType.HumanPlayer
                    : Player.PlayerType.None;
                index++;
            }
        }
    }
}
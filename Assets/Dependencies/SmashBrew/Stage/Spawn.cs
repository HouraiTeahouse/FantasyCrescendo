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
    /// <summary> A EventHandler for spawning characters at the start of the match </summary>
    public class Spawn : EventHandlerBehaviour<MatchStartEvent> {
        [Serializable]
        class SpawnPoint {
#pragma warning disable 0649
            public Transform Point;
            public bool Direction;
#pragma warning restore 0649
        }

        [SerializeField]
        [Tooltip("The spawn points for each of the characters")]
        SpawnPoint[] _spawnPoints;

        /// <summary> Spawns players when the match begins. </summary>
        /// <param name="eventArgs"> </param>
        protected override void OnEvent(MatchStartEvent eventArgs) {
            var i = 0;
            IEnumerator<Player> activePlayers =
                Player.ActivePlayers.GetEnumerator();
            while (i < _spawnPoints.Length && activePlayers.MoveNext()) {
                Player player = activePlayers.Current;
                Character runtimeCharacter = player.Spawn(
                    _spawnPoints[i].Point,
                    _spawnPoints[i].Direction);
                i++;
                if (runtimeCharacter == null)
                    continue;

                //TODO: Fix this hack, get netplay working
                runtimeCharacter.gameObject.SetActive(true);
                runtimeCharacter.name =
                    "Player {0} ({1})".With(player.PlayerNumber + 1,
                        player.SpawnedCharacter.name);
                EventManager.Publish(new PlayerSpawnEvent {
                    Player = player,
                    PlayerObject = runtimeCharacter.gameObject
                });
            }
        }
    }
}

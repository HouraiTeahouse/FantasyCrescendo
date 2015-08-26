using System;
using UnityEngine;
using Vexe.Runtime.Extensions;

namespace Hourai.SmashBrew {
    
    public sealed class Player {

        public enum PlayerType {

            Disabled = 0,
            CPU = 1,
            HumanPlayer = 2

        };

        public readonly int PlayerNumber;
        public CharacterData Character;

        private Character _spawnedInstance;

        public int CpuLevel { get; set; }

        public int Pallete { get; set; }

        public PlayerType Type { get; set; }

        public Character SpawnedCharacter { get; private set; }

        public static PlayerType GetNextType(PlayerType pt) {
            return pt + 1 > PlayerType.HumanPlayer ? PlayerType.Disabled : pt + 1;
        }

        public void CycleType() {
            Type = GetNextType(Type);
            if (Type == PlayerType.Disabled)
                Character = null;
        }

        public Color GetColor() {
            switch (Type) {
                case PlayerType.HumanPlayer:
                    return SmashGame.GetPlayerColor(PlayerNumber);
                case PlayerType.CPU:
                    return SmashGame.Config.CPUColor;
                default:
                    return Color.clear;
            }
        }

        internal Player(int number) {
            PlayerNumber = number;
            CpuLevel = 3;
        }

        internal Character Spawn(Transform transform = null) {
            if(transform == null)
                throw new ArgumentNullException("transform");
            return Spawn(transform.position, transform.rotation);
        }

        internal Character Spawn(Vector3 pos, Quaternion rot) {
            Character prefab = Character.LoadPrefab(Pallete);
            if (prefab == null)
                return null;
            SpawnedCharacter = prefab.InstantiateNew(pos, rot);
            SpawnedCharacter.PlayerNumber = PlayerNumber;
            return SpawnedCharacter;
        }

    }

}
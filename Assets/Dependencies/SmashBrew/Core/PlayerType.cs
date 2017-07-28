using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public sealed class PlayerType {

        public static readonly PlayerType[] Types;

        public static readonly PlayerType None = new PlayerType {
            Name = "None",
            ShortName = "",
            IsActive = false,
            IsCPU = false,
            Color = UnityEngine.Color.clear
        };

        public static readonly PlayerType HumanPlayer = new PlayerType {
            Name = "Player {0}",
            ShortName = "P{0}",
            IsActive = true,
            IsCPU = false,
            Color = null
        };

        public static readonly PlayerType CPU = new PlayerType {
            Name = "CPU",
            ShortName = "CPU",
            IsActive = true,
            IsCPU = true,
            Color = UnityEngine.Color.gray
        };

        static PlayerType() {
            Types = new[] {None, HumanPlayer, CPU};
            for (short i = 0; i < Types.Length - 1; i++) {
                Types[i].Next = Types[i + 1];
                Types[i].ID = i;
            }
            Types[Types.Length - 1].Next = Types[0];
        }

        // Private constructor
        PlayerType() { }
        public short ID { get; private set; }
        public string Name { get; private set; }
        public string ShortName { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsCPU { get; private set; }
        public Color? Color { get; private set; }
        public PlayerType Next { get; private set; }

        public override string ToString() { return Name; }

    }
}

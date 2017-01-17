using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    public class TestMatchSelect : MonoBehaviour {

        [SerializeField]
        PlayerSelection[] testCharacters;

        /// <summary> Unity callback. Called on object instantiation. </summary>
        void Awake() {
            var index = 0;
            foreach (Player player in PlayerManager.Instance.MatchPlayers) {
                if (index >= testCharacters.Length)
                    break;
                if (player == null || testCharacters[index] == null)
                    continue;
                player.Selection = testCharacters[index];
                player.Type = player.Selection.Character ? PlayerType.HumanPlayer : PlayerType.None;
                index++;
            }
        }

    }

}

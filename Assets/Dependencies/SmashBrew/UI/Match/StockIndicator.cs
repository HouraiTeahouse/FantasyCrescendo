using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    [RequireComponent(typeof (Text))]
    public class StockIndicator : MonoBehaviour, IPlayerGUIComponent {

        private Text display;
        private Player _player;
        private StockMatch _stockMatch;

        [SerializeField]
        private int index;

        private void Start() {
            _stockMatch = FindObjectOfType<StockMatch>();
            if (!_stockMatch) {
                enabled = false;
                return;
            }
            display = GetComponent<Text>();
        }

        private void Update() {
            display.text = _stockMatch[_player].ToString();
        }

        public void SetPlayer(Player data) {
            _player = data;
        }

    }

}

using UnityEngine;
using UnityEngine.UI;

namespace Crescendo.API {

    [RequireComponent(typeof (Text))]
    public class StockIndicator : MonoBehaviour {

        private StockMatch.StockCriteria criteria;
        private Text display;

        [SerializeField]
        private int index;

        // Use this for initialization
        private void Start() {
            var match = FindObjectOfType<StockMatch>();
            criteria = match.GetPlayerData(index);
            display = GetComponent<Text>();
        }

        // Update is called once per frame
        private void Update() {
            display.text = criteria.Lives.ToString();
        }

    }

}
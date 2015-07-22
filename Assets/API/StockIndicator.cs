using UnityEngine;
using Genso.API;
using UnityEngine.UI;

namespace Genso.API {

    [RequireComponent(typeof(Text))]
    public class StockIndicator : MonoBehaviour {

        private StockMatch.StockCriteria criteria;
        private Text display;

        [SerializeField]
        private int index;

        // Use this for initialization
        void Start() {
            StockMatch match = FindObjectOfType<StockMatch>();
            criteria = match.GetPlayerData(index);
            display = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update() {
            display.text = criteria.Lives.ToString();
        }
    }

}


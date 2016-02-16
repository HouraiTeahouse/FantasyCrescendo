using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {

    /// <summary>
    /// Matches the color between multiple Graphics.
    /// </summary>
    [ExecuteInEditMode]
    public class MatchColor : MonoBehaviour {

        [SerializeField]
        private Graphic _source;

        [SerializeField]
        private Graphic[] _targets;

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            if (_source == null)
                _source = GetComponent<Graphic>();
        }

        /// <summary>
        /// Unity Callback. Called once per frame.
        /// </summary>
        void Update() {
            if (_source == null || _targets == null)
                return;

            foreach (Graphic graphic in _targets)
                graphic.color = _source.color;
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew {

    public class SynchronizeTextBestFit : MonoBehaviour {

        [SerializeField]
        Text[] _texts;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        void OnEnable() {
            StartCoroutine(UpdateSizes());
        }

        IEnumerator UpdateSizes() {
            yield return null;
            yield return null;
            while (true) {
                var minSize = _texts.Min(t => t.cachedTextGenerator.fontSizeUsedForBestFit);
                Log.Debug(minSize);
                foreach (var text in _texts)
                    text.resizeTextMaxSize = minSize;
                yield return null;
            }
        }

    }
}

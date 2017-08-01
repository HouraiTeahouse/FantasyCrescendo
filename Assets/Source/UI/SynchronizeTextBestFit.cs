using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew {

    public class SynchronizeTextBestFit : UIBehaviour {

        [SerializeField]
        Text[] _texts;

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        protected override void OnEnable() {
            base.OnEnable();
            StartCoroutine(UpdateSizes());
        }

        protected override void OnRectTransformDimensionsChange() {
            foreach (var text in _texts)
                if (text != null)
                    text.resizeTextMaxSize = 10000;
            if (!isActiveAndEnabled)
                return;
            StopAllCoroutines();
            StartCoroutine(UpdateSizes());
        }

        IEnumerator UpdateSizes() {
            yield return null;
            yield return null;
            while (true) {
                var minSize = _texts.Min(t => t.cachedTextGenerator.fontSizeUsedForBestFit);
                foreach (var text in _texts)
                    text.resizeTextMaxSize = minSize;
                yield return null;
            }
        }

    }
}

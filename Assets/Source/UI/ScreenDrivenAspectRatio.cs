using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.FantasyCrescendo.UI {

    [ExecuteInEditMode]
    public class ScreenDrivenAspectRatio : UIBehaviour { 

        [SerializeField]
        AspectRatioFitter _aspectRatioFitter;

        protected override void OnRectTransformDimensionsChange() {
            UpdateAspectRatio();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update() {
            UpdateAspectRatio();
        }
#endif

        void UpdateAspectRatio() {
            if (_aspectRatioFitter == null)
                return;
            var canvas = GetComponentInParent<Canvas>();
            var rect = canvas.pixelRect;
            if (rect.height == 0)
                return;
            _aspectRatioFitter.aspectRatio = rect.AspectRatio();
        }

    }

}

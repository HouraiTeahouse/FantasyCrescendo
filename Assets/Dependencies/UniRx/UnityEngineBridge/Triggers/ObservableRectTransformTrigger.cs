#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using UnityEngine;

namespace UniRx.Triggers {

    [DisallowMultipleComponent]
    public class ObservableRectTransformTrigger : ObservableTriggerBase {

        Subject<Unit> onRectTransformDimensionsChange;

        Subject<Unit> onRectTransformRemoved;

        // Callback that is sent if an associated RectTransform has it's dimensions changed
        public void OnRectTransformDimensionsChange() {
            if (onRectTransformDimensionsChange != null)
                onRectTransformDimensionsChange.OnNext(Unit.Default);
        }

        /// <summary> Callback that is sent if an associated RectTransform has it's dimensions changed. </summary>
        public IObservable<Unit> OnRectTransformDimensionsChangeAsObservable() {
            return onRectTransformDimensionsChange ?? (onRectTransformDimensionsChange = new Subject<Unit>());
        }

        // Callback that is sent if an associated RectTransform is removed
        public void OnRectTransformRemoved() {
            if (onRectTransformRemoved != null)
                onRectTransformRemoved.OnNext(Unit.Default);
        }

        /// <summary> Callback that is sent if an associated RectTransform is removed. </summary>
        public IObservable<Unit> OnRectTransformRemovedAsObservable() {
            return onRectTransformRemoved ?? (onRectTransformRemoved = new Subject<Unit>());
        }

        protected override void RaiseOnCompletedOnDestroy() {
            if (onRectTransformDimensionsChange != null) {
                onRectTransformDimensionsChange.OnCompleted();
            }
            if (onRectTransformRemoved != null) {
                onRectTransformRemoved.OnCompleted();
            }
        }

    }

}

#endif
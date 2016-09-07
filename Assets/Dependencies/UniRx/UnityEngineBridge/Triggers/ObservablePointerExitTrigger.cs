#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using UnityEngine;
using UnityEngine.EventSystems;
// require keep for Windows Universal App

namespace UniRx.Triggers {

    [DisallowMultipleComponent]
    public class ObservablePointerExitTrigger : ObservableTriggerBase, IEventSystemHandler, IPointerExitHandler {

        Subject<PointerEventData> onPointerExit;

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if (onPointerExit != null)
                onPointerExit.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnPointerExitAsObservable() {
            return onPointerExit ?? (onPointerExit = new Subject<PointerEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy() {
            if (onPointerExit != null) {
                onPointerExit.OnCompleted();
            }
        }

    }

}

#endif
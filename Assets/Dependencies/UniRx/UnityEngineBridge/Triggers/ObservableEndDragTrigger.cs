#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using UnityEngine;
using UnityEngine.EventSystems;
// require keep for Windows Universal App

namespace UniRx.Triggers {

    [DisallowMultipleComponent]
    public class ObservableEndDragTrigger : ObservableTriggerBase, IEventSystemHandler, IEndDragHandler {

        Subject<PointerEventData> onEndDrag;

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            if (onEndDrag != null)
                onEndDrag.OnNext(eventData);
        }

        public IObservable<PointerEventData> OnEndDragAsObservable() {
            return onEndDrag ?? (onEndDrag = new Subject<PointerEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy() {
            if (onEndDrag != null) {
                onEndDrag.OnCompleted();
            }
        }

    }

}

#endif
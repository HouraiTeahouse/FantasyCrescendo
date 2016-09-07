#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using UnityEngine;
using UnityEngine.EventSystems;
// require keep for Windows Universal App

namespace UniRx.Triggers {

    [DisallowMultipleComponent]
    public class ObservableMoveTrigger : ObservableTriggerBase, IEventSystemHandler, IMoveHandler {

        Subject<AxisEventData> onMove;

        void IMoveHandler.OnMove(AxisEventData eventData) {
            if (onMove != null)
                onMove.OnNext(eventData);
        }

        public IObservable<AxisEventData> OnMoveAsObservable() {
            return onMove ?? (onMove = new Subject<AxisEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy() {
            if (onMove != null) {
                onMove.OnCompleted();
            }
        }

    }

}

#endif
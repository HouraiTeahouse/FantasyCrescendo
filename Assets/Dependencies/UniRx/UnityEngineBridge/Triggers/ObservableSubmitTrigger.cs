#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using UnityEngine;
using UnityEngine.EventSystems;
// require keep for Windows Universal App

namespace UniRx.Triggers {

    [DisallowMultipleComponent]
    public class ObservableSubmitTrigger : ObservableTriggerBase, IEventSystemHandler, ISubmitHandler {

        Subject<BaseEventData> onSubmit;

        void ISubmitHandler.OnSubmit(BaseEventData eventData) {
            if (onSubmit != null)
                onSubmit.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnSubmitAsObservable() {
            return onSubmit ?? (onSubmit = new Subject<BaseEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy() {
            if (onSubmit != null) {
                onSubmit.OnCompleted();
            }
        }

    }

}

#endif
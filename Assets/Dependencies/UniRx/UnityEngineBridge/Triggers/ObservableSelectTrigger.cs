#if !(UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5)

using UnityEngine;
using UnityEngine.EventSystems;
// require keep for Windows Universal App

namespace UniRx.Triggers {

    [DisallowMultipleComponent]
    public class ObservableSelectTrigger : ObservableTriggerBase, IEventSystemHandler, ISelectHandler {

        Subject<BaseEventData> onSelect;

        void ISelectHandler.OnSelect(BaseEventData eventData) {
            if (onSelect != null)
                onSelect.OnNext(eventData);
        }

        public IObservable<BaseEventData> OnSelectAsObservable() {
            return onSelect ?? (onSelect = new Subject<BaseEventData>());
        }

        protected override void RaiseOnCompletedOnDestroy() {
            if (onSelect != null) {
                onSelect.OnCompleted();
            }
        }

    }

}

#endif
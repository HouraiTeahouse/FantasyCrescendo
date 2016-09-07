using UnityEngine;
// require keep for Windows Universal App

namespace UniRx.Triggers {

    [DisallowMultipleComponent]
    public class ObservableCollisionTrigger : ObservableTriggerBase {

        Subject<Collision> onCollisionEnter;

        Subject<Collision> onCollisionExit;

        Subject<Collision> onCollisionStay;

        /// <summary> OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider. </summary>
        void OnCollisionEnter(Collision collision) {
            if (onCollisionEnter != null)
                onCollisionEnter.OnNext(collision);
        }

        /// <summary> OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider. </summary>
        public IObservable<Collision> OnCollisionEnterAsObservable() {
            return onCollisionEnter ?? (onCollisionEnter = new Subject<Collision>());
        }

        /// <summary> OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider. </summary>
        void OnCollisionExit(Collision collisionInfo) {
            if (onCollisionExit != null)
                onCollisionExit.OnNext(collisionInfo);
        }

        /// <summary> OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider. </summary>
        public IObservable<Collision> OnCollisionExitAsObservable() {
            return onCollisionExit ?? (onCollisionExit = new Subject<Collision>());
        }

        /// <summary> OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider. </summary>
        void OnCollisionStay(Collision collisionInfo) {
            if (onCollisionStay != null)
                onCollisionStay.OnNext(collisionInfo);
        }

        /// <summary> OnCollisionStay is called once per frame for every collider/rigidbody that is touching rigidbody/collider. </summary>
        public IObservable<Collision> OnCollisionStayAsObservable() {
            return onCollisionStay ?? (onCollisionStay = new Subject<Collision>());
        }

        protected override void RaiseOnCompletedOnDestroy() {
            if (onCollisionEnter != null) {
                onCollisionEnter.OnCompleted();
            }
            if (onCollisionExit != null) {
                onCollisionExit.OnCompleted();
            }
            if (onCollisionStay != null) {
                onCollisionStay.OnCompleted();
            }
        }

    }

}
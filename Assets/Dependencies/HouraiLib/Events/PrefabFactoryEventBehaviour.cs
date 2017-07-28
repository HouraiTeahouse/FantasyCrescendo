using UnityEngine;

namespace HouraiTeahouse {

    /// <summary> An AbstractFactoryEventHandler that creates objects from prefabs in response to events. </summary>
    /// <typeparam name="T"> the type of the object and prefab </typeparam>
    /// <typeparam name="TEvent"> the type of event </typeparam>
    public abstract class PrefabFactoryEventBehaviour<T, TEvent> : AbstractFactoryEventBehaviour<T, TEvent>
        where T : Object {

        [SerializeField]
        T _prefab;

        /// <summary> Gets or sets the prefab to spawn. </summary>
        public T Prefab {
            get { return _prefab; }
            set { _prefab = value; }
        }

        /// <summary>
        ///     <see cref="AbstractFactoryEventBehaviour{T,TEvent}.ShouldCreate" />
        /// </summary>
        protected override bool ShouldCreate(TEvent eventArgs) {
            return base.ShouldCreate(eventArgs) && _prefab != null;
        }

        /// <summary>
        ///     <see cref="AbstractFactoryEventBehaviour{T,TEvent}.Create" />
        /// </summary>
        protected override T Create(TEvent eventArgs) {
            return Instantiate(_prefab);
        }

    }

}
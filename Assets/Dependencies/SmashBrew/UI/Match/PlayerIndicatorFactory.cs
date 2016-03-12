using System;
using HouraiTeahouse.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// A PrefabFactoryEventHandler that produces PlayerIndicators in response to Players spawning.
    /// </summary>
    public sealed class PlayerIndicatorFactory : PrefabFactoryEventHandler<PlayerIndicator, PlayerSpawnEvent> {
        /// <summary>
        /// <see cref="AbstractFactoryEventHandler{T,TEvent}.Create"/>
        /// </summary>
        protected override PlayerIndicator Create(PlayerSpawnEvent eventArgs) {
            PlayerIndicator indicator = base.Create(eventArgs);
            indicator.GetComponentsInChildren<IDataComponent<Player>>().SetData(eventArgs.Player);
            return indicator;
        }
    }
}
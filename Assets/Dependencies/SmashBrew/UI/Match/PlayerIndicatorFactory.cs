namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A PrefabFactoryEventHandler that produces PlayerIndicators in response to Players spawning. </summary>
    public sealed class PlayerIndicatorFactory : PrefabFactoryEventBehaviour<PlayerIndicator, PlayerSpawnEvent> {

        /// <summary>
        ///     <see cref="AbstractFactoryEventBehaviour{T,TEvent}.Create" />
        /// </summary>
        protected override PlayerIndicator Create(PlayerSpawnEvent eventArgs) {
            PlayerIndicator indicator = base.Create(eventArgs);
            indicator.GetComponentsInChildren<IDataComponent<Player>>().SetData(eventArgs.Player);
            return indicator;
        }

    }

}
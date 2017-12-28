namespace HouraiTeahouse.FantasyCrescendo {

public interface ICharacterSimulation : ICharacterComponent,
                                        ISimulation<PlayerState, PlayerInput> {

  void Presimulate(PlayerState state);

}

}

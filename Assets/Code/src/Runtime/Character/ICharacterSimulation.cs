namespace HouraiTeahouse.FantasyCrescendo {

public interface ICharacterSimulation : ICharacterComponent,
                                        ISimulation<PlayerState, PlayerInputContext> {

  void Presimulate(PlayerState state);

}

}

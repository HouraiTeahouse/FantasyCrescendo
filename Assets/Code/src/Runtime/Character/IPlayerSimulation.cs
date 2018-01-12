namespace HouraiTeahouse.FantasyCrescendo {

public interface IPlayerSimulation : IPlayerComponent,
                                     ISimulation<PlayerState, PlayerInputContext> {

  PlayerState ResetState(PlayerState state);
  void Presimulate(PlayerState state);

}

}

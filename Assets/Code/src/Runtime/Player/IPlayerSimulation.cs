namespace HouraiTeahouse.FantasyCrescendo.Players {

public interface IPlayerSimulation : IPlayerComponent,
                                     ISimulation<PlayerState, PlayerInputContext> {

  void ResetState(ref PlayerState state);
  void Presimulate(in PlayerState state);

}

}

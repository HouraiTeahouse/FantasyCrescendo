namespace HouraiTeahouse.FantasyCrescendo {

public interface IGameController<S> {

  uint Timestep { get; set; }
  S CurrentState { get; set; }

  void Update();

}

}

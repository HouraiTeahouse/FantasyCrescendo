namespace HouraiTeahouse.FantasyCrescendo {

public abstract class AbstractGameController {

  public virtual uint Timestep { get; protected set; }
  public GameState CurrentState { get; set; }
  public ISimulation<GameState, GameInput> Simulation { get; set; }

  public abstract void Update();

}

}

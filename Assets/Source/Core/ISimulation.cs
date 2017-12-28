
namespace HouraiTeahouse.FantasyCrescendo {

public interface ISimulation<S, I> {

  S Simulate(S state, I input);

}

}

using HouraiTeahouse.Tasks;

namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// An asynchronously initializable object.
/// </summary>
public interface IInitializable<T> {

  /// <summary>
  /// Starts initializing the object with a given config.
  /// </summary>
  /// <param name="config">the configuration options for initialization.</param>
  /// <returns>a promise that will be resolved on completion of initialization.</returns>
  ITask Initialize(T config);

}

}

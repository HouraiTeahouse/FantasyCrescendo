namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A timed input summary.
/// </summary>
public struct TimedInput<I> {
  
  /// <summary>
  /// The input value.
  /// </summary>
  public I Input;

  /// <summary>
  /// The timestep associated with the input.
  /// </summary>
  public uint Timestep;
}

}
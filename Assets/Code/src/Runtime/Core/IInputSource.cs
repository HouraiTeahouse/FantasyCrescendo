namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A source of MatchInputs.
/// </summary>
public interface IInputSource<I> {

  /// <summary>
  /// Samples an input to represent the current timeframe's input.
  /// </summary>
  /// <returns>the input summary for the current time frame.</returns>
  I SampleInput();

}

}

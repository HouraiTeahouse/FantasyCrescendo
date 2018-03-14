using HouraiTeahouse.FantasyCrescendo.Matches;

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

public interface IMatchInputSource : IInputSource<MatchInput> {

  /// <summary>
  /// Gets the bitmask of what inputs are valid. 
  /// </summary>
  /// <remarks>
  /// Lowest signifgant bit is Player 1.
  /// Highest signifigant bit is Player 8.
  /// </remarks>
  byte ValidMask { get; }

}

}

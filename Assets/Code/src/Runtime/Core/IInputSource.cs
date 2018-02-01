namespace HouraiTeahouse.FantasyCrescendo {

/// <summary>
/// A source of input that can be summarized as data objects
/// of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">the type of input that is sampled.</typeparam>
public interface IInputSource<I> {

  /// <summary>
  /// Samples an input to represent the current timeframe's input.
  /// </summary>
  /// <returns>the input summary for the current time frame.</returns>
  I SampleInput();

}

}

using HouraiTeahouse.FantasyCrescendo.Matches;

namespace HouraiTeahouse.FantasyCrescendo {

public class BufferedInputSource : IInputSource {

  public byte ValidMask => baseInputSource.ValidMask;

  readonly IInputSource baseInputSource;
  readonly MatchInput[] buffer;
  int index;

  public BufferedInputSource(int bufferSize, IInputSource baseInputSource) {
    this.baseInputSource = baseInputSource;
    buffer = new MatchInput[bufferSize];
    MatchInput sampledBaseInput = baseInputSource.SampleInput();
    for (int i = 0; i < buffer.Length; i++) {
      buffer[i] = sampledBaseInput;
    }
  }

  public MatchInput SampleInput() {
    buffer[index] = baseInputSource.SampleInput();
    index = (index + 1) % buffer.Length;
    return buffer[index];
  }

}

}

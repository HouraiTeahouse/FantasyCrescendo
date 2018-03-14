using HouraiTeahouse.FantasyCrescendo.Matches;

namespace HouraiTeahouse.FantasyCrescendo {

public class BufferedInputSource<I> : IInputSource<I> {

  readonly IInputSource<I> baseInputSource;
  readonly I[] buffer;
  int index;

  public BufferedInputSource(int bufferSize, IInputSource<I> baseInputSource) {
    this.baseInputSource = baseInputSource;
    buffer = new I[bufferSize];
    I sampledBaseInput = baseInputSource.SampleInput();
    for (int i = 0; i < buffer.Length; i++) {
      buffer[i] = sampledBaseInput;
    }
  }

  public I SampleInput() {
    buffer[index] = baseInputSource.SampleInput();
    index = (index + 1) % buffer.Length;
    return buffer[index];
  }

}

}

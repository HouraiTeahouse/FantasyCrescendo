using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class InControlInputSource : IMatchInputSource {

  public byte ValidMask { get; }

  MatchInput input;
  readonly IInputSource<PlayerInput>[] playerInputs;

  public InControlInputSource(MatchConfig config) {
    input = new MatchInput(config);
    ValidMask = 0;
    playerInputs = new IInputSource<PlayerInput>[config.PlayerCount];
    for (var i = 0; i < config.PlayerCount; i++) {
      playerInputs[i] = new InControlPlayerInputSource(config.PlayerConfigs[i]);
      if (!config.PlayerConfigs[i].IsLocal) continue;
      ValidMask |= (byte)(1 << i);
    }
    Debug.Log($"Valid Mask: {ValidMask}");
  }
  
  public MatchInput SampleInput() {
    input = new MatchInput(input.PlayerCount);
    for (var i = 0; i < input.PlayerCount; i++) {
      input[i] = playerInputs[i].SampleInput();
    }
    return input;
  }

}

}

using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class MatchInputSourceBase<T> : IMatchInputSource where T : IInputSource<PlayerInput> {

  public byte ValidMask { get; }

  MatchInput input;
  readonly IInputSource<PlayerInput>[] playerInputs;

  protected MatchInputSourceBase(MatchConfig config) {
    input = new MatchInput(config);
    ValidMask = 0;
    playerInputs = new IInputSource<PlayerInput>[config.PlayerCount];
    for (var i = 0; i < config.PlayerCount; i++) {
      playerInputs[i] = BuildPlayerInputSource(ref config.PlayerConfigs[i]);
      if (!config.PlayerConfigs[i].IsLocal) continue;
      ValidMask |= (byte)(1 << i);
    }
    Debug.Log($"Valid Mask: {ValidMask}");
  }

  IInputSource<PlayerInput> BuildPlayerInputSource(ref PlayerConfig config) {
    IInputSource<PlayerInput> inputSource = (T)Activator.CreateInstance(typeof(T), config);
    // Only override for player 1
    if (config.LocalPlayerID == 0) {
      inputSource = new KeyboardOverridePlayerInputSource(inputSource);
    }
    inputSource = new TapPlayerInputSource(inputSource);
    return inputSource;
  }
  
  public MatchInput SampleInput() {
    input = new MatchInput(input.PlayerCount);
    for (var i = 0; i < input.PlayerCount; i++) {
      input[i] = playerInputs[i].SampleInput();
    }
    return input;
  }

}

public class InControlInputSource : MatchInputSourceBase<InControlPlayerInputSource> {
  public InControlInputSource(MatchConfig config) : base(config) {
  }
}

public class UnityInputSource : MatchInputSourceBase<UnityPlayerInputSource> {
  public UnityInputSource(MatchConfig config) : base(config) {
  }
}


}

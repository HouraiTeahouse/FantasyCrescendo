using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class MatchInputSourceBase<T> : IInputSource<MatchInput> where T : IInputSource<PlayerInput> {

  readonly IInputSource<PlayerInput>[] playerInputs;

  protected MatchInputSourceBase(MatchConfig config) {
    playerInputs = new IInputSource<PlayerInput>[config.PlayerCount];
    for (var i = 0; i < playerInputs.Length; i++) {
      playerInputs[i] = BuildPlayerInputSource(ref config[i]);
    }
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
    var input = new MatchInput();
    for (var i = 0; i < playerInputs.Length; i++) {
      input[i] = playerInputs[i].SampleInput();
    }
    return input;
  }

}

public class UnityInputSource : MatchInputSourceBase<UnityPlayerInputSource> {
  public UnityInputSource(MatchConfig config) : base(config) {
  }
}


}

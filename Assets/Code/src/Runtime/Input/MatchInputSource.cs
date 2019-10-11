using HouraiTeahouse.FantasyCrescendo.Matches;
using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using UnityEngine;
using PlayerInput = HouraiTeahouse.FantasyCrescendo.Players.PlayerInput;

namespace HouraiTeahouse.FantasyCrescendo {

public abstract class MatchInputSourceBase<T> : IInputSource<MatchInput> where T : IInputSource<PlayerInput> {

  byte _validMask;

  MatchInput input;
  readonly IInputSource<PlayerInput>[] playerInputs;

  protected MatchInputSourceBase(MatchConfig config) {
    input = new MatchInput(config);
    _validMask = 0;
    playerInputs = new IInputSource<PlayerInput>[config.PlayerCount];
    for (var i = 0; i < config.PlayerCount; i++) {
      playerInputs[i] = BuildPlayerInputSource(ref config.PlayerConfigs[i]);
      BitUtil.SetBit(ref _validMask, i, config.PlayerConfigs[i].IsLocal);
    }
    Debug.Log($"Valid Mask: {_validMask}");
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
    input.ValidMask = _validMask;
    for (var i = 0; i < input.PlayerCount; i++) {
      if (input.IsPlayerValid(i)) {
        input[i] = playerInputs[i].SampleInput();
      }
    }
    return input;
  }

}

public class UnityInputSource : MatchInputSourceBase<UnityPlayerInputSource> {
  public UnityInputSource(MatchConfig config) : base(config) {
  }
}


}

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace HouraiTeahouse.FantasyCrescendo {

[RequireComponent(typeof(CharacterStateMachine))]
public class CharacterAnimation : MonoBehaviour, IPlayerSimulation, IPlayerView {

  public PlayableDirector Director;

  CharacterStateMachine StateMachine;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    StateMachine = GetComponent<CharacterStateMachine>();
    if (Director == null) {
      Director = GetComponent<PlayableDirector>();
    }
    Director.timeUpdateMode = DirectorUpdateMode.Manual;
  }

  public Task Initialize(PlayerConfig config, bool isView = false) => Task.CompletedTask;

  public void Presimulate(PlayerState state) => ApplyState(state);

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    Director.time += Time.fixedDeltaTime;
    Director.Evaluate();
    state.NormalizedStateTime = (float)(Director.time / Director.duration);
    return state;
  }

  public void ApplyState(PlayerState state) {
    StateMachine.Presimulate(state);
  }

  public PlayerState ResetState(PlayerState state) => state;

  void PlayState(ref PlayerState state) {
    var timeline = StateMachine.StateData.Timeline;
    var time = state.NormalizedStateTime * timeline.duration;
    if (timeline != Director.playableAsset) {
      Director.Play(timeline);
    }
    Director.time = time % Director.duration;
    Director.Evaluate();
  }

}

}

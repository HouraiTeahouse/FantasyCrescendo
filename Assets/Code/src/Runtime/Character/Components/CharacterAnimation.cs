using HouraiTeahouse.FantasyCrescendo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

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

  public Task Initialize(PlayerConfig config, bool isView = false) {
    if (!isView) {
      var animator = GetComponentInChildren<Animator>();
      if (animator != null){
        animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
      }
    }
    return Task.CompletedTask;
  }

  public void Presimulate(PlayerState state) => ApplyState(state);

  public PlayerState Simulate(PlayerState state, PlayerInputContext input) {
    Director.time += Time.fixedDeltaTime;
    Director.Evaluate();
    state.StateTick++;
    ApplyState(state);
    return state;
  }

  public void ApplyState(PlayerState state) {
    StateMachine.Presimulate(state);
    PlayState(state);
  }

  public PlayerState ResetState(PlayerState state) => state;

  void PlayState(PlayerState state) {
    var timeline = StateMachine.StateData.Timeline;
    if (timeline != Director.playableAsset) {
      Director.Play(timeline);
    }
    Director.time = state.StateTime % Director.duration;
    Director.Evaluate();
  }

}

}

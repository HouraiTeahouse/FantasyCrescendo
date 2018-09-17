using HouraiTeahouse.FantasyCrescendo.Players;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Assertions;
using UnityEngine.Audio;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

[RequireComponent(typeof(CharacterStateMachine))]
public class CharacterAnimation : PlayerComponent {

  class ControllerInfo {

    const int kMinOutputCount = 2;

    public PlayableGraph Graph;
    public GameObject Owner;
    public AnimationMixerPlayable Animation;
    public AudioMixerPlayable Audio;

    public Playable CreatePlayable(TimelineAsset timeline) {
      var playable = timeline.CreatePlayable(Graph, Owner);
      if (playable.GetOutputCount() < kMinOutputCount) {
        playable.SetOutputCount(kMinOutputCount);
      }
      return playable;
    }

    public void Connect(int index, Playable timeline) {
      if (!Animation.IsNull() && Animation.IsValid()) Animation.ConnectInput(index, timeline, 0);
      if (!Audio.IsNull() && Audio.IsValid()) Audio.ConnectInput(index, timeline, 1);
    }

    public void Disconnect(int index) {
      if (!Animation.IsNull() && Animation.IsValid()) Animation.DisconnectInput(index);
      if (!Audio.IsNull() && Audio.IsValid()) Audio.DisconnectInput(index);
    }

    public void SetWeight(int index, float weight) {
      if (!Animation.IsNull() && Animation.IsValid()) Animation.SetInputWeight(index, weight);
      if (!Audio.IsNull() && Audio.IsValid()) Audio.SetInputWeight(index, weight);
    }

  }

  class StateInfo {

    public readonly State State;
    public readonly ControllerInfo Controller;

    readonly Playable TimelinePlayable;
    readonly double Duration;
    readonly int MixerIndex;

    public StateInfo(State state, ControllerInfo controller, int mixerIndex = 0) {
      State = state;
      Controller = controller;
      MixerIndex = mixerIndex;

      var timeline = State.Data.Timeline;
      Duration = timeline.duration;
      TimelinePlayable = controller.CreatePlayable(timeline);
    }

    public void Connect() => Controller.Connect(MixerIndex, TimelinePlayable);
    public void Disconnect() => Controller.Disconnect(MixerIndex);
    public void SetWeight(float weight) => Controller.SetWeight(MixerIndex, weight);
    public void SetTime(in PlayerState state) => TimelinePlayable.SetTime(state.StateTime % Duration);

  }

  CharacterStateMachine StateMachine;
  PlayableGraph _playableGraph;
  Dictionary<uint, StateInfo> _stateMap;
  StateInfo[] _states;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    StateMachine = GetComponent<CharacterStateMachine>();
    ObjectUtil.DestroyAll<PlayableDirector>(this);
  }

  /// <summary>
  /// This function is called when the MonoBehaviour will be destroyed.
  /// </summary>
  void OnDestroy() {
    if (_playableGraph.IsValid()) {
      _playableGraph.Destroy();
    }
  }

  public override Task Initialize(PlayerConfig config, bool isView = false) {
    var animator = ObjectUtil.GetFirst<Animator>(this);
    if (animator == null) return Task.CompletedTask;
    //OptimizeHierarchy();

    _playableGraph = PlayableGraph.Create(name);
    _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

    var controller = new ControllerInfo { 
      Graph = _playableGraph, 
      Owner = gameObject,
    };

    _stateMap = new Dictionary<uint, StateInfo>();
    // TODO(james7132): Set up proper state blending
    foreach (var state in StateMachine.StateController.States) {
      _stateMap[state.Id] = new StateInfo(state, controller);
      _stateMap[state.Id].Disconnect();
    }
    _states = _stateMap.Values.ToArray();

    controller.Animation = CreateAnimationBinding(_playableGraph, animator);
    controller.Audio = CreateAudioBinding(_playableGraph, GetAudioSource());

    if (!isView) {
      animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
    }

    Assert.IsTrue(_playableGraph.IsValid());

    return Task.CompletedTask;
  }

  // By default presimulation calls UpdateView, this should be avoided here.
  public override void Presimulate(in PlayerState state) {}

  public override void Simulate(ref PlayerState state, PlayerInputContext input) {
    state.StateTick++;
    UpdateView(state);
  }

  public override void UpdateView(in PlayerState state) {
    StateMachine.Presimulate(state);
    var stateInfo = GetControllerState(state);
    if (stateInfo == null) return;
    foreach (var controllerState in _states) {
      controllerState.Disconnect();
      controllerState.SetWeight(0f);
    }

    stateInfo.Connect();
    stateInfo.SetWeight(1f);
    stateInfo.SetTime(state);
    _playableGraph.Evaluate();
  }

  StateInfo GetControllerState(in PlayerState state) {
    StateInfo controllerState;
    if (_stateMap.TryGetValue(state.StateID, out controllerState)) {
      return controllerState;
    } else {
      return null;
    }
  }

  static AnimationMixerPlayable CreateAnimationBinding(PlayableGraph graph, Animator animator) {
    var mixerPlayable = AnimationMixerPlayable.Create(graph, 1);
    var output = (AnimationPlayableOutput)graph.GetOutputByType<AnimationPlayableOutput>(0);
    if (!output.IsOutputNull()) {
      output.SetTarget(animator);
      output.SetSourcePlayable(mixerPlayable);
      Assert.IsTrue(output.IsOutputValid());
    }
    return mixerPlayable;
  }

  static AudioMixerPlayable CreateAudioBinding(PlayableGraph graph, AudioSource audioSource) {
    var mixerPlayable = AudioMixerPlayable.Create(graph, 1);
    var output = (AudioPlayableOutput)graph.GetOutputByType<AudioPlayableOutput>(0);
    if (!output.IsOutputNull()) {
      output.SetTarget(audioSource);
      output.SetSourcePlayable(mixerPlayable);
      Assert.IsTrue(output.IsOutputValid());
    }
    return mixerPlayable;
  }

  void OptimizeHierarchy(Animator animator) {
    foreach (var animators in ObjectUtil.GetAll<Animator>(this)) {
      var root = animator.gameObject;
      var transforms = ObjectUtil.GetAll<Component>(root)
                           .Where(comp => !(comp is Transform))
                           .Select(comp => comp.name)
                           .Distinct()
                           .ToArray();
      AnimatorUtility.OptimizeTransformHierarchy(root, transforms);
    }
  }

  AudioSource GetAudioSource() {
    var source = ObjectUtil.GetFirst<AudioSource>(this);
    if (source == null) { 
      source = new GameObject(gameObject.name + "_Audio").AddComponent<AudioSource>();
      source.transform.parent = transform;
      source.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }
    return source;
  }

}

}

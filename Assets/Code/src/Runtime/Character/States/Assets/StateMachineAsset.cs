using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo.Characters {

/// <summary>
/// The serializable asset representation of a Character's 
/// </summary>
public class StateMachineAsset : BaseStateMachineAsset {

  [SerializeField] List<BaseStateAsset> _states;
  [SerializeField] List<StateTransitionAsset> _transitions;
  StateMachineMetadata _metadata;

  public ReadOnlyCollection<BaseStateAsset> States => new ReadOnlyCollection<BaseStateAsset>(_states);
  public StateMachineMetadata Metadata => _metadata ?? (_metadata = GetMetadataAsset());

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => Initialize();

  /// <inheritdoc/>
  public override StateController BuildController() {
    Initialize();
    var builder = new StateControllerBuilder();
    var stateMap = BuildStates(builder, _states);
    BuildTransitions(_transitions, stateMap);
    return builder.Build();
  }

  /// <summary>
  /// Gets all transitions related to a state.
  /// </summary>
  /// <param name="source">the source state to get the transitions for</param>
  /// <returns>an enumeration of all transitions to be made.</returns>
  public IEnumerable<StateTransitionAsset> GetTransitions(StateAsset source) {
    Initialize();
    if (source == null) {
      return Enumerable.Empty<StateTransitionAsset>();
    }
    return _transitions.Where(t => t.SourceState == source);
  }

  static Dictionary<StateAsset, State> BuildStates(StateControllerBuilder builder,
                                                   List<BaseStateAsset> stateAssets) {
    var stateMap = new Dictionary<StateAsset, State>();
    uint id = 0;
    foreach (var baseStateAsset in stateAssets) {
      if (baseStateAsset == null) continue;
      foreach (var stateAsset in baseStateAsset.GetBaseStates()) {
        if (stateAsset == null || stateMap.ContainsKey(stateAsset)) continue;
        State state = stateAsset.BuildState(id);
        builder.AddState(state);
        stateMap[stateAsset] = state;
        id++;
      }
    }
    return stateMap;
  }

  static void BuildTransitions(List<StateTransitionAsset> transitionAssets,
                               Dictionary<StateAsset, State> stateMap) {
    foreach (var transitionAsset in transitionAssets) {
      if (transitionAsset == null || 
          transitionAsset.SourceState == null ||
          transitionAsset.DestinationState == null) continue;
      var sourceStates = transitionAsset.SourceState.GetBaseStates();
      var destStates = transitionAsset.DestinationState.GetBaseStates();
      foreach (var destState in destStates) {
        State srcState, dstState;
        if (destState == null || !stateMap.TryGetValue(destState, out dstState)) continue;
        var transition = transitionAsset.BuildTransition(dstState);
        if (transition == null) continue;
        foreach (var sourceState in sourceStates) {
          if (sourceState == null || !stateMap.TryGetValue(sourceState, out srcState)) continue;
          srcState.AddTransition(transition);
        }
      }
    }
  }

  void Initialize() {
    _states = _states ?? new List<BaseStateAsset>();
    _transitions = _transitions ?? new List<StateTransitionAsset>();
  }

#if UNITY_EDITOR

  /// <summary>
  /// Returns state machine asset
  /// </summary>
  /// <returns></returns>
  public static StateMachineAsset GetStateMachineAsset() {
    var myInstance = (StateMachineAsset)Resources.Load("StateMachine/StateMachine") as StateMachineAsset;
    if (myInstance == null) {
      myInstance = CreateInstance<StateMachineAsset>();
      AssetDatabase.CreateAsset(myInstance, "Assets/Resources/StateMachine/StateMachine.asset");
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }
    return myInstance;
  }

  /// <summary>
  /// Returns metadata to build editor window
  /// </summary>
  /// <returns></returns>
  public StateMachineMetadata GetMetadataAsset() {
    var path = AssetDatabase.GetAssetPath(this);
    if (string.IsNullOrEmpty(path)) {
      throw new InvalidOperationException("State Controller is not a saved asset. Cannot create meta data.");
    }
    var asset = AssetDatabase.LoadAllAssetsAtPath(path).OfType<StateMachineMetadata>().FirstOrDefault();
    if (asset == null) {
      asset = StateMachineMetadata.Create();
      AssetDatabase.AddObjectToAsset(asset, path);
    }
    return asset;
  }

  /// <summary>
  /// Creates a StateAsset and adds it to the state machine's list of states.
  /// </summary>
  /// <remarks>
  /// This will save any unsaved changes with the state machine.
  /// </remarks>
  /// <param name="name">the name of the state</param>
  /// <returns>the created state asset</returns>
  public T CreateState<T>(string name) where T : BaseStateAsset {
    Initialize();
    string path = AssetDatabase.GetAssetPath(this);
    if (string.IsNullOrEmpty(path)) {
      throw new InvalidOperationException("State Controller is not a saved asset. Cannot create state.");
    }
    var state = BaseStateAsset.Create<T>();
    AppendSubAsset(path, state, _states);
    return state;
  }

  /// <summary>
  /// Creates a StateTransitionAsset and adds it to the state machine's set of transitions.
  /// </summary>
  /// <remarks>
  /// This will save any unsaved changes with the state machine to disk.
  /// </remarks>
  /// <param name="src">the source state for the transition</param>
  /// <param name="dst">the target state for the transition</param>
  /// <returns>the created transition asset</returns>
  public StateTransitionAsset CreateTransition(StateAsset src, StateAsset dst) {
    Initialize();
    string path = AssetDatabase.GetAssetPath(this);
    if (string.IsNullOrEmpty(path)) {
      throw new InvalidOperationException("State Controller is not a saved asset. Cannot create transition.");
    }
    var transition = StateTransitionAsset.Create(src, dst);
    AppendSubAsset(path, transition, _transitions);
    return transition;
  }

  /// <summary>
  /// Removes a state fro the state machine.
  /// </summary>
  /// <remarks>
  /// This will also destroy all transitions related to the state (i.e. transitions starting from or ending 
  /// with the state).
  /// 
  /// Does nothing if the state is null.
  /// This will save any unsaved changes with the state machine to disk.
  /// </remarks>
  /// <param name="state">the state to remove</param>
  public void RemoveState(StateAsset state) {
    Initialize();
    if (state == null) return;
    _states.RemoveAll(s => s == state);

    var invalidTransitions = _transitions.Where(t => t.Involves(state)).ToArray();
    _transitions.RemoveAll(t => invalidTransitions.Contains(t));
    foreach (var transition in invalidTransitions) {
      Object.DestroyImmediate(transition, true);
    }

    Object.DestroyImmediate(state, true);
    SaveAsset();
  }

  /// <summary>
  /// Removes a transition from the state machine.
  /// </summary>
  /// <remarks>
  /// Does nothing if the transition is null.
  /// This will save any unsaved changes with the state machine to disk.
  /// </remarks>
  /// <param name="transition">the transition to remove.</param>
  public void RemoveTransition(StateTransitionAsset transition) {
    if (transition == null) return;
    _transitions.Remove(transition);
    Object.DestroyImmediate(transition, true);
    SaveAsset();
  }

  void AppendSubAsset<T>(string path, T subasset, List<T> collection) where T : Object {
    AssetDatabase.AddObjectToAsset(subasset, path);
    collection.Add(subasset);
    SaveAsset();
  }

  void SaveAsset() {
    EditorUtility.SetDirty(this);

    string path = AssetDatabase.GetAssetPath(this);
    if (string.IsNullOrEmpty(path)) return;
    AssetDatabase.ImportAsset(path);
  }
#endif
}

}
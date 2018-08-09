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
/// The serializable asset representation of a State Controller.
/// </summary>
public class StateMachineAsset : BaseStateMachineAsset {

  [SerializeField] List<BaseStateAsset> _states;

    // TODO(james7132): Move this so StateMachineAsset doesn't have any reference to the metadata.
  StateMachineMetadata _metadata;

  /// <summary>
  /// Gets a read-only collection of all states within the state machine.
  /// </summary>
  public ReadOnlyCollection<BaseStateAsset> States => new ReadOnlyCollection<BaseStateAsset>(_states);
  
  /// <summary>
  /// Gets an enumeration of all transitions within the state machine.
  /// </summary>
  public IEnumerable<StateTransitionAsset> Transitions => _states.SelectMany(s => s.Transitions);

#if UNITY_EDITOR
  public StateMachineMetadata Metadata => _metadata ?? (_metadata = GetMetadataAsset());
#endif

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => Initialize();

  /// <inheritdoc/>
  public override StateController BuildController() {
    Initialize();
    var builder = new StateControllerBuilder();
    var stateMap = BuildStates(builder, _states);
    BuildTransitions(Transitions, stateMap);
    return builder.Build();
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

  static void BuildTransitions(IEnumerable<StateTransitionAsset> transitions,
                               Dictionary<StateAsset, State> stateMap) {
    foreach (var transitionAsset in transitions) {
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
    var state = BaseStateAsset.Create<T>(this);
    _states.Add(state);
#if UNITY_EDITOR
    AppendSubAsset(state);
#endif
    return state;
  }

  /// <summary>
  /// Removes a state from the state machine.
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
    if (state == null || !_states.Contains(state)) return;
    _states.RemoveAll(s => s == state);

    foreach (var src in _states) {
      src.RemoveTransitionsTo(state);
    }

    state.Destroy();
#if UNITY_EDITOR
    SaveAsset();
#endif
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
    var asset = AssetDatabase.LoadAllAssetsAtPath(GetAssetPath()).OfType<StateMachineMetadata>().FirstOrDefault();
    if (asset == null) {
      asset = StateMachineMetadata.Create();
      AppendSubAsset(asset);
    }
    return asset;
  }

  public void AppendSubAsset<T>(T subasset) where T : Object {
    if (EditorApplication.isPlayingOrWillChangePlaymode) return;
    AssetDatabase.AddObjectToAsset(subasset, GetAssetPath());
    SaveAsset();
  }

  string GetAssetPath() {
    var path = AssetDatabase.GetAssetPath(this);
    if (string.IsNullOrEmpty(path)) {
      throw new InvalidOperationException("State Controller is not a saved asset. Cannot create state.");
    }
    return path;
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
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

  [SerializeField] List<StateAsset> _states;
  [SerializeField] List<StateTransitionAsset> _transitions;

  public ReadOnlyCollection<StateAsset> States => new ReadOnlyCollection<StateAsset>(_states);

  #if UNITY_EDITOR
  // Editor window purposes
  int idCounter = 0;
  Dictionary<int, StateMachineNode> _nodeDictionary;
  List<StateMachineNode> _stateNodes;
  List<int> idDisposeStack;

  public ReadOnlyDictionary<int, StateMachineNode> NodeDictionary => new ReadOnlyDictionary<int, StateMachineNode>(_nodeDictionary);
  public ReadOnlyCollection<StateMachineNode> StateNodes => new ReadOnlyCollection<StateMachineNode>(_stateNodes);
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
                                                   List<StateAsset> stateAssets) {
    var stateMap = new Dictionary<StateAsset, State>();
    uint id = 0;
    foreach (var stateAsset in stateAssets) {
      if (stateAsset == null) continue;
      State state = stateAsset.BuildState(id);
      builder.AddState(state);
      stateMap[stateAsset] = state;
      id++;
    }
    return stateMap;
  }

  static void BuildTransitions(List<StateTransitionAsset> transitionAssets,
                               Dictionary<StateAsset, State> stateMap) {
    foreach (var transitionAsset in transitionAssets) {
      if (transitionAsset == null || 
          transitionAsset.SourceState == null ||
          transitionAsset.DestinationState == null) continue;
      State srcState, dstState;
      if (stateMap.TryGetValue(transitionAsset.SourceState, out srcState) &&
          stateMap.TryGetValue(transitionAsset.DestinationState, out dstState)) {
        var transition = transitionAsset.BuildTransition(dstState);
        if (transition == null) continue;
        srcState.AddTransition(transition);
      }
    }
  }

  void Initialize() {
    _states = _states ?? new List<StateAsset>();
    _transitions = _transitions ?? new List<StateTransitionAsset>();

    #if UNITY_EDITOR
    if (_nodeDictionary == null) {
      _nodeDictionary = new Dictionary<int, StateMachineNode>(StateNodes.Count);
      foreach (var item in StateNodes) _nodeDictionary.Add(item.Id, item);
    }

    _stateNodes = _stateNodes ?? new List<StateMachineNode>();
    idDisposeStack = idDisposeStack ?? new List<int>();
  #endif
  }

#if UNITY_EDITOR

    #region EditorWindow

    /// <summary>
    /// Used to save the editor window's state between sessions.
    /// </summary>
    [Serializable]
    public class StateMachineNode {
      public int Id;
      public Rect Window = Rect.zero;

      public StateAsset Asset;
      public List<int> DestinationIds;

      public Vector2 GetCenter => Window.center;
      public Vector2 GetDrawLineEnd(Vector2 Direction) => Window.center + (Vector2)Vector3.Cross(Direction, Vector3.forward) * 5;
      public Vector2 GetDrawLineArrowEnd(Vector2 Direction, Vector2 LineCenter, Vector3 CrossVector) => LineCenter - (5 * Direction) + (Vector2)Vector3.Cross(Direction, CrossVector) * 5;

      public StateMachineNode(int id, StateAsset asset) {
        Id = id;
        Asset = asset;
        DestinationIds = new List<int>();
      }
    }

    public void AddNode() {
      var ID = GetNodeID();
      var asset = StateAsset.Create(ID.ToString());
      var temp = new StateMachineNode(ID, asset);

      _states.Add(asset);
      _stateNodes.Add(temp);
      _nodeDictionary.Add(ID, temp);
    }

    public void AddTransitionNode(StateMachineNode source, StateMachineNode destination){
      var asset = StateTransitionAsset.Create(source.Asset, destination.Asset);

      _transitions.Add(asset);
      source.DestinationIds.Add(destination.Id);
    }

    public bool TransitionNodeExists(StateMachineNode source, StateMachineNode destination)
      => GetTransitions(source.Asset).Any(t => t.DestinationState == destination.Asset); 

    private int GetNodeID() => idDisposeStack.Count > 0 ? idDisposeStack[idDisposeStack.Count - 1] : idCounter++;

    /// <summary>
    /// Returns state machine asset to build editor window
    /// </summary>
    /// <returns></returns>
    public static StateMachineAsset GetStateMachineAsset() {
      var myInstance = (StateMachineAsset)Resources.Load("StateMachineBuilder/StateMachine") as StateMachineAsset;
      if (myInstance == null) {
        myInstance = CreateInstance<StateMachineAsset>();
        AssetDatabase.CreateAsset(myInstance, "Assets/Resources/StateMachineBuilder/StateMachine.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
      }
      return myInstance;
    }

    #endregion

  /// <summary>
  /// Creates a StateAsset and adds it to the state machine's list of states.
  /// </summary>
  /// <remarks>
  /// This will save any unsaved changes with the state machine.
  /// </remarks>
  /// <param name="name">the name of the state</param>
  /// <returns>the created state asset</returns>
  public StateAsset CreateState(string name) {
    Initialize();
    string path = AssetDatabase.GetAssetPath(this);
    if (string.IsNullOrEmpty(path)) {
      throw new InvalidOperationException("State Controller is not a saved asset. Cannot create state.");
    }
    var state = StateAsset.Create();
    AssetDatabase.AddObjectToAsset(state, path);
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
      Object.DestroyImmediate(transition);
    }

    Object.DestroyImmediate(state);
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
    Object.DestroyImmediate(transition);
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
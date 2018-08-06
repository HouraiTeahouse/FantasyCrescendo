using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateMachineMetadata : ScriptableObject {

  [SerializeField] int idCounter = 0;

  Dictionary<int, StateMachineNode> _nodeDictionary;
  [SerializeField] List<StateMachineNode> _stateNodes;
  [SerializeField] List<int> _idDisposeStack;

  public StateMachineAsset _stateMachine;

  public ReadOnlyDictionary<int, StateMachineNode> NodeDictionary => new ReadOnlyDictionary<int, StateMachineNode>(_nodeDictionary);
  public ReadOnlyCollection<StateMachineNode> StateNodes => new ReadOnlyCollection<StateMachineNode>(_stateNodes);

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => Initialize();


  void Initialize() {
    _stateMachine = _stateMachine ?? StateMachineAsset.GetStateMachineAsset();
    _stateNodes = _stateNodes ?? new List<StateMachineNode>();
    _idDisposeStack = _idDisposeStack ?? new List<int>();
    _stateMachine = _stateMachine ?? StateMachineAsset.GetStateMachineAsset();

    if (_nodeDictionary == null) {
      _nodeDictionary = new Dictionary<int, StateMachineNode>(StateNodes.Count);
      foreach (var node in StateNodes) _nodeDictionary.Add(node.Id, node);
    }
  }

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
    var asset = _stateMachine.CreateState(ID.ToString());
    var temp = new StateMachineNode(ID, asset);

    _stateNodes.Add(temp);
    _nodeDictionary.Add(ID, temp);
  }

  public void AddTransitionNode(StateMachineNode source, StateMachineNode destination) {
    var asset = _stateMachine.CreateTransition(source.Asset, destination.Asset);

    source.DestinationIds.Add(destination.Id);
  }

  public bool TransitionNodeExists(StateMachineNode source, StateMachineNode destination)
    => _stateMachine.GetTransitions(source.Asset).Any(t => t.DestinationState == destination.Asset);

  private int GetNodeID() => _idDisposeStack.Count > 0 ? _idDisposeStack[_idDisposeStack.Count - 1] : idCounter++;

  public static StateMachineMetadata Create() {
    var meta = ScriptableObject.CreateInstance<StateMachineMetadata>();
    meta.name = "Metadata";
    meta.Initialize();
    return meta;
  }

  }
}

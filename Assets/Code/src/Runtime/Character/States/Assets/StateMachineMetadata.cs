using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateMachineMetadata : ScriptableObject {

  [SerializeField] int idCounter = 0;

  Dictionary<int, StateNode> _stateDictionary;
  [SerializeField] List<StateNode> _stateNodes;
  [SerializeField] List<TransitionNode> _transitionNodes;

  public StateMachineAsset _stateMachine;

  public ReadOnlyDictionary<int, StateNode> StateDictionary => new ReadOnlyDictionary<int, StateNode>(_stateDictionary);
  public ReadOnlyCollection<StateNode> StateNodes => new ReadOnlyCollection<StateNode>(_stateNodes);
  public ReadOnlyCollection<TransitionNode> TransitionNodes => new ReadOnlyCollection<TransitionNode>(_transitionNodes);

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => Initialize();

  void Initialize() {
    _stateMachine = _stateMachine ?? StateMachineAsset.GetStateMachineAsset();
    _stateNodes = _stateNodes ?? new List<StateNode>();
    _transitionNodes = _transitionNodes ?? new List<TransitionNode>();

    if (_stateDictionary == null) {
      _stateDictionary = new Dictionary<int, StateNode>(StateNodes.Count);
      foreach (var node in StateNodes) _stateDictionary.Add(node.Id, node);
    }
  }

  /// <summary>
  /// Used to save the editor window's state between sessions.
  /// </summary>
  [Serializable]
  public class Node<T> where T : ScriptableObject {
    public int Id => Asset.GetInstanceID();
    public T Asset;
#if UNITY_EDITOR
    public bool IsSelected => Selection.objects.Contains(Asset);
#endif
  }

  [Serializable]
  public class StateNode : Node<StateAsset> {
    public Rect Window = Rect.zero;
    public Rect PreviousWindow = Rect.zero;
    public Vector2 Center => Window.center;

    public List<int> Transitions;

    public StateNode(StateAsset asset) {
      Transitions = new List<int>();
    }
  }

  [Serializable]
  public class TransitionNode: Node<StateTransitionAsset> {
    public int SourceId => Asset.SourceState.GetInstanceID();
    public int DestinationId => Asset.DestinationState.GetInstanceID();

    public Vector2 CornerSource;
    public Vector2 CornerSourceAway;
    public Vector2 CornerDestination;
    public Vector2 CornerDestinationAway;

    public Vector2 Center;
    public Vector2 CenterSource;
    public Vector2 CenterDestination;
    
    public Vector2 ArrowLeftEnd;
    public Vector2 ArrowRightEnd;

    public float Area;

    public const float TransitionSize = 15f;
    public const float ArrowSize = 4f;

    public TransitionNode(StateTransitionAsset asset) {
      Asset = asset;
    }

    public bool Involves(int id) => SourceId == id || DestinationId == id; 

    public void UpdateVectors(StateNode src, StateNode dst, bool force = false){
      // Prevent updating every gui call if the window didn't move
      if (!force && src.PreviousWindow == src.Window && dst.PreviousWindow == dst.Window) return;

      var Direction = (dst.Center - src.Center).normalized;
      var CornerOffet = GetCornerOffset(Direction);
      CornerSource = src.Center;
      CornerSourceAway = CornerSource + CornerOffet;
      CornerDestination = dst.Center;
      CornerDestinationAway = CornerDestination + CornerOffet;

      CenterSource = GetMiddle(CornerSource, CornerSourceAway);
      CenterDestination = GetMiddle(CornerDestination, CornerDestinationAway);
      Center = GetMiddle(CenterSource, CenterDestination);

      ArrowLeftEnd = GetArrowEnd(Direction, Center, Vector3.forward);
      ArrowRightEnd = GetArrowEnd(Direction, Center, Vector3.back);

      Area = GetAreaOfRectangle(CornerSource, CornerSourceAway, CornerDestinationAway);
    }

    private Vector2 GetMiddle(Vector2 a, Vector2 b) => 0.5f * (a + b);
    private Vector2 GetCornerOffset(Vector2 Direction) 
      => (Vector2)Vector3.Cross(Direction, Vector3.forward) * TransitionSize;
    private Vector2 GetArrowEnd(Vector2 Direction, Vector2 LineCenter, Vector3 CrossVector) 
      => LineCenter - (TransitionSize * Direction) + (Vector2)Vector3.Cross(Direction, CrossVector) * ArrowSize;

    public bool Contains(Vector2 e){
      return Mathf.Approximately(Area, GetAreaOfTriangle(CornerSource, CornerSourceAway, e)
                                    + GetAreaOfTriangle(CornerSourceAway, CornerDestinationAway, e)
                                    + GetAreaOfTriangle(CornerDestinationAway, CornerDestination, e)
                                    + GetAreaOfTriangle(CornerDestination, CornerSource, e));
    }

    private float GetAreaOfTriangle(Vector2 a, Vector2 b, Vector2 c)
      => 0.5f * Mathf.Abs((a.x) * (b.y - c.y) + (b.x) * (c.y - a.y) + (c.x) * (a.y - b.y));

    private float GetAreaOfRectangle(Vector2 a, Vector2 b, Vector2 c)
        => Mathf.Sqrt(Vector2.SqrMagnitude(a - b) * Vector2.SqrMagnitude(b - c));
  }

  public StateNode FindState(StateAsset asset) => _stateNodes.FirstOrDefault(s => s.Asset == asset);
  public TransitionNode FindTransition(StateTransitionAsset asset) => _transitionNodes.FirstOrDefault(s => s.Asset == asset);

  /// <summary>
  /// Creates state editor node alongside a StateMachineAsset
  /// </summary>
  public StateNode AddStateNode() {
    var asset = _stateMachine.CreateState("State");
    var state = new StateNode(asset);

    _stateNodes.Add(state);
    _stateDictionary.Add(state.Id, state);
    return state;
  }

  /// <summary>
  /// Creates transition editor node alongside a StateTransitionAsset
  /// </summary>
  public TransitionNode AddTransitionNode(StateNode src, StateNode dst) {
    var asset = _stateMachine.CreateTransition(src.Asset, dst.Asset);
    var node = new TransitionNode(asset);
    node.UpdateVectors(src, dst, true);

    src.Transitions.Add(node.Id);
    _transitionNodes.Add(node);
    return node;
  }

  /// <summary>
  /// Removes state editor node from metadata and state machine
  /// </summary>
  /// <param name="node"></param>
  public void RemoveStateNode(StateNode node){
    if (node == null) return;
    _stateDictionary.Remove(node.Id);
    _stateNodes.Remove(node);

    var invalidTransitions = _transitionNodes.Where(t => t.Involves(node.Id)).ToArray();
    _transitionNodes.RemoveAll(t => invalidTransitions.Contains(t));
    _stateMachine.RemoveState(node.Asset);
  }

  /// <summary>
  /// Removes transition editor node from metadata and state machine
  /// </summary>
  /// <param name="node"></param>
  public void RemoveTransitionNode(TransitionNode node){
    if (node == null) return;
    StateDictionary[node.SourceId].Transitions.Remove(node.Id);
    _transitionNodes.Remove(node);
    _stateMachine.RemoveTransition(node.Asset);
  }

  /// <summary>
  /// Updates transition's vectors to drawing lines and detecting selection
  /// </summary>
  public void UpdateTransitionNodes(){
    foreach (var node in TransitionNodes) node.UpdateVectors(StateDictionary[node.SourceId], StateDictionary[node.DestinationId]);
    foreach (var node in StateNodes) node.PreviousWindow = node.Window;
  }

  /// <summary>
  /// Checks if a transition exists between two nodes
  /// </summary>
  /// <param name="src"></param>
  /// <param name="dst"></param>
  /// <returns>if transition exists</returns>
  public bool TransitionNodeExists(StateNode src, StateNode dst)
    => TransitionNodes.Any(t => t.SourceId == src.Id && t.DestinationId == dst.Id);

  public static StateMachineMetadata Create() {
    var meta = ScriptableObject.CreateInstance<StateMachineMetadata>();
    meta.name = "Metadata";
    meta.Initialize();
    return meta;
  }

  }
}

using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateMachineMetadata : ScriptableObject {

  [SerializeField] int idCounter = 0;

  Dictionary<int, StateMachineStateNode> _stateDictionary;
  [SerializeField] List<StateMachineStateNode> _stateNodes;
  [SerializeField] List<StateMachineTransitionNode> _transitionNodes;
  [SerializeField] List<int> _idDisposeStack;

  public StateMachineAsset _stateMachine;

  public ReadOnlyDictionary<int, StateMachineStateNode> StateDictionary => new ReadOnlyDictionary<int, StateMachineStateNode>(_stateDictionary);
  public ReadOnlyCollection<StateMachineStateNode> StateNodes => new ReadOnlyCollection<StateMachineStateNode>(_stateNodes);
  public ReadOnlyCollection<StateMachineTransitionNode> TransitionNodes => new ReadOnlyCollection<StateMachineTransitionNode>(_transitionNodes);

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => Initialize();

  void Initialize() {
    _stateMachine = _stateMachine ?? StateMachineAsset.GetStateMachineAsset();
    _stateNodes = _stateNodes ?? new List<StateMachineStateNode>();
    _transitionNodes = _transitionNodes ?? new List<StateMachineTransitionNode>();
    _idDisposeStack = _idDisposeStack ?? new List<int>();

    if (_stateDictionary == null) {
      _stateDictionary = new Dictionary<int, StateMachineStateNode>(StateNodes.Count);
      foreach (var node in StateNodes) _stateDictionary.Add(node.Id, node);
    }
  }

  /// <summary>
  /// Used to save the editor window's state between sessions.
  /// </summary>
  [Serializable]
  public class StateMachineNode{
    public int Id;

    public StateMachineNode(int id) {
      Id = id;
    }
  }

  [Serializable]
  public class StateMachineStateNode : StateMachineNode{
    public Rect Window = Rect.zero;
    public Rect PreviousWindow = Rect.zero;
    public Vector2 GetCenter => Window.center;

    public StateAsset Asset;
    public List<int> Transitions;

    public StateMachineStateNode(int id, StateAsset asset) : base(id){
      Asset = asset;
      Transitions = new List<int>();
    }
  }

  [Serializable]
  public class StateMachineTransitionNode: StateMachineNode{
    public int SourceId;
    public int DestinationId;
    public StateTransitionAsset Asset;

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

    public StateMachineTransitionNode(int id, StateTransitionAsset asset, int src, int dst) : base(id){
      Asset = asset;

      SourceId = src;
      DestinationId = dst;
    }

    public bool Involves(int id) => SourceId == id || DestinationId == id; 

    public void UpdateVectors(StateMachineStateNode src, StateMachineStateNode dst, bool force = false){
      // Prevent updating every gui call if the window didn't move
      if (!force && src.PreviousWindow == src.Window && dst.PreviousWindow == dst.Window) return;

      var Direction = (dst.GetCenter - src.GetCenter).normalized;
      var CornerOffet = GetCornerOffset(Direction);
      CornerSource = src.GetCenter;
      CornerSourceAway = CornerSource + CornerOffet;
      CornerDestination = dst.GetCenter;
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

  /// <summary>
  /// Creates state editor node alongside a StateMachineAsset
  /// </summary>
  public StateMachineStateNode AddStateNode() {
    var ID = GetNodeID();
    var asset = _stateMachine.CreateState(ID.ToString());
    var temp = new StateMachineStateNode(ID, asset);

    _stateNodes.Add(temp);
    _stateDictionary.Add(ID, temp);
    return temp;
  }

  /// <summary>
  /// Creates transition editor node alongside a StateTransitionAsset
  /// </summary>
  public StateMachineTransitionNode AddTransitionNode(StateMachineStateNode src, StateMachineStateNode dst) {
    var ID = GetNodeID();
    var asset = _stateMachine.CreateTransition(src.Asset, dst.Asset);
    var temp = new StateMachineTransitionNode(ID, asset, src.Id, dst.Id);
    temp.UpdateVectors(src, dst, true);

    src.Transitions.Add(ID);
    _transitionNodes.Add(temp);
    return temp;
  }

  /// <summary>
  /// Removes state editor node from metadata and state machine
  /// </summary>
  /// <param name="node"></param>
  public void RemoveStateNode(StateMachineStateNode node){
    if (node == null) return;
    _stateDictionary.Remove(node.Id);
    _stateNodes.Remove(node);

    var invalidTransitions = _transitionNodes.Where(t => t.Involves(node.Id)).ToArray();
    _transitionNodes.RemoveAll(t => invalidTransitions.Contains(t));
    foreach (var transition in invalidTransitions) {
      _idDisposeStack.Add(transition.Id);
    }
    _idDisposeStack.Add(node.Id);

    _stateMachine.RemoveState(node.Asset);
  }

  /// <summary>
  /// Removes transition editor node from metadata and state machine
  /// </summary>
  /// <param name="node"></param>
  public void RemoveTransitionNode(StateMachineTransitionNode node){
    if (node == null) return;
    StateDictionary[node.SourceId].Transitions.Remove(node.Id);
    _transitionNodes.Remove(node);
    _idDisposeStack.Add(node.Id);

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
  public bool TransitionNodeExists(StateMachineStateNode src, StateMachineStateNode dst)
    => TransitionNodes.Any(t => t.SourceId == src.Id && t.DestinationId == dst.Id);

  private int GetNodeID() {
    var ID = -1;
    if (_idDisposeStack.Count > 0){
      ID = _idDisposeStack[_idDisposeStack.Count - 1];
      _idDisposeStack.RemoveAt(_idDisposeStack.Count - 1);
    } else {
      ID = idCounter++;
    }
    return ID;
  }

  public static StateMachineMetadata Create() {
    var meta = ScriptableObject.CreateInstance<StateMachineMetadata>();
    meta.name = "Metadata";
    meta.Initialize();
    return meta;
  }

  }
}

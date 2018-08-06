using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateMachineMetadata : ScriptableObject {

  [SerializeField] int idCounter = 0;

  Dictionary<int, StateMachineNode> _stateDictionary;
  Dictionary<int, StateMachineTransitionNode> _transitionDictionary;
  [SerializeField] List<StateMachineNode> _stateNodes;
  [SerializeField] List<StateMachineTransitionNode> _transitionNodes;
  [SerializeField] List<int> _idDisposeStack;

  public StateMachineAsset _stateMachine;

  public ReadOnlyDictionary<int, StateMachineNode> StateDictionary => new ReadOnlyDictionary<int, StateMachineNode>(_stateDictionary);
  public ReadOnlyDictionary<int, StateMachineTransitionNode> TransitionDictionary => new ReadOnlyDictionary<int, StateMachineTransitionNode>(_transitionDictionary);
  public ReadOnlyCollection<StateMachineNode> StateNodes => new ReadOnlyCollection<StateMachineNode>(_stateNodes);
  public ReadOnlyCollection<StateMachineTransitionNode> TransitionNodes => new ReadOnlyCollection<StateMachineTransitionNode>(_transitionNodes);

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  void OnEnable() => Initialize();


  void Initialize() {
    _stateMachine = _stateMachine ?? StateMachineAsset.GetStateMachineAsset();
    _stateNodes = _stateNodes ?? new List<StateMachineNode>();
    _transitionNodes = _transitionNodes ?? new List<StateMachineTransitionNode>();
    _idDisposeStack = _idDisposeStack ?? new List<int>();
    _stateMachine = _stateMachine ?? StateMachineAsset.GetStateMachineAsset();

    if (_stateDictionary == null) {
      _stateDictionary = new Dictionary<int, StateMachineNode>(StateNodes.Count);
      foreach (var node in StateNodes) _stateDictionary.Add(node.Id, node);
    }
    if (_transitionDictionary == null) {
      _transitionDictionary = new Dictionary<int, StateMachineTransitionNode>(TransitionNodes.Count);
      foreach (var node in TransitionNodes) _transitionDictionary.Add(node.Id, node);
    }
  }

  /// <summary>
  /// Used to save the editor window's state between sessions.
  /// </summary>
  [Serializable]
  public class StateMachineNode {
    public int Id;
    public Rect Window = Rect.zero;
    public Rect PreviousWindow = Rect.zero;

    public StateAsset Asset;
    public List<int> Transitions;

    public Vector2 GetCenter => Window.center;

    public StateMachineNode(int id, StateAsset asset) {
      Id = id;
      Asset = asset;
      Transitions = new List<int>();
    }
  }

  [Serializable]
  public class StateMachineTransitionNode{
    public int Id;  
  
    public int SourceId;
    public int DestinationId;

    public bool Selected;

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

    public StateMachineTransitionNode(int id, StateTransitionAsset asset, int src, int dst){
      Id = id;
      Asset = asset;

      SourceId = src;
      DestinationId = dst;
    }

    public void UpdateVectors(StateMachineNode src, StateMachineNode dst){
      // Prevent updating every gui call if the window didn't move
      if (src.PreviousWindow == src.Window && dst.PreviousWindow == dst.Window) return;

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

    public Vector2 GetMiddle(Vector2 a, Vector2 b) => 0.5f * (a + b);
    public Vector2 GetCornerOffset(Vector2 Direction) 
      => (Vector2)Vector3.Cross(Direction, Vector3.forward) * TransitionSize;
    public Vector2 GetArrowEnd(Vector2 Direction, Vector2 LineCenter, Vector3 CrossVector) 
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

  public void AddNode() {
    var ID = GetNodeID();
    var asset = _stateMachine.CreateState(ID.ToString());
    var temp = new StateMachineNode(ID, asset);

    _stateNodes.Add(temp);
    _stateDictionary.Add(ID, temp);
  }

  public void AddTransitionNode(StateMachineNode src, StateMachineNode dst) {
    var ID = GetNodeID();
    var asset = _stateMachine.CreateTransition(src.Asset, dst.Asset);
    var temp = new StateMachineTransitionNode(ID, asset, src.Id, dst.Id);

    src.Transitions.Add(ID);
    _transitionNodes.Add(temp);
    _transitionDictionary.Add(ID, temp);
  }

  public void UpdateTransitionNodes(){
    foreach (var node in TransitionNodes) node.UpdateVectors(StateDictionary[node.SourceId], StateDictionary[node.DestinationId]);
    foreach (var node in StateNodes) node.PreviousWindow = node.Window;
  }

  public bool TransitionNodeExists(StateMachineNode src, StateMachineNode dst)
    => TransitionNodes.Any(t => t.SourceId == src.Id && t.DestinationId == dst.Id);

  private int GetNodeID() => _idDisposeStack.Count > 0 ? _idDisposeStack[_idDisposeStack.Count - 1] : idCounter++;

  public static StateMachineMetadata Create() {
    var meta = ScriptableObject.CreateInstance<StateMachineMetadata>();
    meta.name = "Metadata";
    meta.Initialize();
    return meta;
  }

  }
}

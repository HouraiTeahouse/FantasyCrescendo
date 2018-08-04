using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System;

namespace HouraiTeahouse.FantasyCrescendo {
  public class StateMachineAsset : ScriptableObject {
    public int IdCounter = 0;

    [System.Serializable]
    public class StateMachineNode{
      public int Id;
      public Rect Window = Rect.zero;

      public StateAsset Asset;
      public List<StateTransitionAsset> Transitions = new List<StateTransitionAsset>();
      public List<int> TransitionIds = new List<int>();

      public Vector2 GetCenter => Window.center;
      public Vector2 GetDrawLineEnd(Vector2 Direction) => Window.center + (Vector2)Vector3.Cross(Direction, Vector3.forward) * 5;
      public Vector2 GetDrawLineArrowEnd (Vector2 Direction, Vector2 LineCenter, Vector3 CrossVector) => LineCenter - (5 * Direction) + (Vector2)Vector3.Cross(Direction, CrossVector) * 5;

      public StateMachineNode(StateMachineAsset reference, int ID){
        Id = ID;

        Asset = StateAsset.Create(Id.ToString());

        reference.StateNodes.Add(this);
        reference.NodeDictionary.Add(ID, this);
      }

      public void AddTransition(StateMachineNode destinationNode) {
        Transitions.Add(StateTransitionAsset.Create(Asset, destinationNode.Asset, string.Format("{0}-{1}", Id, destinationNode.Id)));
        TransitionIds.Add(destinationNode.Id);
      }

      public void RemoveTransition(StateTransitionAsset asset, int ID) {
        Transitions.Remove(asset);
        TransitionIds.Remove(ID);
      }

      public bool TransitionExists(StateMachineNode destinationNode) {
        foreach (var transition in TransitionIds) {
          if (transition == destinationNode.Id)
            return true;
        }
        return false;
      }
    }

    // Editor window purposes
    public Dictionary<int, StateMachineNode> NodeDictionary = new Dictionary<int, StateMachineNode>();
    public List<StateMachineNode> StateNodes = new List<StateMachineNode>();

    // State Assets from Nodes
    public List<StateAsset> StateAssets => StateNodes.Select(s => s.Asset).ToList();
    public List<StateTransitionAsset> StateTransitions => StateNodes.SelectMany(s => s.Transitions).ToList();
    public List<StateMachineNode> StateTransitionNodes => StateNodes.SelectMany(s => s.TransitionIds.Select(i => NodeDictionary[i])).ToList();

    [SerializeField] private List<int> IdDisposeStack = new List<int>();

    public void OnEnable() {
      NodeDictionary = new Dictionary<int, StateMachineNode>(StateNodes.Count);
      foreach (var item in StateNodes) NodeDictionary.Add(item.Id, item);
    }

    public void AddNode(){
      var ID = GetID();
      var temp = new StateMachineNode(this, ID);
    }

    public void DeleteNode(StateMachineNode item){
      // TODO: Delete links from item

      IdDisposeStack.Add(item.Id);
    }

    private int GetID(){
      var ID = -1;
      if (IdDisposeStack.Count > 0)
        ID = IdDisposeStack[IdDisposeStack.Count - 1];
      else
        ID = IdCounter++;
      return ID;
    }

    public static StateMachineAsset GetStateMachineAsset(){
      var myInstance = (StateMachineAsset)Resources.Load("StateMachineBuilder/StateMachine") as StateMachineAsset;
      if (myInstance == null) {
        myInstance = CreateInstance<StateMachineAsset>();
        AssetDatabase.CreateAsset(myInstance, "Assets/Resources/StateMachineBuilder/StateMachine.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
      }
      return myInstance;
    }
  }

  public class StateAsset: ScriptableObject{
    public string State;

    public static StateAsset Create(string name = null) {
      var state = CreateInstance<StateAsset>();
      state.name = name ?? "State";
      AssetDatabase.CreateAsset(state, string.Format("Assets/Resources/StateMachineBuilder/Nodes/{0}.asset", state.name));
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();

      return state;
    }
  }

  public struct StateTransitionCondition{

  }

  public class StateTransitionAsset: ScriptableObject{
    public StateAsset SourceState;
    public StateAsset DestinationState;

    public List<StateTransitionCondition> Conditions = new List<StateTransitionCondition>();

    public static StateTransitionAsset Create(StateAsset Source, StateAsset Destination, string name = null) {
      var state = CreateInstance<StateTransitionAsset>();
      state.name = name ?? "Transition";
      AssetDatabase.CreateAsset(state, string.Format("Assets/Resources/StateMachineBuilder/Transitions/{0}.asset", state.name));
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();

      state.SourceState = Source;
      state.DestinationState = Destination;

      return state;
    }

    public void BuildTransition<T>(State<T> targetState){
      // TODO: Implment
    }
  }
}
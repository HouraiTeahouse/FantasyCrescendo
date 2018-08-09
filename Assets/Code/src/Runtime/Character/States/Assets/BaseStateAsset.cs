using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo.Characters {

/// <summary>
/// An asset that represents a single state or a group of states in built StateController.
/// </summary>
public abstract class BaseStateAsset : ScriptableObject {

  [SerializeField] StateMachineAsset _stateMachine;
  [SerializeField] List<StateTransitionAsset> _transitions;

  /// <summary>
  /// Gets the StateMachine that owns the state 
  /// </summary>
  public StateMachineAsset StateMachine => _stateMachine;

  /// <summary>
  /// Get a list of all transitions that originate from the state(s).
  /// 
  /// The order is the same order as they are evaluated at in the built StateController.
  /// </summary>
  /// <returns></returns>
  public ReadOnlyCollection<StateTransitionAsset> Transitions => new ReadOnlyCollection<StateTransitionAsset>(_transitions);

  /// <summary>
  /// This function is called when the object becomes enabled and active.
  /// </summary>
  protected virtual void OnEnable() => Initialize();

  /// <summary>
  /// Gets the full set of states that constitute the state.
  /// 
  /// Order not guarenteed to be deterministic.
  /// </summary>
  /// <returns>an enumeration of all the consittuent base state</returns>
  public abstract IEnumerable<StateAsset> GetBaseStates();

  internal static T Create<T>(StateMachineAsset machine, string name = null) where T : BaseStateAsset {
    var state = ScriptableObject.CreateInstance<T>();
    state.hideFlags = HideFlags.HideInHierarchy;
    state._stateMachine = machine;
    state.name = name ?? "State";
    return state;
  }

  void Initialize() {
    _transitions = _transitions ?? new List<StateTransitionAsset>();
  }

  /// <summary>
  /// Destroys the state and all of its transitions.
  /// </summary>
  public void Destroy() {
    foreach (var transition in _transitions) {
      ObjectUtil.Destroy(transition);
    }
    ObjectUtil.Destroy(this);
  }

  /// <summary>
  /// Creates a StateTransitionAsset and adds it to the state machine's set of transitions.
  /// </summary>
  /// <remarks>
  /// This will save any unsaved changes with the state machine to disk.
  /// </remarks>
  /// <param name="dst">the target state for the transition</param>
  /// <returns>the created transition asset</returns>
  public StateTransitionAsset CreateTransition(BaseStateAsset dst) {
    Initialize();
    var transition = StateTransitionAsset.Create(this, dst);
    _transitions.Add(transition);
#if UNITY_EDITOR
    StateMachine.AppendSubAsset(transition);
#endif
    return transition;
  }

  /// <summary>
  /// Removes and destroys a transition from the state.
  /// </summary>
  /// <param name="transition"></param>
  /// <returns></returns>
  public bool RemoveTransition(StateTransitionAsset transition) => RemoveTransition(t => t == transition);

  /// <summary>
  /// Removes and destroys a transition from the state.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public bool RemoveTransitionsTo(BaseStateAsset target) => RemoveTransition(t => t.DestinationState == target);

  bool RemoveTransition(Predicate<StateTransitionAsset> predicate) {
    var toRemove = new List<StateTransitionAsset>();
    foreach (var transition in _transitions) {
      if (predicate(transition)) toRemove.Add(transition);
    }
    foreach (var transition in toRemove) {
      _transitions.Remove(transition);
      ObjectUtil.Destroy(transition);
    }
    return toRemove.Count > 0;
  }

}

}
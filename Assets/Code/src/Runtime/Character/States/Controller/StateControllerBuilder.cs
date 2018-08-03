using System;
using System.Collections.Generic;
using System.Linq;

namespace HouraiTeahouse.FantasyCrescendo.Characters {

public class StateControllerBuilder {

  internal readonly HashSet<State> _states;
  public State DefaultState { get; set; }
  public IEnumerable<State> States =>  _states.Select(x => x);

  public StateControllerBuilder() {
    _states = new HashSet<State>();
  }

  public StateController Build() {
    if (DefaultState == null) {
      throw new InvalidOperationException();
    }
    if (!_states.Contains(DefaultState)) {
      throw new InvalidOperationException();
    }
    return new StateController(this);
  }

  public StateControllerBuilder WithDefaultState(State state) {
    if (state == null) {
      throw new ArgumentNullException("state");
    }
    if (!_states.Contains(state)) {
      AddState(state);
    }
    DefaultState = state;
    return this;
  }

  public StateControllerBuilder AddState(State state) {
    if (state == null) {
      throw new ArgumentNullException("state");
    }
    if (_states.Contains(state)) {
      throw new ArgumentException($"States cannot be added multiple times: {state}");
    }
    _states.Add(state);
    return this;
  }

}

}


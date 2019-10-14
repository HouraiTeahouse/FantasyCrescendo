using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace HouraiTeahouse.FantasyCrescendo {

public class CustomEventBuilder {

  readonly string EventName;
  readonly IDictionary<string, object> Parameters;

  public CustomEventBuilder(string eventName) {
    EventName = eventName;
    Parameters = new Dictionary<string, object>();
  }

  public CustomEventBuilder AddParameter(string name, object value) {
    Parameters[name] = value;
    return this;
  }

  public void Execute() { 
    if (Parameters.Count <= 0)  {
      Analytics.CustomEvent(EventName);
    } else {
      Analytics.CustomEvent(EventName, Parameters);
    }
  }

}

public class AnalyticsManager : MonoBehaviour {

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  void Awake() {
    if (!Analytics.enabled) return;
    var events = Mediator.Global.CreateUnityContext(this);
    events.Subscribe<MatchEndEvent>(OnMatchEnd);
  }

  void OnMatchEnd(MatchEndEvent evt) {
    new CustomEventBuilder("matchCompleted")
      .AddParameter("players", evt.MatchConfig.PlayerCount)
      .AddParameter("stage", evt.MatchConfig.StageID)
      .AddParameter("stocks", evt.MatchConfig.Stocks)
      .AddParameter("time", evt.MatchConfig.Time)
      .AddParameter("result", evt.MatchResult)
      .AddParameter("result", evt.MatchResult)
      .Execute();
  }

}

}
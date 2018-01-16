using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class EditorQuickMatchResults : MonoBehaviour {

  public MatchResult Results;
  public PlayerMatchResultDisplayFactory DisplayFactory;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  async void Awake() {
    await DataLoader.LoadTask.Task;
    foreach (var playerStats in Results.PlayerStats) {
      Debug.Log(playerStats);
      DisplayFactory.CreateViews(playerStats.Config).ContinueWith(task => {
        foreach (var view in task.Result) {
          view.ApplyState(playerStats);
        }
      });
    }

  }

}

}

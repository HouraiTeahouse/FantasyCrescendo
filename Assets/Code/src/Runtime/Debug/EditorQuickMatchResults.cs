using HouraiTeahouse.FantasyCrescendo.Matches;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo {

public class EditorQuickMatchResults : EditorOnlyBehaviour {

  public MatchResult Results;
  public PlayerMatchResultDisplayFactory DisplayFactory;

  /// <summary>
  /// Awake is called when the script instance is being loaded.
  /// </summary>
  protected override async void Awake() {
    base.Awake();
    await DataLoader.LoadTask.Task;
    await Task.WhenAll(Results.PlayerStats.Select(async playerStats => {
      var views = await DisplayFactory.CreateViews(playerStats.Config);
      foreach (var view in views) {
        view.UpdateView(playerStats);
      }
    }));
  }

}

}

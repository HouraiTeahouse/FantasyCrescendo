using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class DefaultMatch : Match {

  protected override async Task InitializeMatch(MatchManager gameManager, MatchConfig config) {
    var gameSim = CreateSimulation(config);
    var controller = CreateMatchController(config);

		Debug.Log($"Match Controller Type: {controller.GetType()}");

    controller.CurrentState = CreateInitialState(config);
    controller.InputSource = Config.Get<GameplayConfig>().CreateInputSource(config);
    controller.Simulation = gameSim;

    var viewTask = BuildView(config);

    gameManager.MatchController = controller;
    gameManager.View = await viewTask;

    await gameSim.Initialize(config).ContinueWith(task => {
      Debug.Log("Simulation initialized.");
    });

    controller.CurrentState = gameSim.ResetState(controller.CurrentState);
    Debug.Log("Match initialized.");
  }

  async Task<IStateView<MatchState>> BuildView(MatchConfig config) {
    var playerView = new MatchPlayerView();
    var view = new ViewBuilder<MatchState>()
      .AddSubitem(playerView)
      .AddSubitems(await CoreUtility.CreateAllViews<MatchState, MatchConfig>(config))
      .Build();
    await playerView.Initialize(config);
    Debug.Log("View initialized.");
    return view;
  }

}
		
}
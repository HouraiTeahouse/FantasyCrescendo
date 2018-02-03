using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

public class DefaultMatch : Match {

  protected override async Task InitializeMatch(MatchManager gameManager, MatchConfig config) {
    var gameView = new MatchView();
    var gameSim = CreateSimulation(config);
    var controller = CreateMatchController(config);

		Debug.Log($"Match Controller Type: {controller.GetType()}");

    controller.CurrentState = CreateInitialState(config);
    controller.InputSource = Config.Get<GameplayConfig>().CreateInputSource(config);
    controller.Simulation = gameSim;

    gameManager.MatchController = controller;
    gameManager.View = gameView;

    var simTask = gameSim.Initialize(config).ContinueWith(task => {
      Debug.Log("Simulation initialized.");
    });
    var viewTask = gameView.Initialize(config).ContinueWith(task => {
      Debug.Log("View initialized.");
    });

    await Task.WhenAll(viewTask, simTask);
    controller.CurrentState = gameSim.ResetState(controller.CurrentState);
    Debug.Log("Match initialized.");
  }

}
		
}
using HouraiTeahouse.FantasyCrescendo.Matches.Rules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace HouraiTeahouse.FantasyCrescendo.Matches {

// TODO(james7132): Refactor out parts of this class
public abstract class Match {

  public async Task<MatchResult> RunMatch(MatchConfig config, bool loadScene = true) {
    Debug.Log($"Running Match. Config: {config}");
    await DataLoader.LoadTask.Task;
    Task sceneLoad = Task.CompletedTask;
    if (loadScene) {
      var stage = Registry.Get<SceneData>().Get(config.StageID);
      Assert.IsTrue(stage != null && stage.Type == SceneType.Stage);
      await Addressables.LoadSceneAsync(stage.Scene);
    }
    var additionalScenes = Config.Get<SceneConfig>().AdditionalStageScenes;
    await Task.WhenAll(
      additionalScenes.Select(async s => await Addressables.LoadSceneAsync(s, LoadSceneMode.Additive)));
    var matchManager = MatchManager.Instance;
    matchManager.enabled = false;
    matchManager.Config = config;
    try {
      await LoadingScreen.Await(InitializeMatch(matchManager, config));
      await LoadingScreen.AwaitAll();
    } catch (Exception e) {
      Debug.LogException(e);
    }
    matchManager.enabled = true;
    return await matchManager.RunMatch();
  }

  protected virtual IEnumerable<IMatchRule> CreateRules(MatchConfig config) {
    return MatchRuleFactory.CreateRules(config);
  }

  protected IMatchSimulation CreateSimulation(MatchConfig config) {
    return new MatchSimulation(new IMatchSimulation[] { 
      new MatchPlayerSimulation(),
      new MatchHitboxSimulation(config),
      new MatchRuleSimulation(CreateRules(config))
    });
  }

	// TODO(james71323): Refactor or move this to somewhere more sane
	protected virtual IMatchController CreateMatchController(MatchConfig config) {
		return new MatchController(config);
	}

  protected abstract Task InitializeMatch(MatchManager manager, MatchConfig config);

  protected MatchState CreateInitialState(MatchConfig config) {
    var tag = Config.Get<SceneConfig>().SpawnTag;
    var startPositions = GameObject.FindGameObjectsWithTag(tag);
    MatchState state;
    if (startPositions.Length > 0) {
      state = CreateInitialStateByTransform(config, startPositions);
    } else {
      state = CreateInitialStateSimple(config);
    }
    SetDefaultDamages(config, state);
    return state;
  }

  MatchState CreateInitialStateByTransform(MatchConfig config, GameObject[] startPositions) {
    var initialState = new MatchState(config);
    startPositions = startPositions.OrderBy(s => s.transform.GetSiblingIndex()).ToArray();
    for (int i = 0; i < initialState.PlayerCount; i++) {
      var startPos = startPositions[i % startPositions.Length].transform;
      initialState[i].Position = startPos.position;
      initialState[i].Direction = startPos.transform.forward.x >= 0;
    }
    return initialState;
  }

  MatchState CreateInitialStateSimple(MatchConfig config) {
    var initialState = new MatchState(config);
    for (var i = 0; i < initialState.PlayerCount; i++) {
      initialState[i].Position = new Vector3((int)i * 2 - 3, 1, 0);
    }
    return initialState;
  }

  void SetDefaultDamages(MatchConfig config, MatchState initialState) {
    for (int i = 0; i < initialState.PlayerCount; i++) {
      initialState[i].Damage = config.PlayerConfigs[i].DefaultDamage;
    }
  }

}

}
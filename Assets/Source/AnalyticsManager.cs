using HouraiTeahouse.SmashBrew;
using HouraiTeahouse.SmashBrew.Matches;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace HouraiTeahouse.FantasyCrescendo {

    public class AnalyticsManager : MonoBehaviour {

        void Awake() {
            var mediator = Mediator.Global;
            mediator.Subscribe<PlayerDieEvent>(e => {
                CharacterData character = null;
                if (e.Player != null && e.Player.Selection.Character != null)
                    character =  e.Player.Selection.Character;
                Analytics.CustomEvent("playerDie", new Dictionary<string, object> {
                    { "character", character ? character.name : "N/A" }
                });
            });
            mediator.Subscribe<MenuOpenedEvent>(e => new Dictionary<string, object> {
                { "menuName", e.MenuName }
            });
            mediator.Subscribe<LoadSceneEvent>(e => {
                if (e.Scene == null || e.Scene.Type != SceneType.Stage)
                    return;
                Analytics.CustomEvent("stageLoaded", new Dictionary<string, object> {
                    { "stage", e.Scene.name }
                });
            });
            mediator.Subscribe<MatchStartEvent>(e => Analytics.CustomEvent("matchStarted"));
            mediator.Subscribe<MatchEndEvent>(e => {
                CharacterData winningCharacter = null;
                if (e.Winner != null)
                    winningCharacter =  e.Winner.Selection.Character;
                Analytics.CustomEvent("matchFinished", new Dictionary<string, object> {
                    { "result", e.MatchResult.ToString() },
                    { "winningCharacter", winningCharacter != null ? winningCharacter.name : "N/A" }
                });
            });
        }

    }

}


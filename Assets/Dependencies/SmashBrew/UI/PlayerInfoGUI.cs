using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Vexe.Runtime.Extensions;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew.UI {

    public class PlayerInfoGUI : BaseBehaviour {

        [SerializeField]
        private PlayerIndicator playerIndicatorPrefab;

        [SerializeField]
        private List<GameObject> _displays;

        void Awake() {
            Match.OnMatchStart += _displays.Clear;
            Match.OnSpawnCharacter += OnSpawn;
        }

        void OnDestroy() {
            Match.OnMatchStart -= _displays.Clear;
            Match.OnSpawnCharacter -= OnSpawn;
        }

        void OnSpawn(CharacterMatchData data) {
            if (data == null || _displays.Count <= 0)
                return;

            GameObject display = null;
            while (_displays.Count > 0 && display == null) {
                display = _displays[0];
                _displays.RemoveAt(0);
            }

            if (display == null)
                return;

            if (playerIndicatorPrefab != null) {
                PlayerIndicator indicator = playerIndicatorPrefab.InstantiateNew();
                indicator.Target = data.SpawnedInstance;
            }

            _displays.Remove(display);
            display.SetActive(true);

            // Update the display with the Character information
            foreach (ICharacterGUIComponent component in display.GetComponentsInChildren<ICharacterGUIComponent>())
                component.SetCharacterData(data);

            display.name = "Player " + (data.PlayerNumber + 1) + " Display";
        }

    }

}


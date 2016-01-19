using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Hourai.SmashBrew.UI {

    public interface ICharacterGUIComponent {

        void SetCharacter(CharacterData character);

    }

    public class CharacterSelectDisplay : MonoBehaviour {

        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private RectTransform _character;

        /// <summary>
        /// 
        /// </summary>
        void Start() {
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null || !_container || !_character)
                return;

            foreach (CharacterData data in dataManager.Characters) {
                if (data == null || !data.IsVisible)
                    return;
                RectTransform character = Instantiate(_character);
                character.SetParent(_container, false);
                character.name = data.name;
                LayoutRebuilder.MarkLayoutForRebuild(character);
                foreach(ICharacterGUIComponent guiComponent in character.GetComponentsInChildren<ICharacterGUIComponent>())
                    guiComponent.SetCharacter(data);
            }

        }

    }

}
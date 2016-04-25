using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// Constructs the player section of the in-match UI 
    /// </summary>
    public sealed class CharacterSelectBuilder : Builder {
        [Header("Character Select")] [SerializeField] private RectTransform _characterContainer;

        [SerializeField] private RectTransform _character;
        /// <summary>
        /// Construct the select area for characters.
        /// </summary>
        public override void CreateSelect ()
        {
            DataManager dataManager = DataManager.Instance;
            if (dataManager == null || !_characterContainer || !_character)
                return;

            foreach (CharacterData data in dataManager.Characters) {
                if (data == null || !data.IsVisible)
                    continue;
                RectTransform character = Instantiate(_character);
                Attach(character, _characterContainer);
                character.name = data.name;
                character.GetComponentsInChildren<IDataComponent<CharacterData>>().SetData(data);
            }
        }
    }
}

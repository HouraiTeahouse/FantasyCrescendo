using UnityEngine;
using HouraiTeahouse.Localization;

namespace HouraiTeahouse.SmashBrew.UI {
    /// <summary>
    /// A Component that displays a Character (or a Player's Character) name on a UI Text object
    /// </summary>
    public sealed class SceneName : AbstractLocalizedText, IDataComponent<SceneData> {
        [SerializeField, Tooltip("The Scene who's name is to be displayed")] private SceneData _scene;

        [SerializeField, Tooltip("Capitalize the Scene's name?")] private bool _capitalize;

        /// <summary>
        /// <see cref="IDataComponent{CharacterData}.SetData"/>
        /// </summary>
        public void SetData(SceneData data) {
            Text.text = data == null ? string.Empty : data.Name;
        }

        /// <summary>
        /// <see cref="AbstractLocalizedText.Process"/>
        /// </summary>
        protected override string Process(string val) {
            return !_capitalize ? val : val.ToUpperInvariant();
        }
    }
}

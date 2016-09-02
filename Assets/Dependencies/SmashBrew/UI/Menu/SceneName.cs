using HouraiTeahouse.Localization;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary> A Component that displays a Character (or a Player's Character) name on a UI Text object </summary>
    public sealed class SceneName : AbstractLocalizedText, IDataComponent<SceneData> {

        [SerializeField]
        [Tooltip("Capitalize the Scene's name?")]
        bool _capitalize;

        [SerializeField]
        [Tooltip("The Scene who's name is to be displayed")]
        SceneData _scene;

        /// <summary>
        ///     <see cref="IDataComponent{CharacterData}.SetData" />
        /// </summary>
        public void SetData(SceneData data) { Text.text = data == null ? string.Empty : data.Name; }

        /// <summary>
        ///     <see cref="AbstractLocalizedText.Process" />
        /// </summary>
        protected override string Process(string val) { return !_capitalize ? val : val.ToUpperInvariant(); }

    }

}
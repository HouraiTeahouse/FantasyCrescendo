using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    /// <summary>
    /// A CharacterUIComponent that displays the Character's UI icon
    /// on an Image. 
    /// </summary>
    public sealed class IconDisplay : CharacterUIComponent<Image> {

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component == null || data == null)
                return;
            Component.sprite = data.Icon.Load();
        }

    }

}


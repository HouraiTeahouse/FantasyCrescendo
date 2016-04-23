using UnityEngine;
using HouraiTeahouse.SmashBrew.UI;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew {
    /// <summary>
    /// A CharacterUIComponent that creates a playable AudioSOurce that will play the Character's
    /// announcer audio clip
    /// </summary>
    public sealed class AnnouncerAudio : CharacterUIComponent<AudioSource>, ISubmitHandler {
        /// <summary>
        /// <see cref="ISubmitHandler.OnSubmit"/>
        /// </summary>
        public void OnSubmit(BaseEventData eventData) {
            if (Component)
                Component.Play();
        }

        /// <summary>
        /// <see cref="IDataComponent{T}.SetData"/>
        /// </summary>
        public override void SetData(CharacterData data) {
            base.SetData(data);
            if (Component == null || data == null)
                return;
            data.Announcer.LoadAsync(clip => Component.clip = clip);
        }
    }
}

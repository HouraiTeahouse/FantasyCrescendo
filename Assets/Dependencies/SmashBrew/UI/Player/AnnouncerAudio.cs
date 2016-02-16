using UnityEngine;
using HouraiTeahouse.SmashBrew.UI;
using UnityEngine.EventSystems;

namespace HouraiTeahouse.SmashBrew {

    public class AnnouncerAudio : CharacterUIComponent<AudioSource>, ISubmitHandler {

        public void OnSubmit(BaseEventData eventData) {
            if(Component)
                Component.Play();
        }

        public override void SetCharacter(CharacterData data) {
            base.SetCharacter(data);
            if (Component == null || data == null)
                return;
            Component.clip = data.Announcer.Load();
        }
    }
}

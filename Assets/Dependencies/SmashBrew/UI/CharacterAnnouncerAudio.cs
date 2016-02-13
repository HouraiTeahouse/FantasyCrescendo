using UnityEngine;
using System.Collections;
using Hourai.SmashBrew.UI;
using UnityEngine.EventSystems;

namespace Hourai.SmashBrew {

    public class CharacterAnnouncerAudio : MonoBehaviour, ISubmitHandler, ICharacterGUIComponent {

        [SerializeField]
        private AudioSource _announcer;

        void Reset() {
            if (!_announcer)
                _announcer = GetComponentInParent<AudioSource>();
        }

        public void OnSubmit(BaseEventData eventData) {
            if(_announcer)
                _announcer.Play();
        }

        public void SetCharacter(CharacterData character) {
            if (character && _announcer)
                _announcer.clip = character.Announcer.Load();
        }
    }
}

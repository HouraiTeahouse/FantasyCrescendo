using UnityEngine;
using Hourai.SmashBrew.UI;
using UnityEngine.EventSystems;

namespace Hourai.SmashBrew {

    public class CharacterAnnouncerAudio : MonoBehaviour, ISubmitHandler, ICharacterGUIComponent {

        [SerializeField]
        private CharacterData _character;

        [SerializeField]
        private AudioSource _announcer;

        /// <summary>
        /// Unity Callback. Called on object instantiation.
        /// </summary>
        void Awake() {
            SetCharacter(_character);
        }

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

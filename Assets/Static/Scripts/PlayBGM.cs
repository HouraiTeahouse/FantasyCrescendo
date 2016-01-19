using UnityEngine;

namespace Hourai {

    [RequireComponent(typeof(AudioSource))]
    public class PlayBGM : MonoBehaviour {

        [SerializeField]
        private BGMGroup group;

        void Start() {
            if (!group) {
                Destroy(this);
                return;
            }
            var effect = gameObject.AddComponent<SoundEffect>();
            effect.hideFlags = HideFlags.HideInInspector;
            group.PlayRandom(GetComponent<AudioSource>());            
        }

        void FixedUpdate() {
            group.HandleLooping(GetComponent<AudioSource>());
        }

    }

}
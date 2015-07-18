using UnityEngine;

namespace Genso.API {


    public class CharacterDeath : CharacterComponent {


        [SerializeField]
        private ParticleSystem deathPrefab;

        private CharacterPhysics _physics;

        protected override void Awake() {
            base.Awake();
            _physics = GetComponentInParent<CharacterPhysics>();
        }

        public override void OnBlastZoneExit() {

            Vector3 position = Character.transform.position;

            if (deathPrefab != null) {
                ParticleSystem copy = deathPrefab.Copy(position);
                copy.transform.LookAt(transform.position - position);
                copy.startColor = Character.PlayerColor;
            }



        }

    }

}

﻿using UnityEngine;

namespace Hourai.SmashBrew {

    [DisallowMultipleComponent]
    public class CharacterDeath : CharacterComponent {

        [SerializeField]
        private ParticleSystem deathPrefab;

        protected override void Start() {
            base.Start();
            if (Character == null)
                return;

            // Subscribe to Character events
            Character.OnBlastZoneExit += OnBlastZoneExit;
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            if (Character != null)
                Character.OnBlastZoneExit += OnBlastZoneExit;
        }

        private void OnBlastZoneExit() {
            Vector3 position = Character.transform.position;

            if (!deathPrefab)
                return;

            ParticleSystem copy = Instantiate(deathPrefab);
            copy.transform.position = position;
            copy.transform.LookAt(transform.position - position);
            copy.startColor = Character.Player.Color;
        }

    }

}
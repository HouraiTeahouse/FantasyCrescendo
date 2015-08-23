using System;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Hourai.SmashBrew {

    public class CharacterData : BaseScriptableObject {

        [SerializeField]
        private Alternative[] alternatives;

        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private string announcerKey;

        [SerializeField]
        private string _firstNameKey;

        [SerializeField]
        private string _lastNameKey;

        public string FirstName {
            get { return _firstNameKey; }
        }

        public string LastName {
            get { return _lastNameKey; }
        }

        public string Name {
            get { return FirstName + " " + LastName; }
        }

        public int AlternativeCount {
            get { return alternatives == null ? 0 : alternatives.Length; }
        }

        public Sprite Icon {
            get { return icon; }
        }

        public Sprite LoadPortrait(int alternativeChoice) {
            if (alternativeChoice < 0 || alternativeChoice >= AlternativeCount)
                throw new ArgumentException();
            return alternatives[alternativeChoice].Portrait;
        }

        public Character LoadPrefab(int alternativeChoice) {
            if (alternativeChoice < 0 || alternativeChoice >= AlternativeCount)
                throw new ArgumentException();
            return alternatives[alternativeChoice].Prefab;
        }

        private void OnEnable() {
            if (alternatives == null)
                return;
            foreach (Alternative alternative in alternatives)
                alternative.Initialize();
        }

        [Serializable]
        public class Alternative {
      

            [SerializeField, ResourcePath(typeof (GameObject))]
            private string _prefab;
            private Resource<GameObject> _prefabResource;

            [SerializeField]
            private Sprite _portrait;

            public Sprite Portrait {
                get { return _portrait;  }
            }

            public Character Prefab {
                get { return _prefabResource.Load().GetComponent<Character>(); }
            }

            public void Initialize() {
                _prefabResource = new Resource<GameObject>(_prefab);
            }

        }

    }

}
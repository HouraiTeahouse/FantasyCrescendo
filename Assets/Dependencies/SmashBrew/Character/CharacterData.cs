using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class CharacterData : ScriptableObject {

        [SerializeField]
        private string _firstNameKey;

        [SerializeField]
        private string _lastNameKey;

        [SerializeField]
        private Alternative[] alternatives;

        [SerializeField, Resource(typeof(Sprite))]
        private string _icon;

        private Resource<Sprite> _iconResource; 

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

        public Resource<Sprite> Icon {
            get { return _iconResource; }
        }

        public Sprite LoadPortrait(int alternativeChoice) {
            if (alternativeChoice < 0 || alternativeChoice >= AlternativeCount)
                throw new ArgumentException();
            return alternatives[alternativeChoice].Portrait.Load();
        }

        public Character LoadPrefab(int alternativeChoice) {
            if (alternativeChoice < 0 || alternativeChoice >= AlternativeCount)
                throw new ArgumentException();
            return alternatives[alternativeChoice].Prefab;
        }

        private void OnEnable() {
            if (alternatives == null)
                return;
            _iconResource = new Resource<Sprite>(_icon);
            foreach (Alternative alternative in alternatives)
                alternative.Initialize();
        }

        [Serializable]
        public class Alternative {
      
            [SerializeField, Resource(typeof(GameObject))]
            private string _prefab;
            private Resource<GameObject> _prefabResource;

            [SerializeField, Resource(typeof(Sprite))]
            private string _portrait;
            private Resource<Sprite> _portraitResource; 

            public Resource<Sprite> Portrait {
                get { return _portraitResource;  }
            }

            public Character Prefab {
                get { return _prefabResource.Load().GetComponent<Character>(); }
            }

            public void Initialize() {
                _prefabResource = new Resource<GameObject>(_prefab);
                _portraitResource = new Resource<Sprite>(_portrait);
            }

        }

    }

}
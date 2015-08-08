using System;
using SmartLocalization;
using UnityEngine;
using Vexe.Runtime.Types;

namespace Crescendo.API {

    public class CharacterData : BaseScriptableObject {

        private LanguageManager _languageManager;
        
        [SerializeField]
        private Alternative[] alternatives;

        [SerializeField]
        private string announcerKey;

        [SerializeField]
        private string firstNameKey;

        [SerializeField]
        private string lastNameKey;

        public string FirstName {
            get { return _languageManager.GetTextValue(firstNameKey); }
        }

        public string LastName {
            get { return _languageManager.GetTextValue(firstNameKey); }
        }

        public string Name {
            get { return FirstName + " " + LastName; }
        }

        public int AlternativeCount {
            get { return alternatives == null ? 0 : alternatives.Length; }
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
            if (!Application.isEditor || Application.isPlaying)
                _languageManager = LanguageManager.Instance;
            if (alternatives == null)
                return;
            foreach (var alternative in alternatives)
                alternative.Initialize();
        }

        [Serializable]
        public class Alternative {

            private Resource<Sprite> _portrait;

            [SerializeField, ResourcePath(typeof(GameObject))]
            private string _prefab;

            private Resource<GameObject> _prefabResource;

            [SerializeField, ResourcePath]
            private string portrait;

            public Resource<Sprite> Portrait {
                get {
                    if (_portrait == null)
                        _portrait = new Resource<Sprite>(portrait);
                    return _portrait;
                }
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
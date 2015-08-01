using System;
using UnityEngine;
using System.Collections;
using SmartLocalization;

namespace Crescendo.API {

    public class CharacterData : ScriptableObject {

        private LanguageManager _languageManager;

        [System.Serializable]
        public class Alternative {


            [SerializeField, ResourcePath(typeof(Sprite))]
            private string portrait;
            private Resource<Sprite> _portrait;
            public Resource<Sprite> Portrait
            {
                get
                {
                    if (_portrait == null)
                        _portrait = new Resource<Sprite>(portrait);
                    return _portrait;
                }
            }

            [SerializeField, ResourcePath(typeof(GameObject))]
            private string _prefab;
            private Resource<GameObject> _prefabResource;

            public Character Prefab
            {
                get
                {
                    return _prefabResource.Load().GetComponent<Character>();
                }
            }

            public void Initialize() {
                _prefabResource = new Resource<GameObject>(_prefab);
            }

        }

        [SerializeField]
        private string firstNameKey;

        public string FirstName {
            get { return _languageManager.GetTextValue(firstNameKey); }
        }

        [SerializeField]
        private string lastNameKey;

        public string LastName {
            get { return _languageManager.GetTextValue(firstNameKey); }
        }

        public string Name {
            get { return FirstName + " " + LastName; }
        }

        public int AlternativeCount {
            get {
                return alternatives == null ? 0 : alternatives.Length;
            }
        }

        [SerializeField]
        private string announcerKey;

        [SerializeField]
        private Alternative[] alternatives;

        public Sprite LoadPortrait(int alternativeChoice) {
            if(alternativeChoice < 0 || alternativeChoice >= AlternativeCount)
                throw new ArgumentException();
            return alternatives[alternativeChoice].Portrait.Load();
        }

        public Character LoadPrefab(int alternativeChoice) {
            if (alternativeChoice < 0 || alternativeChoice >= AlternativeCount)
                throw new ArgumentException();
            return alternatives[alternativeChoice].Prefab;
        }

        void OnEnable() {
            if(!Application.isEditor || Application.isPlaying)
                _languageManager = LanguageManager.Instance;
            foreach(var alternative in alternatives)
                alternative.Initialize();
        }

    }

}


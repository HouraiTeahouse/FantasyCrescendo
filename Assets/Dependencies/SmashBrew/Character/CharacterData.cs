using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class CharacterData : ScriptableObject {

        [SerializeField]
        private string _firstNameKey;

        [SerializeField]
        private string _lastNameKey;

        [SerializeField, Resource(typeof(Sprite))]
        private string[] _alternativePortraits;

        [SerializeField, Resource(typeof(GameObject))]
        private string _prefab;

        [SerializeField, Resource(typeof(Sprite))]
        private string _icon;

        private Resource<Sprite> _iconResource;
        private Resource<GameObject> _prefabResource;
        private Resource<Sprite>[] _portraitResources; 

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
            get { return _alternativePortraits == null ? 0 : _alternativePortraits.Length; }
        }

        public Resource<Sprite> Icon {
            get { return _iconResource; }
        }

        public Sprite LoadPortrait(int alternativeChoice) {
            if (alternativeChoice < 0 || alternativeChoice >= AlternativeCount)
                throw new ArgumentException();
            return _portraitResources[alternativeChoice].Load();
        }

        public Character LoadPrefab() {
            return _prefabResource.Load().GetComponent<Character>();
        }

        private void OnEnable() {
            if (_alternativePortraits == null)
                return;
            _iconResource = new Resource<Sprite>(_icon);
            _prefabResource = new Resource<GameObject>(_prefab);
            _portraitResources = new Resource<Sprite>[_alternativePortraits.Length];
            for(var i = 0; i < _alternativePortraits.Length; i++)
                _portraitResources[i] = new Resource<Sprite>(_alternativePortraits[i]);
        }

    }

}
using System;
using UnityEngine;

namespace Hourai.SmashBrew {

    public class CharacterData : ScriptableObject {

        [SerializeField]
        private string _shortNameKey;

        [SerializeField]
        private string _fullNameKey;

        [SerializeField, Resource(typeof(Sprite))]
        private string[] _alternativePortraits;

        [SerializeField, Resource(typeof(GameObject))]
        private string _prefab;

        [SerializeField, Resource(typeof(Sprite))]
        private string _icon;

        private Resource<Sprite> _iconResource;
        private Resource<GameObject> _prefabResource;
        private Resource<Sprite>[] _portraitResources; 

        public string ShortName {
            get { return _shortNameKey; }
        }

        public string FullName {
            get { return _fullNameKey; }
        }

        public string Name {
            get { return ShortName + " " + FullName; }
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
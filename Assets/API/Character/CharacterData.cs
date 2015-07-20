using UnityEngine;
using System.Collections;
using SmartLocalization;

namespace Genso.API {

    public class CharacterData : ScriptableObject {

        private LanguageManager _languageManager;

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

        [SerializeField]
        private string announcerKey;

        [SerializeField, ResourcePath(typeof(Sprite))]
        private string portrait;
        private Resource<Sprite> _portrait;
        public Resource<Sprite> Portrait {
            get {
                if(_portrait == null)
                    _portrait = new Resource<Sprite>(portrait);
                return _portrait;
            }    
        }

        [SerializeField, ResourcePath(typeof(GameObject))]
        private string _prefab;
		private Resource<GameObject> _prefabResource;

		public Character Prefab {
			get {
				return _prefabResource.Load().GetComponent<Character>();
			}
		}

        void OnEnable() {
            _languageManager = LanguageManager.Instance;
			_prefabResource = new Resource<GameObject>(_prefab);
        }

    }

}


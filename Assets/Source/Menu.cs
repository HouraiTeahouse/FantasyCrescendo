using UnityEngine;

namespace HouraiTeahouse {

    public class Menu : MonoBehaviour {

        [SerializeField]
        string _name;

        public string Name {
            get { return _name; }
        }

        void Reset() { _name = name; }

    }

}
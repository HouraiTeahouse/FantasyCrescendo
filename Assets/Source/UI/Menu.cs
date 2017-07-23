using UnityEngine;

namespace HouraiTeahouse {

    public class Menu : MonoBehaviour {

        [SerializeField]
        string _name;

        public string Name {
            get { return _name; }
        }

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake() {
            Log.Debug("HELLO");
            var manager = MenuManager.Instance;
            if (manager == null) {
                Log.Error("No MenuManager available for {0} to register itself onto.", name);
                return;
            }
            manager.Register(this);
        }

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy() {
            var manager = MenuManager.Instance;
            if (manager == null)
                return;
            manager.Unregister(this);
        }

        void Reset() { _name = name; }

    }

}
using UnityEngine;

namespace HouraiTeahouse {

    public class DisableObject : SingleActionBehaviour {

        private enum Method {
            Enable,
            Disable,
            Toggle
        }

        [SerializeField]
        private GameObject[] _objects;

        [SerializeField]
        private Method _method; 

        protected override void Action() {
            foreach (GameObject o in _objects) {
                switch (_method) {
                    case Method.Enable:
                        o.SetActive(true);
                        break;
                    case Method.Disable:
                        o.SetActive(false);
                        break;
                    default:
                    case Method.Toggle:
                        o.SetActive(!o.activeSelf);
                        break;
                }
            }
        }
    }
}

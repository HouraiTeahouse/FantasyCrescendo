using UnityEngine;

namespace HouraiTeahouse {

    public class DisableObject : SingleActionBehaviour {

        enum Method {

            Enable,
            Disable,
            Toggle

        }

        [SerializeField]
        Method _method;

        [SerializeField]
        GameObject[] _objects;

        public override void Action() {
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
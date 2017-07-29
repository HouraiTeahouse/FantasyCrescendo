using UnityEngine;
using UnityEngine.UI;

namespace HouraiTeahouse {

    public class CreateObject : SingleActionBehaviour {

        [SerializeField]
        bool _copyPosiiton;

        [SerializeField]
        bool _copyRotation;

        [SerializeField]
        Object _object;

        public System.Action<Object> OnCreate;

        public override void Action() {
            if (!_object)
                return;
            Object obj = Instantiate(_object);
            if (!_copyPosiiton && !_copyRotation)
                return;
            var go = obj as GameObject;
            var comp = obj as Component;
            Transform objTransform = null;
            if (go != null)
                objTransform = go.transform;
            else if (comp != null)
                objTransform = comp.transform;
            if (!objTransform)
                return;
            var rTransform = objTransform as RectTransform;
            if (rTransform) {
                //if (_copyPosiiton)
                //    rTransform.anchoredPosition3D = transform.position;
                //if (_copyRotation)
                //    rTransform.rotation = transform.rotation;
                LayoutRebuilder.MarkLayoutForRebuild(rTransform);
            }
            else {
                if (_copyPosiiton)
                    objTransform.position = transform.position;
                if (_copyRotation)
                    objTransform.rotation = transform.rotation;
            }
            if (OnCreate != null)
                OnCreate(obj);
        }

    }

}
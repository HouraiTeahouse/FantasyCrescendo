using UnityEngine;

namespace HouraiTeahouse {
    /// <summary>
    /// A SingleActionBehaviour that attaches one object to another.
    /// </summary>
    public class AttachToObject : SingleActionBehaviour {
        [SerializeField, Tooltip("The parent object to attach the child to")] private Transform _parent;

        [SerializeField, Tag, Tooltip("The tag to search for if a parent is not provided.")] private string _tag;

        [SerializeField, Tooltip("The child to attach to the parent.")] private Transform _child;

        [SerializeField, Tooltip("Whether to keep the child's world position when attaching to the parent")] private
            bool _keepWorldPosition;

        [SerializeField, Tooltip("The sibiling index to set the child to. Set as -1 to leave as is.")] private int
            _siblingIndex = -1;

        /// <summary>
        /// Unity callback. Called when the Editor resets the object.
        /// </summary>
        void Reset() {
            _child = transform;
        }

        /// <summary>
        /// <see cref="SingleActionBehaviour.Action"/>
        /// </summary>
        protected override void Action() {
            Transform child = _child;
            Transform parent = _parent;
            if (!parent) {
                GameObject go = GameObject.FindWithTag(_tag);
                if (go)
                    parent = go.transform;
            }
            if (parent) {
                child.SetParent(parent, _keepWorldPosition);
                if (_siblingIndex >= 0)
                    child.SetSiblingIndex(_siblingIndex);
            }
        }
    }
}

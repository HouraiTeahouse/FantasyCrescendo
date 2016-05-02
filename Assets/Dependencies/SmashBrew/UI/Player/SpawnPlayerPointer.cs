using System.Collections;
using HouraiTeahouse.SmashBrew;
using HouraiTeahouse.SmashBrew.UI;
using UnityEngine;

namespace HouraiTeahouse {
    public class SpawnPlayerPointer : PlayerUIComponent {
        [SerializeField]
        private RectTransform _pointer;

        [SerializeField, Tag]
        private string _tag;

        private RectTransform _cTransform;
        private RectTransform _currentPointer;

        /// <summary>
        /// Unity callback. Called on object instaniation.
        /// </summary>
        protected override void Awake() {
            base.Awake();
            GameObject go = GameObject.FindWithTag(_tag);
            _currentPointer = Instantiate(_pointer);
            _currentPointer.SetParent(go.transform);
            _cTransform = _currentPointer.GetComponentInParent<Canvas>().transform as RectTransform;
            StartCoroutine(Test());
        }

        // Ridiculously hacky coroutine to get the pointer to the right location 
        IEnumerator Test() {
            // need to wait two frames frames before everything is properly settled
            yield return null;
            yield return null;
            Vector2 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
            _currentPointer.anchoredPosition3D = viewportPos.Mult(_cTransform.sizeDelta) - 0.5f * _cTransform.sizeDelta;
            _currentPointer.localScale = Vector3.one;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnDestroy() {
            base.OnDestroy();
            if (_currentPointer)
                Destroy(_currentPointer.gameObject);
        }

        protected override void OnPlayerChange() {
            base.OnPlayerChange();
            if (_currentPointer)
                _currentPointer.GetComponentsInChildren<IDataComponent<Player>>().SetData(Player);
        }
    }
}

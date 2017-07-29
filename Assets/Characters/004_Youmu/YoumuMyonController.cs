using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

    public class YoumuMyonController : NetworkBehaviour {

        [SerializeField]
        [Resource(typeof(GameObject))]
        string _myonPrefab;

        [SerializeField]
        Transform _targetBone;

        [SerializeField]
        [Range(0f, 1f)]
        float _movementSpeed = 1f;


        Transform _myonInstance;

        void Awake() {
            if (_targetBone == null)
                _targetBone = transform;
        }

        public override void OnStartServer() {
            Resource.Get<GameObject>(_myonPrefab).LoadAsync().Then(prefab => {
                if (prefab == null)
                    return;
                _myonInstance = Instantiate(prefab).transform;
                NetworkServer.Spawn(_myonInstance.gameObject);
                _myonInstance.position = _targetBone.position;
            });
        }

        void Update() {
            if (!isServer || _myonInstance == null)
                return;
            var currentPos = _myonInstance.position;
            var targetPos = _targetBone.position;
            float distance = Vector3.Distance(_myonInstance.position, _targetBone.position);
            //Debug.Log("Distance" + distance);
            targetPos = Vector3.Lerp(currentPos, targetPos,  Time.smoothDeltaTime*distance* _movementSpeed);
            //targetPos = Vector3.Lerp(currentPos, targetPos, _movementSpeed);
            
            _myonInstance.position = targetPos;
        }

    }

}
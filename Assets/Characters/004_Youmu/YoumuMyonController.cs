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
        float _movementSpeed = 0.75f;

        [SyncVar]
        NetworkIdentity _myonInstance;

        void Awake() {
            if (_targetBone == null)
                _targetBone = transform;
        }

        public override void OnStartServer() {
            Resource.Get<GameObject>(_myonPrefab).LoadAsync().Then(prefab => {
                if (prefab == null)
                    return;
                var myon = Instantiate(prefab).GetComponent<NetworkIdentity>();
                myon.transform.position = _targetBone.position;
                NetworkServer.SpawnWithClientAuthority(myon.gameObject, connectionToClient);
                _myonInstance = myon;
            });
        }

        void Update() {
            if (!hasAuthority && _myonInstance == null)
                return;
            // TODO(james7132): Include Time.deltaTime into this computation
            var currentPos = _myonInstance.transform.position;
            var targetPos = _targetBone.position;
            targetPos = Vector3.Lerp(currentPos, targetPos, _movementSpeed);
            _myonInstance.transform.position = targetPos;
        }

    }

}
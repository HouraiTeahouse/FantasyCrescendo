using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace HouraiTeahouse.FantasyCrescendo {

    // public class YoumuMyonController : NetworkBehaviour {

    //     [SerializeField]
    //     [Resource(typeof(GameObject))]
    //     string _myonPrefab;

    //     [SerializeField]
    //     Transform _targetBone;

    //     [SerializeField]
    //     [Range(0f, 1f)]
    //     float _movementSpeed = 1f;

    //     [SyncVar]
    //     NetworkIdentity _myonInstance;

    //     void Awake() {
    //         if (_targetBone == null)
    //             _targetBone = transform;
    //     }

    //     public override void OnStartServer() {
    //         Resource.Get<GameObject>(_myonPrefab).LoadAsync().Then(prefab => {
    //             if (prefab == null)
    //                 return;
    //             var myon = Instantiate(prefab, _targetBone.position, Quaternion.identity).GetComponent<NetworkIdentity>();
    //             NetworkServer.SpawnWithClientAuthority(myon.gameObject, connectionToClient);
    //             _myonInstance = myon;
    //         });
    //     }

    //     void Update() {
    //         if (!hasAuthority || _myonInstance == null)
    //             return;
    //         var currentPos = _myonInstance.transform.position;
    //         var targetPos = _targetBone.position;
    //         float distance = Vector3.Distance(_myonInstance.transform.position, _targetBone.position);
    //         targetPos = Vector3.Lerp(currentPos, targetPos,  Time.smoothDeltaTime*distance* _movementSpeed);
    //         _myonInstance.transform.position = targetPos;
    //     }

    // }

}

using UnityEngine;
using System.Collections;

namespace Genso.API {


    public class SpawnPoint : MonoBehaviour {

        [SerializeField]
        private bool mirrorSpawn;

        public T Spawn<T>(T prefab) where T : Component {
            T spawned = prefab.Copy(transform.position);
            if(mirrorSpawn)
                spawned.transform.Rotate(0f, 180f, 0f);
            return spawned;
        }

        public GameObject Spawn(GameObject prefab) {
            GameObject spawned = prefab.Copy(transform.position);
            if (mirrorSpawn)
                spawned.transform.Rotate(0f, 180f, 0f);
            return spawned;
        }

    }

}

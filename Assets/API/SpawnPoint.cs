using System;
using UnityEngine;
using System.Collections;
using Object = UnityEngine.Object;

namespace Genso.API {


    public class SpawnPoint : MonoBehaviour {

        [SerializeField]
        private bool mirrorSpawn;

        public T Spawn<T>(T prefab) where T : Object {
            T spawned = prefab.Copy(transform.position);
            EditSpawn(spawned);
            return spawned;
        }

        public GameObject Spawn(GameObject prefab) {
            GameObject spawned = prefab.Copy(transform.position);
            if (mirrorSpawn)
                spawned.transform.Rotate(0f, 180f, 0f);
            return spawned;
        }

        public void EditSpawn(Object spawn)
        {
            var go = spawn as GameObject;
            var comp = spawn as Component;
            Transform spawnTransform;
            if (go != null)
                spawnTransform = go.transform;
            else if (comp != null)
                spawnTransform = comp.transform;
            else
                throw new InvalidCastException();
            spawnTransform.position = transform.position;
            if (mirrorSpawn)
                spawnTransform.Rotate(0f, 180f, 0f);
        }

    }

}

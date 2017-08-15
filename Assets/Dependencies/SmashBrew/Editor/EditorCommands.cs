using System.Collections.Generic;
using System.Linq;
using HouraiTeahouse.SmashBrew.Characters;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

namespace HouraiTeahouse.SmashBrew.Editor {

    class EditorCommands {

        [MenuItem("Smash Brew/Add Offensive Hitbox %h")]
        static void AddOffensiveHitbox() {
            AddHitbox(Hitbox.Type.Offensive);
        }

        [MenuItem("Smash Brew/Add Hurtbox %#h")]
        static void AddHurtbox() {
            AddHitbox(Hitbox.Type.Damageable);
        }

        [MenuItem("Smash Brew/Add Hurtbox %#h", true)]
        [MenuItem("Smash Brew/Add Offensive Hitbox %h", true)]
        static bool AddHitboxValidate() {
            return Selection.gameObjects.Length > 0;
        }

        static void AddHitbox(Hitbox.Type type) {
            var hitboxes = new List<Hitbox>();
            var rootMap = new Dictionary<GameObject, List<Hitbox>>();
            var idGen = new Random();
            Undo.IncrementCurrentGroup();
            foreach (GameObject go in Selection.gameObjects) {
                var hbGo = new GameObject();
                Undo.RegisterCreatedObjectUndo(hbGo, "Create Hitbox GameObject");
                var collider = Undo.AddComponent<SphereCollider>(hbGo);
                var hb = Undo.AddComponent<Hitbox>(hbGo);
                hb.CurrentType = type;
                hitboxes.Add(hb);
                Undo.SetTransformParent(hb.transform, go.transform, "Parent Hitbox");
                hb.transform.Reset();
                Undo.RecordObject(collider, "Edit Collider Size");
                collider.radius = 1f / ((Vector3) (hb.transform.localToWorldMatrix * Vector3.one)).Max();
                var character = hbGo.GetComponentInParent<Character>();
                GameObject rootGo = character != null ? character.gameObject : hb.transform.root.gameObject;
                if (!rootMap.ContainsKey(rootGo))
                    rootMap[rootGo] = new List<Hitbox>();
                rootMap[rootGo].Add(hb);
            }
            foreach (KeyValuePair<GameObject, List<Hitbox>> set in rootMap) {
                Hitbox[] allHitboxes = set.Key.GetComponentsInChildren<Hitbox>();
                int i = allHitboxes.Length - set.Value.Count;
                Undo.RecordObjects(set.Value.ToArray(), "Name Changes");
                foreach (Hitbox hitbox in set.Value) {
                    hitbox.name = string.Format("{0}_hb_{1}_{2}", set.Key.name, type, i).ToLower();
                    i++;
                }
            }
            Selection.objects = hitboxes.GetGameObject().ToArray();
            Undo.SetCurrentGroupName(string.Format("Generate {0} Hitbox{1}",
                type,
                hitboxes.Count > 0 ? "es" : string.Empty));
        }

    }

}

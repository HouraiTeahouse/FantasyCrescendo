using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Hourai.Editor {

    public class HouraiEditor : MonoBehaviour {

        [MenuItem("Assets/Create/BGM Group")]
        static void CreateStageData() {
            AssetUtil.CreateAssetInProjectWindow<BGMGroup>();
        }

        [MenuItem("Assets/Create/Sound Effect from Clip")]
        static void CreateSoundEffect() {
            var tempGO = new GameObject("Temp", typeof (SoundEffect));
            var audioSource = tempGO.GetComponent<AudioSource>();
            var prefabs = new List<Object>();
            foreach (var clip in Selection.objects.OfType<AudioClip>()) {
                if(!clip)
                    continue;
                audioSource.clip = clip;
                tempGO.name = "sfx_" + clip.name;
                string folder = AssetUtil.GetAssetFolderPath(clip); 
                Object prefab = PrefabUtil.CreatePrefab(folder, tempGO);
                prefabs.Add(prefab);
            }
            Selection.objects = prefabs.ToArray();
            DestroyImmediate(tempGO);
        }

        [MenuItem("Assets/Create/Sound Effect from Clip", true)]
        static bool CreateSoundEffectTest() {
            return Selection.objects.OfType<AudioClip>().Any();
        }

        [MenuItem("Assets/Create/BGM Group from Clips")]
        static void CreateBGMGroupFromCLips() {
            List<string> resourcePaths = new List<string>();
            foreach (var clip in Selection.objects.OfType<AudioClip>()) {
                if (!clip)
                    continue;
                string assetPath = AssetDatabase.GetAssetPath(clip);
                if(!assetPath.Contains("Resources/"))
                    continue;
                string resourcePath = Regex.Replace(assetPath, ".*/Resources/(.*)", "$1");
                if (!string.IsNullOrEmpty(resourcePath))
                    resourcePaths.Add(resourcePath);
            }
            var group = ScriptableObject.CreateInstance<BGMGroup>();
            group.SetBGMClips(resourcePaths);
            AssetUtil.CreateAssetInProjectWindow(group);
        }

    }

}

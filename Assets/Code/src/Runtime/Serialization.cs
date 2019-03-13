using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public class GameObjectReference : AssetReferenceT<GameObject> {
}

[Serializable]
public class SpriteReference : AssetReferenceT<Sprite> {

    public override bool ValidateAsset(string path) {
#if UNITY_EDITOR
        var type = AssetDatabase.GetMainAssetTypeAtPath(path);
        bool isTexture = typeof(Texture2D).IsAssignableFrom(type);
        if (isTexture) {
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            return (importer != null) && (importer.spriteImportMode != SpriteImportMode.None);
        }
#endif
        return false;
    }

}

[Serializable]
public class SceneDataReference : AssetReferenceT<SceneData> {
}

[Serializable]
public class AudioClipReference : AssetReferenceT<AudioClip> {
}
    
}
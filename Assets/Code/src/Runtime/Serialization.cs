using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public class GameObjectReference : AssetReferenceT<GameObject> {
    public GameObjectReference(string guid) : base(guid) {}
}

[Serializable]
public class SpriteReference : AssetReferenceT<Sprite> {

    public SpriteReference(string guid) : base(guid) {}

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
    public SceneDataReference(string guid) : base(guid) {}
}

[Serializable]
public class AudioClipReference : AssetReferenceT<AudioClip> {
    public AudioClipReference(string guid) : base(guid) {}
}
    
}
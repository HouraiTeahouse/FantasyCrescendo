using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HouraiTeahouse.FantasyCrescendo {

[Serializable]
public class GameObjectReference : AssetReferenceT<GameObject> {
}

[Serializable]
public class SpriteReference : AssetReferenceT<Sprite> {
}

[Serializable]
public class SceneDataReference : AssetReferenceT<SceneData> {
}

[Serializable]
public class AudioClipReference : AssetReferenceT<AudioClip> {
}
    
}
using UnityEngine;

public class TextureQualityEnforcer : MonoBehaviour {

    int _cachedLevel;

    [SerializeField]
    int _level;

    void Awake() {
        _cachedLevel = QualitySettings.masterTextureLimit;
        QualitySettings.masterTextureLimit = _level;
    }

    void OnDestroy() { QualitySettings.masterTextureLimit = _cachedLevel; }

}
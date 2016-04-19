using UnityEngine;

public class TextureQualityEnforcer : MonoBehaviour {

    [SerializeField]
    private int _level;
    private int _cachedLevel;

    void Awake() {
        _cachedLevel = QualitySettings.masterTextureLimit;
        Debug.Log(_level - _cachedLevel);
        QualitySettings.masterTextureLimit = _level;
    }

    void OnDestroy() {
        QualitySettings.masterTextureLimit = _cachedLevel;
    }

}

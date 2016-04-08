using UnityEngine;
using System.Collections;

public class QualityLevelPrinter : MonoBehaviour {

    void Awake() {
        Debug.Log(QualitySettings.names[QualitySettings.GetQualityLevel()]);
    }

}

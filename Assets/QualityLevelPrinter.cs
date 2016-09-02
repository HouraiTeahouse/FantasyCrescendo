using UnityEngine;

namespace HouraiTeahouse {

    public class QualityLevelPrinter : MonoBehaviour {

        void Awake() { Log.Info("Quality Level: {0}", QualitySettings.names[QualitySettings.GetQualityLevel()]); }

    }

}
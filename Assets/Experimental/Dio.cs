using UnityEngine;

namespace HouraiTeahouse {

    [ExecuteInEditMode]
    public class Dio : MonoBehaviour {

        [SerializeField]
        [Range(0, 1)]
        float _innerRatio;

        Material _mat;

        [SerializeField]
        [Range(0, 1)]
        float _outerRatio;

        [SerializeField, HideInInspector]
        Shader _shader;

        [SerializeField]
        Vector2 center;

        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            if (_mat == null) {
                if (_shader == null) {
                    enabled = false;
                    return;
                }
                _mat = new Material(_shader);
            }

            float aspectRatio = Screen.width / (float) Screen.height;

            _mat.SetVector("_Aspect", new Vector4(aspectRatio, 1, 1, 1));
            _mat.SetVector("_Center", new Vector4(center.x, center.y, _innerRatio, _outerRatio));

            Graphics.Blit(src, dst, _mat);
        }

    }

}
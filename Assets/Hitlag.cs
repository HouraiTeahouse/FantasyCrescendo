using UnityEngine;

namespace Hourai.SmashBrew {

    [RequiredCharacterComponent]
    [DisallowMultipleComponent]
    public class Hitlag : Status {

        private float _cachedTimeScale;

        protected override float GetDeltaTime() {
            return Time.unscaledDeltaTime;
        }

        protected override void OnStatusStart() {
            base.OnStatusStart();
            _cachedTimeScale = LocalTimeScale;
            LocalTimeScale = 0f;
        }

        protected override void OnStatusEnd() {
            base.OnStatusEnd();
            LocalTimeScale = _cachedTimeScale;
        }

    }

}
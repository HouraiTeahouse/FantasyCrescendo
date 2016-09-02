using UnityEngine;

namespace HouraiTeahouse.SmashBrew {

    /// <summary> A Status effect that keeps a character frozen for a short amount of time after being hit. </summary>
    [Required]
    [DisallowMultipleComponent]
    public class Hitlag : Status {

        float _cachedTimeScale;

        /// <summary>
        ///     <see cref="Status.GetDeltaTime" />
        /// </summary>
        protected override float GetDeltaTime() { return Time.unscaledDeltaTime; }

        /// <summary>
        ///     <see cref="Status.OnStatusStart" />
        /// </summary>
        protected override void OnStatusStart() {
            base.OnStatusStart();
            _cachedTimeScale = LocalTimeScale;
            LocalTimeScale = 0f;
        }

        /// <summary>
        ///     <see cref="Status.OnStatusEnd" />
        /// </summary>
        protected override void OnStatusEnd() {
            base.OnStatusEnd();
            LocalTimeScale = _cachedTimeScale;
        }

    }

}
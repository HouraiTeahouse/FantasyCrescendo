using UnityEngine;

#pragma warning disable 0660, 0661

namespace HouraiTeahouse.HouraiInput {

    public struct InputState {

        public bool State;
        public float Value;

        public void Reset() {
            Value = 0.0f;
            State = false;
        }

        public void Set(float value) {
            Value = value;
            State = !Mathf.Approximately(value, 0.0f);
        }

        public void Set(float value, float threshold) {
            Value = value;
            State = Mathf.Abs(value) > threshold;
        }

        public void Set(bool state) {
            State = state;
            Value = state ? 1.0f : 0.0f;
        }

        public static implicit operator bool(InputState state) { return state.State; }

        public static implicit operator float(InputState state) { return state.Value; }

        public static bool operator ==(InputState a, InputState b) { return Mathf.Approximately(a.Value, b.Value); }

        public static bool operator !=(InputState a, InputState b) { return !Mathf.Approximately(a.Value, b.Value); }

    }

}
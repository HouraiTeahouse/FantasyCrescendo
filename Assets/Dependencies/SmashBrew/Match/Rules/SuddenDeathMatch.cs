using UnityEngine;
using System.Collections;

namespace HouraiTeahouse.SmashBrew {
    public sealed class SuddenDeathMatch : MatchRule {
        protected override void Awake() {
            base.Awake();
        }


        public override Player GetWinner() {
            return null;
        }
    }
}
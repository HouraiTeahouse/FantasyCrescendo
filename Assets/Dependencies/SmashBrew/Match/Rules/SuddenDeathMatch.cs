namespace HouraiTeahouse.SmashBrew {

    public sealed class SuddenDeathMatch : MatchRule {

        protected override void Start() { base.Start(); }

        public override Player GetWinner() { return null; }

    }

}
